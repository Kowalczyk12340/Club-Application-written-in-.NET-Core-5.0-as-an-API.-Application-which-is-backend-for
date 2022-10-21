using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using ClubsAPI.Data;
using System.Threading.Tasks;
using ClubsAPI.Entities;

namespace ClubsAPI.Seeders
{
  public class ClubSeeder : IClubSeeder
  {
    private readonly ApplicationDataContext _dbContext;
    private readonly IPasswordHasher<IdentityUser> _passwordHasher;

    public ClubSeeder(ApplicationDataContext dbContext, IPasswordHasher<IdentityUser> passwordHasher)
    {
      _dbContext = dbContext;
      _passwordHasher = passwordHasher;
    }

    public async Task Seed()
    {
      if (_dbContext.Database.CanConnect())
      {
        if (!_dbContext.Nationalities.Any())
        {
          var nationalities = GetNationalities();
          _dbContext.Nationalities.AddRange(nationalities);
          await _dbContext.SaveChangesAsync();
        }
        if (!_dbContext.Clubs.Any())
        {
          var clubs = GetClubs();
          _dbContext.Clubs.AddRange(clubs);
          await _dbContext.SaveChangesAsync();
        }
        if (!_dbContext.Players.Any())
        {
          var players = GetPlayers();
          _dbContext.Players.AddRange(players);
          await _dbContext.SaveChangesAsync();
        }
        if (!_dbContext.Coach.Any())
        {
          var coaches = GetCoaches();
          _dbContext.Coach.AddRange(coaches);
          await _dbContext.SaveChangesAsync();
        }

        if (!_dbContext.ClubLeagues.Any())
        {
          var clubLeagues = GetClubLeagues();
          _dbContext.ClubLeagues.AddRange(clubLeagues);
          await _dbContext.SaveChangesAsync();
        }

        if (!_dbContext.Users.Any())
        {
          var users = GetUsers();
          _dbContext.Users.AddRange(users);
          await _dbContext.SaveChangesAsync();
        }
      }
    }

    private IEnumerable<Nationality> GetNationalities()
    {
      var nationalities = new List<Nationality>()
      {
        new Nationality
        {
          Name = "Słowenia",
        },
        new Nationality
        {
          Name = "Ghana",
        },
        new Nationality
        {
          Name = "Egipt",
        },
        new Nationality
        {
          Name = "San Marino",
        },
        new Nationality
        {
          Name = "Łotwa",
        },
      };

      return nationalities;
    }

    private IEnumerable<Club> GetClubs()
    {
      var nationalities = new List<Club>()
      {
        new Club
        {
          ClubName = "Lechia Gdańsk",
          Summary = "Klub Sportowy Lechia Gdańsk – polski klub" +
          " piłkarski z siedzibą w Gdańsku, zdobywca 2 Pucharów " +
          "Polski oraz 2 Superpucharów Polski, od sezonu 2008/2009" +
          " występujący w najwyższej polskiej piłkarskiej " +
          "klasie rozgrywkowej – Ekstraklasie.",
          Description = "Klub Sportowy Lechia Gdańsk – polski klub" +
          " piłkarski z siedzibą w Gdańsku, zdobywca 2 Pucharów " +
          "Polski oraz 2 Superpucharów Polski, od sezonu 2008/2009" +
          " występujący w najwyższej polskiej piłkarskiej " +
          "klasie rozgrywkowej – Ekstraklasie.",
          HasOwnStadium = true,
          ReleaseDate = new DateTime(1942,5,4,14,54,34),
        },
        new Club
        {
          ClubName = "Zagłębie Lubin",
          Summary = "KGHM Zagłębie Lubin – polski klub piłkarski z " +
          "siedzibą w Lubinie występujący w Ekstraklasie. Dwukrotny mistrz " +
          "Polski oraz zdobywca Superpucharu Polski w 2007. W Tabeli wszech" +
          " czasów Ekstraklasy Zagłębie zajmuje 10. miejsce. Sponsorem" +
          " tytularnym klubu jest KGHM Polska Miedź.",
          Description = "KGHM Zagłębie Lubin – polski klub piłkarski z " +
          "siedzibą w Lubinie występujący w Ekstraklasie. Dwukrotny mistrz " +
          "Polski oraz zdobywca Superpucharu Polski w 2007. W Tabeli wszech" +
          " czasów Ekstraklasy Zagłębie zajmuje 10. miejsce. Sponsorem" +
          " tytularnym klubu jest KGHM Polska Miedź.",
          HasOwnStadium = true,
          ReleaseDate = new DateTime(1935,7,24,11,43,12),
        },
      };

      return nationalities;
    }

