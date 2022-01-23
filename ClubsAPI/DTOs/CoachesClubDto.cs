using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.DTOs
{
  public class CoachesClubDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }
    public string License { get; set; }
    public int Order { get; set; }
  }
}
