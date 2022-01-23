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

namespace ClubsAPI.Services
{
  public class RatingsService : IRatingsService
  {
    private readonly ApplicationDataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RatingsService(ApplicationDataContext context,
        UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _userManager = userManager;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task Post([FromBody] RatingDto ratingDto)
    {
      var email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
      var user = await _userManager.FindByEmailAsync(email);
      var userId = user.Id;

      var currentRate = await _context.Ratings
          .FirstOrDefaultAsync(x => x.ClubId == ratingDto.ClubId &&
          x.UserId == userId);

      if (currentRate == null)
      {
        var rating = new Rating();
        rating.ClubId = ratingDto.ClubId;
        rating.Rate = ratingDto.Rating;
        rating.UserId = userId;
        _context.Add(rating);
      }
      else
      {
        currentRate.Rate = ratingDto.Rating;
      }

      await _context.SaveChangesAsync();
    }
  }
}