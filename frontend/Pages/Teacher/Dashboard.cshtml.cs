using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using frontend.Data;
using frontend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace frontend.Pages.Teacher
{
    [Authorize(Roles = "Enseignant")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DashboardModel(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public List<CoursStatViewModel> CoursStats { get; set; } = new();
        public Dictionary<string, int> TopWords { get; set; } = new();

        public async Task<IActionResult> OnPostLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return;

            // Récupère les cours de l'enseignant et leurs feedbacks
            var cours = await _context.Cours
                .Where(c => c.EnseignantId == userId)
                .Include(c => c.Feedbacks)
                .ToListAsync();

            // Statistiques
            CoursStats = cours.Select(c => new CoursStatViewModel
            {
                Titre = c.Titre,
                MoyenneNote = c.Feedbacks.Any() ? c.Feedbacks.Average(f => f.Note) : 0.0,
                Commentaires = c.Feedbacks
                    .Where(f => !string.IsNullOrWhiteSpace(f.Commentaire))
                    .Select(f => f.Commentaire!)
                    .ToList()
            }).ToList();

            ExtractTopWords();
        }

        private void ExtractTopWords()
        {
            var allWords = CoursStats
                .SelectMany(c => c.Commentaires)
                .SelectMany(comment =>
                    Regex.Matches(comment.ToLower(), @"\b[\p{L}'’-]+\b")
                        .Select(m => m.Value)
                )
                .Where(w => w.Length > 3 && !StopWords.Contains(w));

            TopWords = allWords
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(30)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private static readonly HashSet<string> StopWords = new()
        {
    "carrément", "pur", "génie", "ça", "va", "débrouille", "moyennement", "bof", "galère",
    "plane", "complètement", "clair", "yeux", "fermés", "assez", "faut", "juste", "suivre",
    "souvent", "confus", "charabia", "total", "top", "logique", "plutôt", "bazar", "brouillon",
    "chaos", "absolu", "super", "intéressant", "accro", "généralement", "captivant",
    "dépend", "jours", "ennuyeux", "invitation", "dodo", "explications", "nickel",
    "pige", "rien", "mystère", "ultra", "juste", "doutes", "parfois", "soupçons", "totalement",
    "dispo", "cool", "facilement", "joignable", "traquer", "fantôme", "respectueux",
    "correct", "limite", "condescendant", "débiles", "hyper", "utiles", "pratiques",
    "trucs", "éléments", "pas", "grand-chose", "merci", "prof", "document", "note",
    "générale", "commentaire", "administration", "ressenti", "filtre", "soumission"
};

        public class CoursStatViewModel
        {
            public string Titre { get; set; } = string.Empty;
            public double MoyenneNote { get; set; }
            public List<string> Commentaires { get; set; } = new();
        }
    }
}
