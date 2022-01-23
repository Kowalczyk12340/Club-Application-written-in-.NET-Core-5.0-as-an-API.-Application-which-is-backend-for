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

namespace ClubsAPI.Services.Interfaces
{
  public interface INationalitiesService
  {
    Task<List<NationalityDto>> Get();
    Task<NationalityDto> GetById(int id);
    Task Post([FromBody] NationalityCreationDto nationalityCreationDto);
    Task Put(int id, [FromBody] NationalityCreationDto nationalityCreationDto);
    Task Delete(int id);
  }
}