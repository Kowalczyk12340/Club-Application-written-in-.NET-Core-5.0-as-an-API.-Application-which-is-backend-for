using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
    public class ClubPostGetDto
    {
        public List<NationalityDto> Nationalities { get; set; }
        public List<ClubLeagueDto> ClubLeagues { get; set; }
    }
}
