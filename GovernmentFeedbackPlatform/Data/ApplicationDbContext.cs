using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GovernmentFeedbackPlatform.Models;

namespace GovernmentFeedbackPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<NewsStory> NewsStories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category
            modelBuilder.Entity<Category>(e =>
            {
                e.HasIndex(c => c.Name).IsUnique();
            });

            // NewsStory
            modelBuilder.Entity<NewsStory>(e =>
            {
                e.HasOne(n => n.Category)
                    .WithMany(c => c.NewsStories)
                    .HasForeignKey(n => n.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(n => n.CreatedBy)
                    .WithMany()
                    .HasForeignKey(n => n.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasIndex(n => n.CategoryId);
                e.HasIndex(n => n.State);
                e.HasIndex(n => n.IsPublished);
                e.HasIndex(n => n.PublishedDate);
            });

            // Feedback
            modelBuilder.Entity<Feedback>(e =>
            {
                e.HasOne(f => f.User)
                    .WithMany(u => u.Feedbacks)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(f => f.NewsStory)
                    .WithMany(n => n.Feedbacks)
                    .HasForeignKey(f => f.NewsStoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Prevent duplicate feedback
                e.HasIndex(f => new { f.UserId, f.NewsStoryId }).IsUnique();

                e.HasIndex(f => f.NewsStoryId);
                e.HasIndex(f => f.CreatedAt);
            });

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Infrastructure", Description = "Roads, bridges, railways, airports and urban development", IconClass = "fas fa-road", ColorHex = "#2563EB" },
                new Category { Id = 2, Name = "Healthcare", Description = "Health schemes, hospitals, medical services", IconClass = "fas fa-heartbeat", ColorHex = "#DC2626" },
                new Category { Id = 3, Name = "Education", Description = "Schools, colleges, scholarships and literacy programs", IconClass = "fas fa-graduation-cap", ColorHex = "#7C3AED" },
                new Category { Id = 4, Name = "Agriculture", Description = "Farmer welfare, crop schemes, irrigation", IconClass = "fas fa-seedling", ColorHex = "#059669" },
                new Category { Id = 5, Name = "Economy", Description = "Budget, taxation, economic policies and GDP", IconClass = "fas fa-chart-line", ColorHex = "#D97706" },
                new Category { Id = 6, Name = "Defence", Description = "Armed forces, border security, defence procurement", IconClass = "fas fa-shield-alt", ColorHex = "#1F2937" },
                new Category { Id = 7, Name = "Environment", Description = "Climate, forests, pollution control, renewable energy", IconClass = "fas fa-leaf", ColorHex = "#10B981" },
                new Category { Id = 8, Name = "Social Welfare", Description = "Poverty alleviation, women empowerment, skill development", IconClass = "fas fa-hands-helping", ColorHex = "#F59E0B" },
                new Category { Id = 9, Name = "Technology", Description = "Digital India, cyber policy, space and science", IconClass = "fas fa-laptop-code", ColorHex = "#3B82F6" },
                new Category { Id = 10, Name = "Foreign Policy", Description = "International relations, trade agreements, diplomacy", IconClass = "fas fa-globe", ColorHex = "#6366F1" }
            );
        }
    }
}
