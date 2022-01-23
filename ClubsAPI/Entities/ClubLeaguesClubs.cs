using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Entities
{
    public class ClubLeaguesClubs
    {
        public int ClubLeagueId { get; set; }
        public int ClubId { get; set; }
        public ClubLeague ClubLeague { get; set; }
        public Club Club { get; set; }
    }
}
