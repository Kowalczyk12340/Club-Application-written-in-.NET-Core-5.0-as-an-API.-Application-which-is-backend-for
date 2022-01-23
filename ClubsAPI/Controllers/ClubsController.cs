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
using ClubsAPI.Services.Interfaces;

namespace ClubsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class ClubsController : ControllerBase
  {
    private readonly IClubsService _clubsService;

    public ClubsController(IClubsService clubsService)
    {
      _clubsService = clubsService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<HomeDto>> Get()
    {
      var result = await _clubsService.Get();
      return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ClubDto>> Get(int id)
    {
      var result = await _clubsService.Get(id);
      return Ok(result);
    }

    [HttpGet("filter")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ClubDto>>> Filter([FromQuery] FilterClubsDto filterClubsDto)
    {
      var result = await _clubsService.Filter(filterClubsDto);
      return Ok(result);
    }

    [HttpGet("PostGet")]
    public async Task<ActionResult<ClubPostGetDto>> PostGet()
    {
      var result = await _clubsService.PostGet();
      return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Post([FromForm] ClubCreationDto clubCreationDto)
    {
      var result = await _clubsService.Post(clubCreationDto);
      return Ok(result);
    }

    [HttpGet("putget/{id:int}")]
    public async Task<ActionResult<ClubPutGetDto>> PutGet(int id)
    {
      var result = await _clubsService.PutGet(id);
      return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] ClubCreationDto clubCreationDto)
    {
      await _clubsService.Put(id, clubCreationDto);
      return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      await _clubsService.Delete(id);
      return NoContent();
    }
  }
}