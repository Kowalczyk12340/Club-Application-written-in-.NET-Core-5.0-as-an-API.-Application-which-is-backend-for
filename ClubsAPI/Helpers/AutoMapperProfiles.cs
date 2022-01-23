using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ClubsAPI.DTOs;
using ClubsAPI.Entities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Helpers
{
  public class AutoMapperProfiles : Profile
  {
    public AutoMapperProfiles(GeometryFactory geometryFactory)
    {
      CreateMap<NationalityDto, Nationality>().ReverseMap();
      CreateMap<NationalityCreationDto, Nationality>().ReverseMap();

      CreateMap<PlayerDto, Player>().ReverseMap();
      CreateMap<PlayerCreationDto, Player>()
          .ForMember(x => x.Picture, options => options.Ignore());

      CreateMap<CoachDto, Coach>().ReverseMap();
      CreateMap<CoachCreationDto, Coach>()
          .ForMember(x => x.Picture, options => options.Ignore());

      CreateMap<ClubLeague, ClubLeagueDto>()
          .ForMember(x => x.Latitude, dto => dto.MapFrom(prop => prop.Location.Y))
          .ForMember(x => x.Longitude, dto => dto.MapFrom(prop => prop.Location.X));

      CreateMap<ClubLeagueCreationDto, ClubLeague>()
          .ForMember(x => x.Location, x => x.MapFrom(dto =>
          geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude))));

      CreateMap<ClubCreationDto, Club>()
          .ForMember(x => x.Poster, options => options.Ignore())
          .ForMember(x => x.ClubsNationalities, options => options.MapFrom(MapClubsNationalities))
          .ForMember(x => x.ClubLeaguesClubs, options => options.MapFrom(MapClubLeaguesClubs))
          .ForMember(x => x.ClubsPlayers, options => options.MapFrom(MapClubsPlayers))
          .ForMember(x => x.ClubsCoaches, options => options.MapFrom(MapClubsCoaches));

      CreateMap<Club, ClubDto>()
          .ForMember(x => x.Nationalities, options => options.MapFrom(MapClubsNationalities))
          .ForMember(x => x.ClubLeagues, options => options.MapFrom(MapClubLeaguesClubs))
          .ForMember(x => x.Players, options => options.MapFrom(MapClubsPlayers))
          .ForMember(x => x.Coaches, options => options.MapFrom(MapClubsCoaches));

      CreateMap<IdentityUser, UserDto>();
    }

    private List<CoachesClubDto> MapClubsCoaches(Club club, ClubDto clubDto)
    {
      var result = new List<CoachesClubDto>();

      if (club.ClubsCoaches != null)
      {
        foreach (var clubsCoaches in club.ClubsCoaches)
        {
          result.Add(new CoachesClubDto()
          {
            Id = clubsCoaches.CoachId,
            Name = clubsCoaches.Coach.Name,
            License = clubsCoaches.License,
            Picture = clubsCoaches.Coach.Picture,
            Order = clubsCoaches.Order
          });
        }
      }

      return result;
    }

    private List<PlayersClubDto> MapClubsPlayers(Club club, ClubDto clubDto)
    {
      var result = new List<PlayersClubDto>();

      if (club.ClubsPlayers != null)
      {
        foreach (var clubsPlayers in club.ClubsPlayers)
        {
          result.Add(new PlayersClubDto()
          {
            Id = clubsPlayers.PlayerId,
            Name = clubsPlayers.Player.Name,
            Position = clubsPlayers.Position,
            Picture = clubsPlayers.Player.Picture,
            Order = clubsPlayers.Order
          });
        }
      }

      return result;
    }

    private List<ClubLeagueDto> MapClubLeaguesClubs(Club club, ClubDto clubDto)
    {
      var result = new List<ClubLeagueDto>();

      if (club.ClubLeaguesClubs != null)
      {
        foreach (var clubLeaguesClubs in club.ClubLeaguesClubs)
        {
          result.Add(new ClubLeagueDto()
          {
            Id = clubLeaguesClubs.ClubLeagueId,
            Name = clubLeaguesClubs.ClubLeague.Name,
            Latitude = clubLeaguesClubs.ClubLeague.Location.Y,
            Longitude = clubLeaguesClubs.ClubLeague.Location.X
          });
        }
      }
      return result;
    }

    private List<NationalityDto> MapClubsNationalities(Club club, ClubDto clubDto)
    {
      var result = new List<NationalityDto>();

      if (club.ClubsNationalities != null)
      {
        foreach (var nationality in club.ClubsNationalities)
        {
          result.Add(new NationalityDto() { Id = nationality.NationalityId, Name = nationality.Nationality.Name });
        }
      }

      return result;
    }

    private List<ClubsNationalities> MapClubsNationalities(ClubCreationDto clubCreationDto, Club club)
    {
      var result = new List<ClubsNationalities>();

      if (clubCreationDto.NationalitiesIds == null) { return result; }

      foreach (var id in clubCreationDto.NationalitiesIds)
      {
        result.Add(new ClubsNationalities() { NationalityId = id });
      }

      return result;
    }

    private List<ClubLeaguesClubs> MapClubLeaguesClubs(ClubCreationDto clubCreationDto,
        Club club)
    {
      var result = new List<ClubLeaguesClubs>();

      if (clubCreationDto.ClubLeaguesIds == null) { return result; }

      foreach (var id in clubCreationDto.ClubLeaguesIds)
      {
        result.Add(new ClubLeaguesClubs() { ClubLeagueId = id });
      }

      return result;
    }

    private List<ClubsPlayers> MapClubsPlayers(ClubCreationDto clubCreationDto, Club club)
    {
      var result = new List<ClubsPlayers>();

      if (clubCreationDto.Players == null) { return result; }

      foreach (var player in clubCreationDto.Players)
      {
        result.Add(new ClubsPlayers() { PlayerId = player.Id, Position = player.Position });
      }

      return result;
    }

    private List<ClubsCoaches> MapClubsCoaches(ClubCreationDto clubCreationDto, Club club)
    {
      var result = new List<ClubsCoaches>();

      if (clubCreationDto.Coaches == null) { return result; }

      foreach (var coach in clubCreationDto.Coaches)
      {
        result.Add(new ClubsCoaches() { CoachId = coach.Id, License = coach.License });
      }

      return result;
    }
  }
}