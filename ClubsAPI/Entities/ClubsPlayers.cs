using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Entities
{
    public class ClubsPlayers
    {
        public int PlayerId { get; set; }
        public int ClubId { get; set; }
        [StringLength(maximumLength: 75)]
        public string Position { get; set; }
        public int Order { get; set; }
        public Player Player { get; set; }
        public Club Club { get; set; }
    }
}
