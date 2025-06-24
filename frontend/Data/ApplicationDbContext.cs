using frontend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace frontend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cours> Cours { get; set; }
        public DbSet<Inscription> Inscriptions { get; set; }
        public DbSet<SessionFeedback> SessionsFeedback { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurations spécifiques si besoin :
            builder.Entity<Cours>()
                .HasOne(c => c.Enseignant)
                .WithMany()
                .HasForeignKey(c => c.EnseignantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Inscription>()
                .HasOne(i => i.Etudiant)
                .WithMany()
                .HasForeignKey(i => i.EtudiantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
