using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClubsAPI.Data;
using ClubsAPI.DTOs;
using ClubsAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Services.Interfaces
{
  public interface IClubLeaguesService
  {
    Task<List<ClubLeagueDto>> Get();
    Task<ClubLeagueDto> Get(int id);
    Task Post(ClubLeagueCreationDto clubLeagueCreation);
    Task Put(int id, ClubLeagueCreationDto clubLeagueCreationDto);
    Task Delete(int id);
  }
}