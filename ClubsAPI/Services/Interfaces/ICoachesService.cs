using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

namespace ClubsAPI.Services.Interfaces
{
  public interface ICoachesService
  {
    Task<List<CoachDto>> Get([FromQuery] PaginationDto paginationDto);
    Task<List<CoachesClubDto>> SearchByName([FromBody] string name);
    Task<CoachDto> Get(int id);
    Task Post([FromForm] CoachCreationDto coachCreationDto);
    Task Put(int id, [FromForm] CoachCreationDto coachCreationDto);
    Task Delete(int id);
    string SaveToCsv(IEnumerable<CoachDto> components);
  }
}