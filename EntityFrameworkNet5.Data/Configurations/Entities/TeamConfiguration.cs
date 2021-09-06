using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkNet5.Data.Configurations.Entities
{
    internal class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder
                .HasMany(e => e.HomeMatches)
                .WithOne(e => e.HomeTeam)
                .HasForeignKey(e => e.HomeTeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(e => e.AwayMatches)
                .WithOne(e => e.AwayTeam)
                .HasForeignKey(e => e.AwayTeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(m => m.Name).HasMaxLength(50);
            builder.HasIndex(p => p.Name).IsUnique();


            builder.HasData(
                    new Team
                    {
                        Id = 30,
                        Name = "Seed Team - 1",
                        LeagueId = 2,
                    },
                    new Team
                    {
                        Id = 31,
                        Name = "Seed Team - 2",
                        LeagueId = 2,
                    },
                    new Team
                    {
                        Id = 32,
                        Name = "Seed Team - 3",
                        LeagueId = 2,
                    }
                );
        }
    }
}