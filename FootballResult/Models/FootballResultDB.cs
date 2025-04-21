using FootballResult.Models.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootballResult.Models
{
    public class FootballResultDB : IdentityDbContext<User>
    {
        public FootballResultDB(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Stats_Teams>()
            .HasKey(ucb => new { ucb.TeamFId, ucb.StatsFId });

            builder.Entity<Stats_Teams>()
            .HasOne(st => st.Team)
            .WithMany(t => t.Stats_Teams)
            .HasForeignKey(st => st.TeamFId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Stats_Teams>()
            .HasOne(st => st.Stats)
            .WithMany(s => s.Stats_Teams)
            .HasForeignKey(st => st.StatsFId)
            .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<FootballResult.Models.Data.Stats> Stats { get; set; } = default!;
        public DbSet<FootballResult.Models.Data.Team> Team { get; set; } = default!;
        public DbSet<FootballResult.Models.Data.Stats_Teams> Stats_Teams { get; set; } = default!;
    }
}
