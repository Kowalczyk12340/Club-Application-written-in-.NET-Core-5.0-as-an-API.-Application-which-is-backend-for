using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
    public class ClubPutGetDto
    {
        public ClubDto Club { get; set; }
        public List<NationalityDto> SelectedNationalities { get; set; }
        public List<NationalityDto> NonSelectedNationalities { get; set; }
        public List<ClubLeagueDto> SelectedClubLeagues { get; set; }
        public List<ClubLeagueDto> NonSelectedClubLeagues { get; set; }
        public List<PlayersClubDto> Players { get; set; }
        public List<CoachesClubDto> Coaches { get; set; }
    }
}
