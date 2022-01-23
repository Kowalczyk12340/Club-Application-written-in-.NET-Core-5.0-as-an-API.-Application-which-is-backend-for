using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Entities
{
    public class Club
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 75)]
        public string ClubName { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public bool HasOwnStadium { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }
        public List<ClubsNationalities> ClubsNationalities { get; set; }
        public List<ClubLeaguesClubs> ClubLeaguesClubs { get; set; }
        public List<ClubsPlayers> ClubsPlayers { get; set; }
        public List<ClubsCoaches> ClubsCoaches { get; set; }
    }
}