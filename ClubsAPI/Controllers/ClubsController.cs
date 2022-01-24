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
using System.Text;

namespace ClubsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class ClubsController : ControllerBase
  {
    private readonly IClubsService _clubsService;

    /// <summary>
    /// Constructor with Dependency Injection for clubService
    /// </summary>
    /// <param name="clubsService"></param>
    public ClubsController(IClubsService clubsService)
    {
      _clubsService = clubsService;
    }

    /// <summary>
    /// Method to add with some parameters an object to the leagues
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Ok if it is added successfully or 500 if any error exists by mapping or validation parameters</returns>
    /// <response code="200">League with object is successfully added</response>
    /// <response code="400">An Error occurred by trying to add league</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<HomeDto>> Get()
    {
      var result = await _clubsService.Get();
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ClubDto>> Get(int id)
    {
      var result = await _clubsService.Get(id);
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("filter")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ClubDto>>> Filter([FromQuery] FilterClubsDto filterClubsDto)
    {
      var result = await _clubsService.Filter(filterClubsDto);
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("PostGet")]
    public async Task<ActionResult<ClubPostGetDto>> PostGet()
    {
      var result = await _clubsService.PostGet();
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<ActionResult<int>> Post([FromForm] ClubCreationDto clubCreationDto)
    {
      var result = await _clubsService.Post(clubCreationDto);
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("putget/{id:int}")]
    public async Task<ActionResult<ClubPutGetDto>> PutGet(int id)
    {
      var result = await _clubsService.PutGet(id);
      return Ok(result);
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
    public async Task<ActionResult> Put(int id, [FromForm] ClubCreationDto clubCreationDto)
    {
      await _clubsService.Put(id, clubCreationDto);
      return Ok();
    }

    /// <summary>
    /// Method to delete chosen club league by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoContent if it is deleted successfully or 404 if not found</returns>
    /// <response code="204">League with chosen id exists and has been successfully deleted</response>
    /// <response code="404">League with chosen id does not exist</response>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      await _clubsService.Delete(id);
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
    public async Task<IActionResult> SaveToCsv(FilterClubsDto dto)
    {
      var date = DateTime.UtcNow;
      var result = await _clubsService.Filter(dto);
      if (result == null)
      {
        return NotFound();
      }
      var csv = _clubsService.SaveToCsv(result);
      return File(new UTF8Encoding().GetBytes(csv), "text/csv", $"Document-{date}.csv");
    }
  }
}