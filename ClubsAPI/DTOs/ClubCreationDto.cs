using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClubsAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
  public class ClubCreationDto
  {
    public string ClubName { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public bool HasOwnStadium { get; set; }
    public DateTime ReleaseDate { get; set; }
    public IFormFile Poster { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
    public List<int> NationalitiesIds { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
    public List<int> ClubLeaguesIds { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder<List<ClubsPlayersCreationDto>>))]
    public List<ClubsPlayersCreationDto> Players { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder<List<ClubsCoachesCreationDto>>))]
    public List<ClubsCoachesCreationDto> Coaches { get; set; }
  }
}