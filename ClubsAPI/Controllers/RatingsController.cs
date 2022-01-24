using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
  [Route("api/[controller]")]
  [ApiController]
  public class RatingsController : ControllerBase
  {
    private readonly IRatingsService _ratingsService;

    /// <summary>
    /// Constructor with Dependency Injection by ratingService
    /// </summary>
    /// <param name="ratingsService"></param>
    public RatingsController(IRatingsService ratingsService)
    {
      _ratingsService = ratingsService;
    }

    /// <summary>
    /// Method to add mark to the rating for chosen club
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Marking from 1 to 5 for club</returns>
    /// <response code="200">Marking from 1 to 5 for club</response>
    /// <response code="500">An error occurred by adding mark</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post([FromBody] RatingDto ratingDto)
    {
      await _ratingsService.Post(ratingDto);
      return Ok();
    }
  }
}