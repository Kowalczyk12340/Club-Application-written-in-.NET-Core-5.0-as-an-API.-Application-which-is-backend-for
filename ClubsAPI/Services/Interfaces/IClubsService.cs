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

namespace ClubsAPI.Services.Interfaces
{
  public interface IClubsService
  {
    Task<HomeDto> Get();
    Task<ClubDto> Get(int id);
    Task<List<ClubDto>> Filter([FromQuery] FilterClubsDto filterClubsDto);
    Task<ClubPostGetDto> PostGet();
    Task<int> Post([FromForm] ClubCreationDto clubCreationDto);
    Task<ClubPutGetDto> PutGet(int id);
    Task Put(int id, [FromForm] ClubCreationDto clubCreationDto);
    Task Delete(int id);
  }
}