using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Entities
{
    public class ClubsNationalities
    {
        public int NationalityId { get; set; }
        public int ClubId { get; set; }
        public Nationality Nationality { get; set; }
        public Club Club { get; set; }
    }
}