    private IEnumerable<Player> GetPlayers()
    {
      var players = new List<Player>()
      {
        new Player
        {
          Name = "Sławomir Peszko",
          Biography = "Sławomir Peszko – polski piłkarz występujący na " +
          "pozycji pomocnika w polskim klubie Wieczysta Kraków. W latach 2008–2018" +
          " reprezentant Polski. Uczestnik Mistrzostw Europy 2016 oraz Mistrzostw" +
          " Świata 2018",
          DateOfBirth = new DateTime(1992,11,11,23,23,21),
        },
        new Player
        {
          Name = "Dominik Hładun",
          Biography = "Dominik Hładun – polski piłkarz występujący na " +
          "pozycji bramkarza w polskim klubie Zagłębie Lubin," +
          " którego jest wychowankiem. W trakcie swojej kariery grał także" +
          " w Chojniczance Chojnice. ",
          DateOfBirth = new DateTime(1995,2,1,12,27,29),
        },
      };

      return players;
    }

    private IEnumerable<Coach> GetCoaches()
    {
      var coaches = new List<Coach>()
      {
        new Coach
        {
          Name = "Sławomir Peszko",
          Biography = "Diego Pablo Simeone González – były argentyński piłkarz, " +
          "środkowy lub defensywny pomocnik. Od grudnia 2011 szkoleniowiec Atlético" +
          " Madryt. Jest ojcem Giovanniego Simeone.",
          DateOfBirth = new DateTime(1962,4,19,2,2,23),
        },
        new Coach
        {
          Name = "Franciszek Smuda",
          Biography = "Franciszek Smuda – polski piłkarz i trener piłkarski, " +
          "szkoleniowiec Wieczystej Kraków. Karierę piłkarską rozpoczynał w Unii" +
          " Racibórz i Odrze Wodzisław. Grał na pozycji obrońcy. Jako piłkarz " +
          "w barwach Stali Mielec i Legii Warszawa występował w I lidze.",
          DateOfBirth = new DateTime(1954,9,12,11,49,32),
        },
      };

      return coaches;
    }

    private IEnumerable<ClubLeague> GetClubLeagues()
    {
      var clubLeagues = new List<ClubLeague>()
      {
        new ClubLeague
        {
          Name = "English League One",
          Location = new NetTopologySuite.Geometries.Point(51.57,29.02),
        },
        new ClubLeague
        {
          Name = "English League Two",
          Location = new NetTopologySuite.Geometries.Point(50.57,29.13),
        },
      };

      return clubLeagues;
    }

    private IEnumerable<IdentityUser> GetUsers()
    {
      IdentityUser userAdded = new IdentityUser();
      userAdded.Email = "uzytkownik.testowy@wp.pl";
      userAdded.PasswordHash = _passwordHasher.HashPassword(userAdded, "Marcingrafik1#");
      userAdded.PhoneNumber = "+48 784 506 342";
      IdentityUser userAdded2 = new IdentityUser();
      userAdded2.Email = "uzytkownik2.testowy@wp.pl";
      userAdded2.PasswordHash = _passwordHasher.HashPassword(userAdded2, "Marcingrafik1#");
      userAdded2.PhoneNumber = "+48 603 455 111";
      IdentityUser userAdded3 = new IdentityUser();
      userAdded3.Email = "uzytkownik3.testowy@wp.pl";
      userAdded3.PasswordHash = _passwordHasher.HashPassword(userAdded3, "Marcingrafik1#");
      userAdded3.PhoneNumber = "+48 509 458 932";
      var users = new List<IdentityUser>()
      {
        userAdded,
        userAdded2,
        userAdded3
      };

      return users;
    }
  }
}