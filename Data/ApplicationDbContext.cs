using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using System;

namespace ProductAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //  Products ke liye DbSet
        public DbSet<Product> Products { get; set; }

        // Repositories ke liye DbSet
        public DbSet<Repository> Repositories { get; set; }

        //  Research & Publications ke liye DbSet
        public DbSet<ResearchPublication> ResearchPublications { get; set; }


        public DbSet<Research> Research { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        //  Seed data
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Repository>().HasData(
                new Repository
                {
                    Id = 1,
                    Title = "AI Free Repo",
                    Description = "Open source AI repository",
                    Type = "Free",
                    GitHubLink = "https://github.com/example/free-ai",
                    CreatedAt = new DateTime(2025, 01, 01) //  static date
                },
                new Repository
                {
                    Id = 2,
                    Title = "ML Premium Repo",
                    Description = "Premium ML project",
                    Type = "Premium",
                    GitHubLink = "https://github.com/example/premium-ml",
                    CreatedAt = new DateTime(2025, 01, 02) //  static date
                }
            );
        }
    }
}
