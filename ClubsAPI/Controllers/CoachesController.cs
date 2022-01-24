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
using System.Text;

namespace ClubsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class CoachesController : ControllerBase
  {
    private readonly ICoachesService _coachesService;

    /// <summary>
    /// Constructor with Dependency Injection with coachService
    /// </summary>
    /// <param name="coachesService"></param>
    public CoachesController(ICoachesService coachesService)
    {
      _coachesService = coachesService;
    }

    /// <summary>
    /// Method to delete chosen club league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoContent if it is deleted successfully or 404 if not found</returns>
    /// <response code="204">League with chosen id exists and has been successfully deleted</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ActionResult<List<CoachDto>>> Get([FromQuery] PaginationDto paginationDto)
    {
      var result = await _coachesService.Get(paginationDto);
      return Ok(result);
    }

    /// <summary>
    /// Method to delete chosen club league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoContent if it is deleted successfully or 404 if not found</returns>
    /// <response code="204">League with chosen id exists and has been successfully deleted</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPost("searchByName")]
    public async Task<ActionResult<List<CoachesClubDto>>> SearchByName([FromBody] string name)
    {
      var result = await _coachesService.SearchByName(name);
      return Ok(result);
    }

    /// <summary>
    /// Method to delete chosen club league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoContent if it is deleted successfully or 404 if not found</returns>
    /// <response code="204">League with chosen id exists and has been successfully deleted</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CoachDto>> Get(int id)
    {
      var result = await _coachesService.Get(id);
      return Ok(result);
    }

    /// <summary>
    /// Method to delete chosen club league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoContent if it is deleted successfully or 404 if not found</returns>
    /// <response code="204">League with chosen id exists and has been successfully deleted</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<ActionResult> Post([FromForm] CoachCreationDto coachCreationDto)
    {
      await _coachesService.Post(coachCreationDto);
      return Ok();
    }

    /// <summary>
    /// Method to delete chosen club league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoContent if it is deleted successfully or 404 if not found</returns>
    /// <response code="204">League with chosen id exists and has been successfully deleted</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] CoachCreationDto coachCreationDto)
    {
      await _coachesService.Put(id, coachCreationDto);
      return Ok();
    }

    /// <summary>
    /// Method to delete chosen club league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoContent if it is deleted successfully or 404 if not found</returns>
    /// <response code="204">League with chosen id exists and has been successfully deleted</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      await _coachesService.Delete(id);
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
    public async Task<IActionResult> SaveToCsv(PaginationDto dto)
    {
      var date = DateTime.UtcNow;
      var result = await _coachesService.Get(dto);
      if (result == null)
      {
        return NotFound();
      }
      var csv = _coachesService.SaveToCsv(result);
      return File(new UTF8Encoding().GetBytes(csv), "text/csv", $"Document-{date}.csv");
    }
  }
}