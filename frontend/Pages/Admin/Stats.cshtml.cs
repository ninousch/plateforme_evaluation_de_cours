using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using frontend.Data;
using frontend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class StatsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StatsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public AdminStatsDto Stats { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Statistiques globales
                Stats.TotalUtilisateurs = await _context.Users.CountAsync();
                Stats.TotalEtudiants = await _context.Users.CountAsync(u => u.Role == "Etudiant");
                Stats.TotalEnseignants = await _context.Users.CountAsync(u => u.Role == "Enseignant");
                Stats.TotalCours = await _context.Cours.CountAsync();
                Stats.TotalSessions = await _context.Sessions.CountAsync();
                Stats.TotalFeedbacks = await _context.Feedbacks.CountAsync();

                Stats.MoyenneGlobale = await _context.Feedbacks.AnyAsync()
                    ? await _context.Feedbacks.AverageAsync(f => f.Note)
                    : 0;

                // Statistiques par enseignant (moyenne de tous ses feedbacks sur tous ses cours)
                Stats.Enseignants = await _context.Users
                    .Where(u => u.Role == "Enseignant")
                    .Select(u => new EnseignantStatDto
                    {
                        EnseignantNom = u.Nom + " " + u.Prenom,
                        FeedbackCount = _context.Feedbacks.Count(f => f.Cours.EnseignantId == u.Id),
                        Moyenne = _context.Feedbacks.Where(f => f.Cours.EnseignantId == u.Id)
                                                   .Select(f => (double?)f.Note)
                                                   .Average() ?? 0
                    })
                    .Where(e => e.FeedbackCount > 0)
                    .ToListAsync();

                // Statistiques par cours avec leur enseignant et moyenne des notes
                Stats.CoursStats = await _context.Cours
                    .Include(c => c.Enseignant)
                    .Select(c => new CoursStatDto
                    {
                        TitreCours = c.Titre,
                        NomCompletEnseignant = c.Enseignant != null ? c.Enseignant.Nom + " " + c.Enseignant.Prenom : "Inconnu",
                        MoyenneNotes = c.Feedbacks.Average(f => (double?)f.Note) ?? 0,
                        NbFeedbacks = c.Feedbacks.Count()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des statistiques : {ex.Message}";
            }
        }

        // DTO principal
        public class AdminStatsDto
        {
            public int TotalUtilisateurs { get; set; }
            public int TotalEtudiants { get; set; }
            public int TotalEnseignants { get; set; }
            public int TotalCours { get; set; }
            public int TotalSessions { get; set; }
            public int TotalFeedbacks { get; set; }
            public double MoyenneGlobale { get; set; }
            public List<EnseignantStatDto> Enseignants { get; set; } = new();
            public List<CoursStatDto> CoursStats { get; set; } = new();
        }

        // DTO pour chaque enseignant
        public class EnseignantStatDto
        {
            public string EnseignantNom { get; set; } = string.Empty;
            public double Moyenne { get; set; }
            public int FeedbackCount { get; set; }
        }

        // DTO pour chaque cours
        public class CoursStatDto
        {
            public string TitreCours { get; set; } = string.Empty;
            public string NomCompletEnseignant { get; set; } = string.Empty;
            public double MoyenneNotes { get; set; }
            public int NbFeedbacks { get; set; }
        }
    }
}
