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
using System.Text;

namespace ClubsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
  public class NationalitiesController : ControllerBase
  {
    private readonly INationalitiesService _nationalitiesService;

    /// <summary>
    /// Constructor with Dependency Injection by nationalitiesService
    /// </summary>
    /// <param name="nationalitiesService"></param>
    public NationalitiesController(INationalitiesService nationalitiesService)
    {
      _nationalitiesService = nationalitiesService;
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
    [AllowAnonymous]
    public async Task<ActionResult<List<NationalityDto>>> Get()
    {
      var result = await _nationalitiesService.Get();
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
    [HttpGet("{Id:int}", Name = "getNationality")]
    public async Task<ActionResult<NationalityDto>> Get(int id)
    {
      var result = await _nationalitiesService.GetById(id);
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
    public async Task<ActionResult> Post([FromBody] NationalityCreationDto nationalityCreationDto)
    {
      await _nationalitiesService.Post(nationalityCreationDto);
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
    public async Task<ActionResult> Put(int id, [FromBody] NationalityCreationDto nationalityCreationDto)
    {
      await _nationalitiesService.Put(id, nationalityCreationDto);
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
      await _nationalitiesService.Delete(id);
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
      var result = await _nationalitiesService.Get();
      if (result == null)
      {
        return NotFound();
      }
      var csv = _nationalitiesService.SaveToCsv(result);
      return File(new UTF8Encoding().GetBytes(csv), "text/csv", $"Document-{date}.csv");
    }
  }
}