using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ClubsAPI.Data;
using ClubsAPI.DTOs;
using ClubsAPI.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace ClubsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountsController : ControllerBase
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDataContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor with Dependency Injection
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="configuration"></param>
    /// <param name="context"></param>
    /// <param name="mapper"></param>
    public AccountsController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        ApplicationDataContext context,
        IMapper mapper)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _configuration = configuration;
      _context = context;
      _mapper = mapper;
    }

    /// <summary>
    /// Method to get all the users
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Return Ok if it is successfully display users or 404 if users were not found</returns>
    /// <response code="200">User exists and have been successfully displayed</response>
    /// <response code="404">Users do not exist</response>
    [ProducesResponseType(typeof(List<ClubLeagueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("listUsers")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public async Task<ActionResult<List<UserDto>>> GetListUsers([FromQuery] PaginationDto paginationDto)
    {
      var queryable = _context.Users.AsQueryable();
      await HttpContext.InsertParametersPaginationInHeader(queryable);
      var users = await queryable.OrderBy(x => x.Email).Paginate(paginationDto).ToListAsync();
      return _mapper.Map<List<UserDto>>(users);
    }

    /// <summary>
    /// Method to add admin status for user with chosen id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Return NoContent if it is successfully changed or 404 if users were not found</returns>
    /// <response code="204">User exist and hs been successfully change status for admin</response>
    /// <response code="404">Admin was not exist</response>
    [ProducesResponseType(typeof(List<ClubLeagueDto>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPost("makeAdmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public async Task<ActionResult> MakeAdmin([FromBody] string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      await _userManager.AddClaimAsync(user, new Claim("role", "admin"));
      return NoContent();
    }

    /// <summary>
    /// Method to remove admin status for user with chosen id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Return NoContent if it is successfully deleted admin status
    /// user or 400 is status admin is not done</returns>
    /// <response code="204">User have been changed status with admin to user</response>
    /// <response code="400">Leagues do not exist</response>
    [ProducesResponseType(typeof(List<ClubLeagueDto>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost("removeAdmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public async Task<ActionResult> RemoveAdmin([FromBody] string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      await _userManager.RemoveClaimAsync(user, new Claim("role", "admin"));
      return NoContent();
    }

    /// <summary>
    /// Method to create user in this website
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Return NoContent with token if it is successfully added user 
    /// or 400 if user was not added to the database</returns>
    /// <response code="204">User is successfully added to the database</response>
    /// <response code="400">User has bad request and he/she is not created</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost("create")]
    public async Task<ActionResult<AuthenticationResponse>> Create(
        [FromBody] UserCredentials userCredentials)
    {
      var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
      var result = await _userManager.CreateAsync(user, userCredentials.Password);

      if (result.Succeeded)
      {
        return await BuildToken(userCredentials);
      }
      else
      {
        return BadRequest(result.Errors);
      }
    }

    /// <summary>
    /// Method to login by bearer token
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Return NoContent with token if login is successfully or
    /// 400 if it is Bad Request and logging in was not successfully</returns>
    /// <response code="204">User is logged in</response>
    /// <response code="400">User has bad request and he/she is not logged</response>
    [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        [FromBody] UserCredentials userCredentials)
    {
      var result = await _signInManager.PasswordSignInAsync(userCredentials.Email,
          userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

      if (result.Succeeded)
      {
        return await BuildToken(userCredentials);
      }
      else
      {
        return BadRequest("Incorrect Login");
      }
    }

    /// <summary>
    /// Method to build token for registration and login
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Return token for logged and registered user</returns>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials)
    {
      var claims = new List<Claim>()
      {
        new Claim("email", userCredentials.Email)
      };

      var user = await _userManager.FindByNameAsync(userCredentials.Email);
      var claimsDB = await _userManager.GetClaimsAsync(user);

      claims.AddRange(claimsDB);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var expiration = DateTime.UtcNow.AddYears(1);

      var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
          expires: expiration, signingCredentials: creds);

      return new AuthenticationResponse()
      {
        Token = new JwtSecurityTokenHandler().WriteToken(token),
        Expiration = expiration
      };
    }
  }
}