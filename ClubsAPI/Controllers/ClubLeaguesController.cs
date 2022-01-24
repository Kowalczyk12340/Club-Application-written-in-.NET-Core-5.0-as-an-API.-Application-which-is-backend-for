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
using Microsoft.AspNetCore.Http;
using System.Text;

namespace ClubsAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class ClubLeaguesController : ControllerBase
  {
    private readonly IClubLeaguesService _clubLeaguesService;

    /// <summary>
    /// Constructor for club leagues controller
    /// </summary>
    /// <param name="clubLeaguesService"></param>
    public ClubLeaguesController(IClubLeaguesService clubLeaguesService)
    {
      _clubLeaguesService = clubLeaguesService;
    }

    /// <summary>
    /// Method to get all leagues
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Ok with list of objects if leagues are fetched successfully or 404 if users
    /// are not found</returns>
    /// <response code="200">Leagues exist and have been successfully fetched</response>
    /// <response code="404">Leagues do not exist</response>
    [ProducesResponseType(typeof(List<ClubLeagueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult<List<ClubLeagueDto>>> Get()
    {
      var result = await _clubLeaguesService.Get();
      return Ok(result);
    }

    /// <summary>
    /// Method to get chosen object by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Ok with object if it is getting object by id is successfully or 404 if it is not found</returns>
    /// <response code="200">League exists and has been successfully fetched</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(ClubLeagueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClubLeagueDto>> Get(int id)
    {
      var result = await _clubLeaguesService.Get(id);
      return Ok(result);
    }

    /// <summary>
    /// Method to add with some parameters an object to the leagues
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Ok if it is added successfully or 500 if any error exists by mapping or validation parameters</returns>
    /// <response code="200">League with object is successfully added</response>
    /// <response code="400">An Error occurred by trying to add league</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult> Post(ClubLeagueCreationDto clubLeagueCreation)
    {
      await _clubLeaguesService.Post(clubLeagueCreation);
      return Ok();
    }

    /// <summary>
    /// Method to edit chosen some parameters in object after choosing league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Ok if it is edited successfully or 404 if not found</returns>
    /// <response code="200">League with chosen id exists and has been successfully edited</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, ClubLeagueCreationDto clubLeagueCreationDto)
    {
      await _clubLeaguesService.Put(id, clubLeagueCreationDto);
      return Ok();
    }

    /// <summary>
    /// Method to delete chosen club league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoContent if it is deleted successfully or 404 if not found</returns>
    /// <response code="204">League with chosen id exists and has been successfully deleted</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      await _clubLeaguesService.Delete(id);
      return NoContent();
    }

    /// <summary>
    /// Method to export chosen club league to the csv file
    /// </summary>
    /// <param name="id"></param>
    /// <returns>File with csv extensions</returns>
    /// <response code="200">Leagues exist and have been successfully save to csv file</response>
    /// <response code="404">Leagues do not exist</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("exporttoexcel")]
    public async Task<IActionResult> SaveToCsv()
    {
      var date = DateTime.UtcNow;
      var result = await _clubLeaguesService.Get();
      if (result == null)
      {
        return NotFound();
      }
      var csv = _clubLeaguesService.SaveToCsv(result);
      return File(new UTF8Encoding().GetBytes(csv), "text/csv", $"Document-{date}.csv");
    }
  }
}