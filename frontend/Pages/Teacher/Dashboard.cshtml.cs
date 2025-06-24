using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using frontend.Data;
using frontend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace frontend.Pages.Teacher
{
    [Authorize(Roles = "Enseignant")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Injection des dépendances
        public DashboardModel(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public List<CoursStatViewModel> CoursStats { get; set; } = new();
        public Dictionary<string, int> TopWords { get; set; } = new();

        // Action pour déconnexion
        public async Task<IActionResult> OnPostLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");  // Redirige vers la page d'accueil après déconnexion
        }

        // Action pour charger les statistiques
        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return;

            // Récupérer les cours de l'enseignant avec leurs feedbacks
            var cours = await _context.Cours
                .Where(c => c.EnseignantId == userId)
                .Include(c => c.Feedbacks)
                .ToListAsync();

            // Préparer les statistiques
            CoursStats = cours.Select(c => new CoursStatViewModel
            {
                Titre = c.Titre,
                MoyenneNote = c.Feedbacks.Any() ? c.Feedbacks.Average(f => f.Note) : 0.0,
                Commentaires = c.Feedbacks.Select(f => f.Commentaire ?? "").ToList()
            }).ToList();

            // Extraction des mots les plus fréquents
            ExtractTopWords();
        }

        // Méthode pour extraire les mots les plus fréquents des commentaires
        private void ExtractTopWords()
        {
            var allWords = CoursStats.SelectMany(c => c.Commentaires)
                .SelectMany(c => c.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(w => w.ToLowerInvariant())
                .Where(w => w.Length > 3 && !StopWords.Contains(w)); // Enlever les "stop words"

            TopWords = allWords
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(30) // Prendre les 30 mots les plus fréquents
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Liste des mots communs à ignorer dans les commentaires
        private static readonly HashSet<string> StopWords = new()
        {
            "avec", "pour", "dans", "avoir", "vous", "cela", "nous", "mais",
            "tout", "tous", "plus", "être", "sans", "cette", "comme"
        };

        // ViewModel pour les statistiques de chaque cours
        public class CoursStatViewModel
        {
            public string Titre { get; set; }
            public double MoyenneNote { get; set; }
            public List<string> Commentaires { get; set; } = new();
        }
    }
}
