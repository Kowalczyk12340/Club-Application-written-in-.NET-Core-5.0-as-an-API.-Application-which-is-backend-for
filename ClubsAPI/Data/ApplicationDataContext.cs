using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClubsAPI.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ClubsAPI.Data
{
  public class ApplicationDataContext : IdentityDbContext
  {
    public ApplicationDataContext([NotNull] DbContextOptions options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.Entity<ClubsPlayers>()
          .HasKey(x => new { x.PlayerId, x.ClubId });

      builder.Entity<ClubsCoaches>()
      .HasKey(x => new { x.CoachId, x.ClubId });

      builder.Entity<ClubsNationalities>()
                .HasKey(x => new { x.NationalityId, x.ClubId });

      builder.Entity<ClubLeaguesClubs>()
          .HasKey(x => new { x.ClubLeagueId, x.ClubId });

      base.OnModelCreating(builder);
    }

    public DbSet<Nationality> Nationalities { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Coach> Coach { get; set; }
    public DbSet<ClubLeague> ClubLeagues { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<ClubsPlayers> ClubsPlayers { get; set; }
    public DbSet<ClubsNationalities> ClubsNationalities { get; set; }
    public DbSet<ClubLeaguesClubs> ClubLeaguesClubs { get; set; }
    public DbSet<Rating> Ratings { get; set; }
  }
}
