using Microsoft.EntityFrameworkCore;
using System;

namespace Perkjam.API.Entities
{
  public class PerkContext : DbContext
  {
    public PerkContext(DbContextOptions<PerkContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed the database with dummy data
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    Name = "Pete",
                    Email = "pete@gmail.com"
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
