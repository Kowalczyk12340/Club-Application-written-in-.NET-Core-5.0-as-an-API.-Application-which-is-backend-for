using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClubsAPI.Data;
using ClubsAPI.DTOs;
using ClubsAPI.Exceptions;
using ClubsAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubsAPI.Services.Interfaces;
using System.Text;

namespace ClubsAPI.Services
{
  public class ClubLeaguesService : IClubLeaguesService
  {
    private readonly ApplicationDataContext _context;
    private readonly IMapper _mapper;

    public ClubLeaguesService(ApplicationDataContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<List<ClubLeagueDto>> Get()
    {
      var entities = await _context.ClubLeagues.OrderBy(x => x.Name).ToListAsync();
      if (entities == null)
      {
        throw new NotFoundException("Nie znaleziono żadnego elementu");
      }
      return _mapper.Map<List<ClubLeagueDto>>(entities);
    }

    public async Task<ClubLeagueDto> Get(int id)
    {
      var clubLeague = await _context.ClubLeagues.FirstOrDefaultAsync(x => x.Id == id);

      if (clubLeague == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      return _mapper.Map<ClubLeagueDto>(clubLeague);
    }

    public async Task Post(ClubLeagueCreationDto clubLeagueCreation)
    {
      var clubLeague = _mapper.Map<ClubLeague>(clubLeagueCreation);
      _context.Add(clubLeague);
      await _context.SaveChangesAsync();
    }

    public async Task Put(int id, ClubLeagueCreationDto clubLeagueCreationDto)
    {
      var clubLeague = await _context.ClubLeagues.FirstOrDefaultAsync(x => x.Id == id);

      if (clubLeague == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      clubLeague = _mapper.Map(clubLeagueCreationDto, clubLeague);
      await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
      var clubLeague = await _context.ClubLeagues.FirstOrDefaultAsync(x => x.Id == id);

      if (clubLeague == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      _context.Remove(clubLeague);
      await _context.SaveChangesAsync();
    }

    public string SaveToCsv(IEnumerable<ClubLeagueDto> components)
    {
      var headers = "Id;Name;Latitude;Longitude;";

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