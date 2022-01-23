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
using ClubsAPI.Services.Interfaces;

namespace ClubsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class PlayersController : ControllerBase
  {
    private readonly IPlayersService _playersService;

    public PlayersController(IPlayersService playersService)
    {
      _playersService = playersService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlayerDto>>> Get([FromQuery] PaginationDto paginationDto)
    {
      var result = await _playersService.Get(paginationDto);
      return Ok(result);
    }

    [HttpPost("searchByName")]
    public async Task<ActionResult<List<PlayersClubDto>>> SearchByName([FromBody] string name)
    {
      var result = await _playersService.SearchByName(name);
      return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerDto>> Get(int id)
    {
      var result = await _playersService.Get(id);
      return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] PlayerCreationDto playerCreationDto)
    {
      await _playersService.Post(playerCreationDto);
      return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] PlayerCreationDto playerCreationDto)
    {
      await _playersService.Put(id, playerCreationDto);
      return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      await _playersService.Delete(id);
      return NoContent();
    }
  }
}