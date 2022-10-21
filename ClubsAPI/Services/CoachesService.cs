using AutoMapper;
using Microsoft.AspNetCore.Http;
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
  public class CoachesService : ICoachesService
  {
    private readonly ApplicationDataContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly string containerName = "coaches";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CoachesService(ApplicationDataContext context, IMapper mapper,
        IFileStorageService fileStorageService, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _mapper = mapper;
      _fileStorageService = fileStorageService;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<CoachDto>> Get([FromQuery] PaginationDto paginationDto)
    {
      var queryable = _context.Coach.AsQueryable();
      await _httpContextAccessor.HttpContext.InsertParametersPaginationInHeader(queryable);
      var coaches = await queryable.OrderBy(x => x.Name).Paginate(paginationDto).ToListAsync();
      return _mapper.Map<List<CoachDto>>(coaches);
    }

    public async Task<List<CoachesClubDto>> SearchByName([FromBody] string name)
    {
      if (string.IsNullOrWhiteSpace(name)) { return new List<CoachesClubDto>(); }
      return await _context.Coach
          .Where(x => x.Name.Contains(name))
          .OrderBy(x => x.Name)
          .Select(x => new CoachesClubDto() { Id = x.Id, Name = x.Name, Picture = x.Picture })
          .Take(5)
          .ToListAsync();
    }

    public async Task<CoachDto> Get(int id)
    {
      var coach = await _context.Coach.FirstOrDefaultAsync(x => x.Id == id);

      if (coach == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      return _mapper.Map<CoachDto>(coach);
    }

    public async Task Post([FromForm] CoachCreationDto coachCreationDto)
    {
      var coach = _mapper.Map<Coach>(coachCreationDto);

      if (coachCreationDto.Picture != null)
      {
        coach.Picture = await _fileStorageService.SaveFile(containerName, coachCreationDto.Picture);
      }

      _context.Add(coach);
      await _context.SaveChangesAsync();
    }

    public async Task Put(int id, [FromForm] CoachCreationDto coachCreationDto)
    {
      var coach = await _context.Coach.FirstOrDefaultAsync(x => x.Id == id);

      if (coach == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      coach = _mapper.Map(coachCreationDto, coach);

      if (coachCreationDto.Picture != null)
      {
        coach.Picture = await _fileStorageService.EditFile(containerName,
            coachCreationDto.Picture, coach.Picture);
      }

      await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
      var coach = await _context.Coach.FirstOrDefaultAsync(x => x.Id == id);

      if (coach == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      _context.Remove(coach);
      await _context.SaveChangesAsync();

      await _fileStorageService.DeleteFile(coach.Picture, containerName);
    }

    public string SaveToCsv(IEnumerable<CoachDto> components)
    {
      var headers = "Id;Name;DateOfBirth;Biography;Picture;";

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