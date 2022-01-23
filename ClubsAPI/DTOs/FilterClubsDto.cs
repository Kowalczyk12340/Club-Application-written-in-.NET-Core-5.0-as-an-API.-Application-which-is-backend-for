using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
    public class FilterClubsDto
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationDto PaginationDto 
        {
            get { return new PaginationDto() { Page = Page, RecordsPerPage = RecordsPerPage }; }
        }
        public string ClubName { get; set; }
        public int NationalityId { get; set; }
        public bool HasOwnStadium { get; set; }
        public bool UpcomingReleases { get; set; }
    }
}
