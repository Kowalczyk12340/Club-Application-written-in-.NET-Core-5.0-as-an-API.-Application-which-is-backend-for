using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ClubsAPI.Data;
using ClubsAPI.DTOs;
using ClubsAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubsAPI.Services.Interfaces;
using ClubsAPI.Exceptions;

namespace ClubsAPI.Services
{
  public class NationalitiesService : INationalitiesService
  {
    private readonly ILogger<INationalitiesService> _logger;
    private readonly ApplicationDataContext _context;
    private readonly IMapper _mapper;

    public NationalitiesService(ILogger<INationalitiesService> logger, ApplicationDataContext context, IMapper mapper)
    {
      _logger = logger;
      _context = context;
      _mapper = mapper;
    }

    public async Task<List<NationalityDto>> Get()
    {
      _logger.LogInformation("Getting all the nationalities");

      var nationalities = await _context.Nationalities.OrderBy(x => x.Name).ToListAsync();
      return _mapper.Map<List<NationalityDto>>(nationalities);
    }

    public async Task<NationalityDto> GetById(int id)
    {
      var nationality = await _context.Nationalities.FirstOrDefaultAsync(x => x.Id == id);

      if (nationality == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      return _mapper.Map<NationalityDto>(nationality);
    }

    public async Task Post([FromBody] NationalityCreationDto nationalityCreationDto)
    {
      var nationality = _mapper.Map<Nationality>(nationalityCreationDto);
      _context.Add(nationality);
      await _context.SaveChangesAsync();
    }

    public async Task Put(int id, [FromBody] NationalityCreationDto nationalityCreationDto)
    {
      var nationality = _mapper.Map<Nationality>(nationalityCreationDto);
      nationality.Id = id;
      _context.Entry(nationality).State = EntityState.Modified;
      await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
      var nationality = await _context.Nationalities.FirstOrDefaultAsync(x => x.Id == id);

      if (nationality == null)
      {
        throw new NotFoundException("Nie znaleziono elementu");
      }

      _context.Remove(nationality);
      await _context.SaveChangesAsync();
    }
  }
}