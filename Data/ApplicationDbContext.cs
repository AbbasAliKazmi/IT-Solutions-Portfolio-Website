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
                    Title = "Research Downloader",
                    Description = "Secure download access system",
                    Type = "Free",
                    GitHubURL = "https://github.com/YourName/ResearchDownloader",
                    License = "MIT",
                    Version = "1.0.0",
                    Domain = "AI",
                    PreviewURL = "https://example.com/preview",
                    IsPremium = false
                },
                new Repository
                {
                    Id = 2,
                    Title = "Research Downloader",
                    Description = "Secure download access system",
                    Type = "Free",
                    GitHubURL = "https://github.com/YourName/ResearchDownloader",
                    License = "MIT",
                    Version = "1.0.0",
                    Domain = "AI",
                    PreviewURL = "https://example.com/preview",
                    IsPremium = false
                }
            );
        }
    }
}
