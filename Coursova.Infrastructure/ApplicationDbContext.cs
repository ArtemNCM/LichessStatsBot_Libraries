using Microsoft.EntityFrameworkCore;
using Coursova.Core.Models.Entities;

namespace Coursova.Infrastructure
{
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            { }

            public DbSet<PlayerInfo> PlayerInfos { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<PlayerInfo>()
                    .HasIndex(p => p.Username)
                    .IsUnique();
            }
        }
}
