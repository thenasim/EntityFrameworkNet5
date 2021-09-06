using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkNet5.Data.Configurations.Entities
{
    internal class CoachConfiguration : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            builder.Property(m => m.Name).HasMaxLength(50);
            builder.HasIndex(p => new { p.Name, p.TeamId }).IsUnique();


            builder.HasData(
                    new Coach
                    {
                        Id = 29,
                        Name = "Seed Coach - 1",
                        TeamId = 30
                    },
                    new Coach
                    {
                        Id = 30,
                        Name = "Seed Coach - 2",
                        TeamId = 31
                    },
                    new Coach
                    {
                        Id = 31,
                        Name = "Seed Coach - 3",
                        TeamId = 32
                    }
                );
        }
    }
}