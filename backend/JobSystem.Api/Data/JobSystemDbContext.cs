using JobSystem.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JobSystem.Api.Data
{
    public class JobSystemDbContext : IdentityDbContext<ApplicationUser>
    {
        public JobSystemDbContext(DbContextOptions<JobSystemDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Job entity configuration
            builder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Company).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.Emirate).HasMaxLength(50);
                entity.Property(e => e.Currency).HasMaxLength(10);
                entity.Property(e => e.ExperienceLevel).HasMaxLength(50);
                entity.Property(e => e.JobType).HasMaxLength(50);
                entity.Property(e => e.Source).HasMaxLength(50);
                entity.Property(e => e.ExternalUrl).HasMaxLength(500);
                entity.Property(e => e.ExternalId).HasMaxLength(100);
                
                // JSON conversion for lists
                entity.Property(e => e.Technologies)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>());

                entity.Property(e => e.Benefits)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>());

                entity.HasIndex(e => e.Title);
                entity.HasIndex(e => e.Company);
                entity.HasIndex(e => e.Emirate);
                entity.HasIndex(e => e.PostedDate);
                entity.HasIndex(e => new { e.Source, e.ExternalId }).IsUnique();
            });

            // JobApplication entity configuration
            builder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ResumeFileName).HasMaxLength(255);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.JobApplications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Job)
                    .WithMany(j => j.Applications)
                    .HasForeignKey(e => e.JobId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.JobId }).IsUnique();
            });

            // UserProfile entity configuration
            builder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LinkedInUrl).HasMaxLength(200);
                entity.Property(e => e.GitHubUrl).HasMaxLength(200);
                entity.Property(e => e.PortfolioUrl).HasMaxLength(200);

                // JSON conversion for complex types
                entity.Property(e => e.Skills)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>());

                entity.Property(e => e.Experience)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<List<Experience>>(v, (JsonSerializerOptions)null!) ?? new List<Experience>());

                entity.Property(e => e.Education)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<List<Education>>(v, (JsonSerializerOptions)null!) ?? new List<Education>());

                entity.Property(e => e.Certifications)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>());

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Profile)
                    .HasForeignKey<UserProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId).IsUnique();
            });

            // ApplicationUser entity configuration
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
            });
        }
    }
}