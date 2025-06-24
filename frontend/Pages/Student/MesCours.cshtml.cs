using Microsoft.AspNetCore.Mvc.RazorPages;
using frontend.Data;
using frontend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace frontend.Pages.Student
{
    public class MesCoursModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MesCoursModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CoursDto> CoursList { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Gérer les erreurs éventuelles si l'étudiant n’est pas connecté
                return;
            }

            // Récupérer tous les cours où l'étudiant est inscrit
            var coursInscrits = await _context.Inscriptions
                .Where(i => i.EtudiantId == userId)
                .Include(i => i.Cours)  // Inclure les Cours
                .ThenInclude(c => c.Enseignant) // Inclure l'Enseignant du cours
                .Select(i => i.Cours) // Sélectionner le cours uniquement
                .ToListAsync();

            // Construction de la liste DTO
            CoursList = coursInscrits.Select(c => new CoursDto
            {
                Id = c.Id,
                Titre = c.Titre,
                Description = c.Description,
                DatePublication = c.DatePublication,
                EnseignantNomComplet = c.Enseignant != null
                    ? $"{c.Enseignant.Nom} {c.Enseignant.Prenom}"
                    : "Inconnu",

                Note = _context.Feedbacks
                    .Where(f => f.CoursId == c.Id && f.EtudiantId == userId)
                    .Select(f => (int?)f.Note)
                    .FirstOrDefault(),

                Commentaire = _context.Feedbacks
                    .Where(f => f.CoursId == c.Id && f.EtudiantId == userId)
                    .Select(f => f.Commentaire)
                    .FirstOrDefault()

            }).ToList();
        }
    }

    public class CoursDto
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DatePublication { get; set; }
        public string EnseignantNomComplet { get; set; } = string.Empty;
        public int? Note { get; set; }
        public string? Commentaire { get; set; }
    }
}
