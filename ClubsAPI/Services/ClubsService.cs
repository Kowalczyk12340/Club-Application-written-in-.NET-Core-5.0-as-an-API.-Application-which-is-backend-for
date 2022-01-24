using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClubsAPI.Data;
using ClubsAPI.DTOs;
using ClubsAPI.Entities;
using ClubsAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubsAPI.Services.Interfaces;
using ClubsAPI.Exceptions;
using System.Text;

namespace ClubsAPI.Services
{
  public class ClubsService : IClubsService
  {
    private readonly ApplicationDataContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly UserManager<IdentityUser> _userManager;
    private string container = "clubs";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClubsService(ApplicationDataContext context, IMapper mapper,
        IFileStorageService fileStorageService,
        UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _mapper = mapper;
      _fileStorageService = fileStorageService;
      _userManager = userManager;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<HomeDto> Get()
    {
      var top = 6;
      var today = DateTime.Today;

      var upcomingReleases = await _context.Clubs
          .Where(x => x.ReleaseDate > today)
          .OrderBy(x => x.ReleaseDate)
          .Take(top)
          .ToListAsync();

      var inTheaters = await _context.Clubs
          .Where(x => x.ReleaseDate > today)
          .OrderBy(x => x.ReleaseDate)
          .Take(top)
          .ToListAsync();

      var homeDto = new HomeDto();
      homeDto.UpcomingReleases = _mapper.Map<List<ClubDto>>(upcomingReleases);
      homeDto.HasOwnStadium = _mapper.Map<List<ClubDto>>(inTheaters);
      return homeDto;
    }

    public async Task<ClubDto> Get(int id)
    {
      var club = await _context.Clubs
          .Include(x => x.ClubsNationalities).ThenInclude(x => x.Nationality)
          .Include(x => x.ClubLeaguesClubs).ThenInclude(x => x.ClubLeague)
          .Include(x => x.ClubsPlayers).ThenInclude(x => x.Player)
          .Include(x => x.ClubsCoaches).ThenInclude(x => x.Coach)
          .FirstOrDefaultAsync(x => x.Id == id);

      if (club == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      var averageVote = 0.0;
      var userVote = 0;

      if (await _context.Ratings.AnyAsync(x => x.ClubId == id))
      {
        averageVote = await _context.Ratings.Where(x => x.ClubId == id)
            .AverageAsync(x => x.Rate);

        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
          var email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
          var user = await _userManager.FindByEmailAsync(email);
          var userId = user.Id;

          var ratingDb = await _context.Ratings.FirstOrDefaultAsync(x => x.ClubId == id
          && x.UserId == userId);

          if (ratingDb != null)
          {
            userVote = ratingDb.Rate;
          }
        }
      }

      var dto = _mapper.Map<ClubDto>(club);

      dto.AverageVote = averageVote;
      dto.UserVote = userVote;

      dto.Players = dto.Players.OrderBy(x => x.Order).ToList();
      dto.Coaches = dto.Coaches.OrderBy(x => x.Order).ToList();
      return dto;
    }

    public async Task<List<ClubDto>> Filter([FromQuery] FilterClubsDto filterClubsDto)
    {
      var clubsQueryable = _context.Clubs.AsQueryable();

      if (!string.IsNullOrEmpty(filterClubsDto.ClubName))
      {
        clubsQueryable = clubsQueryable.Where(x => x.ClubName.Contains(filterClubsDto.ClubName));
      }

      if (filterClubsDto.HasOwnStadium)
      {
        clubsQueryable = clubsQueryable.Where(x => x.HasOwnStadium);
      }

      if (filterClubsDto.NationalityId != 0)
      {
        clubsQueryable = clubsQueryable
            .Where(x => x.ClubsNationalities.Select(y => y.NationalityId)
            .Contains(filterClubsDto.NationalityId));
      }

      await _httpContextAccessor.HttpContext.InsertParametersPaginationInHeader(clubsQueryable);
      var clubs = await clubsQueryable.OrderBy(x => x.ClubName).Paginate(filterClubsDto.PaginationDto)
          .ToListAsync();

      return _mapper.Map<List<ClubDto>>(clubs);
    }

    public async Task<ClubPostGetDto> PostGet()
    {
      var clubLeagues = await _context.ClubLeagues.OrderBy(x => x.Name).ToListAsync();
      var nationalities = await _context.Nationalities.OrderBy(x => x.Name).ToListAsync();

      var clubLeaguesDto = _mapper.Map<List<ClubLeagueDto>>(clubLeagues);
      var nationalitiesDto = _mapper.Map<List<NationalityDto>>(nationalities);

      return new ClubPostGetDto() { Nationalities = nationalitiesDto, ClubLeagues = clubLeaguesDto };
    }

    public async Task<int> Post([FromForm] ClubCreationDto clubCreationDto)
    {
      var club = _mapper.Map<Club>(clubCreationDto);

      if (clubCreationDto.Poster != null)
      {
        club.Poster = await _fileStorageService.SaveFile(container, clubCreationDto.Poster);
      }

      AnnotatePlayersOrder(club);
      AnnotateCoachesOrder(club);
      _context.Add(club);
      await _context.SaveChangesAsync();
      return club.Id;
    }

    public async Task<ClubPutGetDto> PutGet(int id)
    {
      var clubActionResult = await Get(id);
      if (clubActionResult is null) { throw new NotFoundException("Nie znaleziono elementu"); }

      var club = clubActionResult;

      var nationalitieSelectedIds = club.Nationalities.Select(x => x.Id).ToList();
      var nonSelectedNationalities = await _context.Nationalities.Where(x => 
      !nationalitieSelectedIds.Contains(x.Id)).ToListAsync();

      var clubLeaguesIds = club.ClubLeagues.Select(x => x.Id).ToList();
      var nonSelectedClubLeagues = await _context.ClubLeagues.Where(x => 
      !nationalitieSelectedIds.Contains(x.Id)).ToListAsync();

      var nonSelectedNationalitiesDtos = _mapper.Map<List<NationalityDto>>(nonSelectedNationalities);
      var nonSelectedClubLeaguesDto = _mapper.Map<List<ClubLeagueDto>>(nonSelectedClubLeagues);

      var response = new ClubPutGetDto();
      response.Club = club;
      response.SelectedNationalities = club.Nationalities;
      response.NonSelectedNationalities = nonSelectedNationalitiesDtos;
      response.SelectedClubLeagues = club.ClubLeagues;
      response.NonSelectedClubLeagues = nonSelectedClubLeaguesDto;
      response.Players = club.Players;
      response.Coaches = club.Coaches;
      return response;
    }

    private void AnnotatePlayersOrder(Club club)
    {
      if (club.ClubsPlayers != null)
      {
        for (int i = 0; i < club.ClubsPlayers.Count; i++)
        {
          club.ClubsPlayers[i].Order = i;
        }
      }
    }

    private void AnnotateCoachesOrder(Club club)
    {
      if (club.ClubsCoaches != null)
      {
        for (int i = 0; i < club.ClubsCoaches.Count; i++)
        {
          club.ClubsCoaches[i].Order = i;
        }
      }
    }

    public async Task Put(int id, [FromForm] ClubCreationDto clubCreationDto)
    {
      var club = await _context.Clubs
        .Include(x => x.ClubsPlayers)
        .Include(x => x.ClubsCoaches)
          .Include(x => x.ClubsNationalities)
          .Include(x => x.ClubLeaguesClubs)
          .FirstOrDefaultAsync(x => x.Id == id);

      if (club == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      club = _mapper.Map(clubCreationDto, club);

      if (clubCreationDto.Poster != null)
      {
        club.Poster = await _fileStorageService.EditFile(container, clubCreationDto.Poster,
            club.Poster);
      }
      AnnotatePlayersOrder(club);
      AnnotateCoachesOrder(club);
      await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
      var club = await _context.Clubs.FirstOrDefaultAsync(x => x.Id == id);

      if (club == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      _context.Remove(club);
      await _context.SaveChangesAsync();
      await _fileStorageService.DeleteFile(club.Poster, container);
    }

    public string SaveToCsv(IEnumerable<ClubDto> components)
    {
      var headers = "Id;ClubName;Summary;Description;HasOwnStadium;ReleaseDate;Poster;AverageVote;UserVote;Nationalities.Count;ClubLeagues.Count;Players.Count;Coaches.Count;";

      var csv = new StringBuilder(headers);

      csv.Append(Environment.NewLine);

      foreach (var component in components)
      {
        csv.Append(component.GetExportObject());
        csv.Append(Environment.NewLine);
      }
      csv.Append($"Count: {components.Count()}");
      csv.Append(Environment.NewLine);

      return csv.ToString();
    }
  }
}