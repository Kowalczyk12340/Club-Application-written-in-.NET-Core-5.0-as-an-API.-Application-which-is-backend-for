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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Services.Interfaces
{
  public interface IPlayersService
  {
    Task<List<PlayerDto>> Get([FromQuery] PaginationDto paginationDto);
    Task<List<PlayersClubDto>> SearchByName([FromBody] string name);
    Task<PlayerDto> Get(int id);
    Task Post([FromForm] PlayerCreationDto playerCreationDto);
    Task Put(int id, [FromForm] PlayerCreationDto playerCreationDto);
    Task Delete(int id);
    string SaveToCsv(IEnumerable<PlayerDto> components);
  }
}