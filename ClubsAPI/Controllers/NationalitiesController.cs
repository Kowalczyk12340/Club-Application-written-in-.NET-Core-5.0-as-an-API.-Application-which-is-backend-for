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

namespace ClubsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class NationalitiesController : ControllerBase
  {
    private readonly INationalitiesService _nationalitiesService;

    public NationalitiesController(INationalitiesService nationalitiesService)
    {
      _nationalitiesService = nationalitiesService;
    }

    [HttpGet] //api/nationalities
    [AllowAnonymous]
    public async Task<ActionResult<List<NationalityDto>>> Get()
    {
      var result = await _nationalitiesService.Get();
      return Ok(result);
    }

    [HttpGet("{Id:int}", Name = "getNationality")] //api/nationalities/example
    public async Task<ActionResult<NationalityDto>> Get(int id)
    {
      var result = await _nationalitiesService.GetById(id);
      return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] NationalityCreationDto nationalityCreationDto)
    {
      await _nationalitiesService.Post(nationalityCreationDto);
      return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] NationalityCreationDto nationalityCreationDto)
    {
      await _nationalitiesService.Put(id, nationalityCreationDto);
      return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      await _nationalitiesService.Delete(id);
      return NoContent();
    }
  }
}