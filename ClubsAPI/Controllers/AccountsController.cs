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

    [HttpGet("listUsers")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public async Task<ActionResult<List<UserDto>>> GetListUsers([FromQuery] PaginationDto paginationDto)
    {
      var queryable = _context.Users.AsQueryable();
      await HttpContext.InsertParametersPaginationInHeader(queryable);
      var users = await queryable.OrderBy(x => x.Email).Paginate(paginationDto).ToListAsync();
      return _mapper.Map<List<UserDto>>(users);
    }

    [HttpPost("makeAdmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public async Task<ActionResult> MakeAdmin([FromBody] string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      await _userManager.AddClaimAsync(user, new Claim("role", "admin"));
      return NoContent();
    }

    [HttpPost("removeAdmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public async Task<ActionResult> RemoveAdmin([FromBody] string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      await _userManager.RemoveClaimAsync(user, new Claim("role", "admin"));
      return NoContent();
    }

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