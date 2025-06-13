using Microsoft.EntityFrameworkCore;
using CoursFeedback.API.Models;  

namespace CoursFeedback.API.Data  // ← point important : bien CoursFeedback.API.Data, pas CoursFeedbackAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<FeedbackSession> FeedbackSessions { get; set; }  // ← à l’intérieur de la classe

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FeedbackSession>()
                   .HasIndex(f => new { f.CourseId, f.IsActive })
                   .HasFilter("[IsActive] = 1")
                   .IsUnique();
        }
    }
}

