using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
  public class ClubDto
  {
    public int Id { get; set; }
    public string ClubName { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public bool HasOwnStadium { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Poster { get; set; }
    public double AverageVote { get; set; }
    public int UserVote { get; set; }
    public List<NationalityDto> Nationalities { get; set; }
    public List<ClubLeagueDto> ClubLeagues { get; set; }
    public List<PlayersClubDto> Players { get; set; }
    public List<CoachesClubDto> Coaches { get; set; }

    public string GetExportObject()
    {
      return $"{Id};{ClubName};{Summary};{Description};{HasOwnStadium};{ReleaseDate};{Poster};{AverageVote};{UserVote};{Nationalities.Count};{ClubLeagues.Count};{Players.Count};{Coaches.Count};";
    }
  }
}
