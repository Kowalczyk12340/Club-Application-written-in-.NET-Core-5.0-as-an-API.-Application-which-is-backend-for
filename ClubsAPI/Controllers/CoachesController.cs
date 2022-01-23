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
using ClubsAPI.Services.Interfaces;

namespace ClubsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class CoachesController : ControllerBase
  {
    private readonly ICoachesService _coachesService;

    public CoachesController(ICoachesService coachesService)
    {
      _coachesService = coachesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CoachDto>>> Get([FromQuery] PaginationDto paginationDto)
    {
      var result = await _coachesService.Get(paginationDto);
      return Ok(result);
    }

    [HttpPost("searchByName")]
    public async Task<ActionResult<List<CoachesClubDto>>> SearchByName([FromBody] string name)
    {
      var result = await _coachesService.SearchByName(name);
      return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CoachDto>> Get(int id)
    {
      var result = await _coachesService.Get(id);
      return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] CoachCreationDto coachCreationDto)
    {
      await _coachesService.Post(coachCreationDto);
      return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] CoachCreationDto coachCreationDto)
    {
      await _coachesService.Put(id, coachCreationDto);
      return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      await _coachesService.Delete(id);
      return NoContent();
    }
  }
}