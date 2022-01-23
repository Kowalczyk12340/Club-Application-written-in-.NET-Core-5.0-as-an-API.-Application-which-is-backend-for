using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
    public class HomeDto
    {
        public List<ClubDto> HasOwnStadium { get; set; }
        public List<ClubDto> UpcomingReleases { get; set; }
    }
}