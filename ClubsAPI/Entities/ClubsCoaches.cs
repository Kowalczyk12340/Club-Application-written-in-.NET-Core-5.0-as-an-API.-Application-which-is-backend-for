using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Entities
{
  public class ClubsCoaches
  {
    public int CoachId { get; set; }
    public int ClubId { get; set; }
    [StringLength(maximumLength: 75)]
    public string License { get; set; }
    public int Order { get; set; }
    public Coach Coach { get; set; }
    public Club Club { get; set; }
  }
}
