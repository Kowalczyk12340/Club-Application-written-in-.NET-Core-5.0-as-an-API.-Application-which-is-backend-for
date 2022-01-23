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
using ClubsAPI.Services.Interfaces;

namespace ClubsAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class ClubLeaguesController : ControllerBase
  {
    private readonly IClubLeaguesService _clubLeaguesService;

    public ClubLeaguesController(IClubLeaguesService clubLeaguesService)
    {
      _clubLeaguesService = clubLeaguesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClubLeagueDto>>> Get()
    {
      var result = await _clubLeaguesService.Get();
      return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClubLeagueDto>> Get(int id)
    {
      var result = await _clubLeaguesService.Get(id);
      return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post(ClubLeagueCreationDto clubLeagueCreation)
    {
      await _clubLeaguesService.Post(clubLeagueCreation);
      return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, ClubLeagueCreationDto clubLeagueCreationDto)
    {
      await _clubLeaguesService.Put(id, clubLeagueCreationDto);
      return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      await _clubLeaguesService.Delete(id);
      return NoContent();
    }
  }
}