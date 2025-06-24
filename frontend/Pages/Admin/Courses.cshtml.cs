using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using frontend.Data;
using frontend.Models;

namespace frontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CoursesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CoursesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AdminCoursViewModel> Cours { get; set; } = new();
        public List<EnseignantViewModel> Enseignants { get; set; } = new();
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public UpdateCourseInput? CourseToEdit { get; set; }

        public bool IsEditing { get; set; } = false;

        // GET
        public async Task OnGetAsync()
        {
            await LoadCoursesAsync();
        }

        // Formulaire d'édition
        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            await LoadEnseignantsAsync();

            var cours = await _context.Cours
                .Include(c => c.Enseignant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cours == null)
                return NotFound();

            CourseToEdit = new UpdateCourseInput
            {
                Id = cours.Id,
                Titre = cours.Titre,
                Description = cours.Description,
                EnseignantId = cours.EnseignantId
            };

            IsEditing = true;
            await LoadCoursesAsync();

            return Page();
        }

        // Mise à jour d'un cours
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            await LoadEnseignantsAsync();

            if (!ModelState.IsValid || CourseToEdit == null)
            {
                await LoadCoursesAsync();
                IsEditing = true;
                return Page();
            }

            var cours = await _context.Cours.FindAsync(CourseToEdit.Id);
            if (cours == null)
                return NotFound();

            cours.Titre = CourseToEdit.Titre;
            cours.Description = CourseToEdit.Description;
            cours.EnseignantId = CourseToEdit.EnseignantId;

            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ Cours mis à jour avec succès.";
            return RedirectToPage();
        }

        // Suppression
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var cours = await _context.Cours.FindAsync(id);
            if (cours == null)
                return NotFound();

            _context.Cours.Remove(cours);
            await _context.SaveChangesAsync();

            TempData["Message"] = "🗑️ Cours supprimé avec succès.";
            return RedirectToPage();
        }

        // Annulation
        public IActionResult OnPostCancelEdit()
        {
            return RedirectToPage();
        }

        // Charger cours + enseignants
        private async Task LoadCoursesAsync()
        {
            try
            {
                var coursList = await _context.Cours
                    .Include(c => c.Enseignant)
                    .Include(c => c.Inscriptions)
                        .ThenInclude(i => i.Etudiant)
                    .ToListAsync();

                Cours = coursList.Select(c => new AdminCoursViewModel
                {
                    Id = c.Id,
                    Titre = c.Titre,
                    Description = c.Description,
                    EnseignantId = c.EnseignantId,
                    EnseignantNom = $"{c.Enseignant?.Nom} {c.Enseignant?.Prenom}",
                    Etudiants = c.Inscriptions.Select(i => new EtudiantViewModel
                    {
                        Id = i.Etudiant.Id,
                        NomComplet = $"{i.Etudiant.Nom} {i.Etudiant.Prenom}",
                        Email = i.Etudiant.Email
                    }).ToList()
                }).ToList();

                await LoadEnseignantsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"❌ Erreur lors du chargement des cours : {ex.Message}";
            }
        }

        // Charger enseignants via la propriété Role personnalisée
        private async Task LoadEnseignantsAsync()
        {
            Enseignants = await _context.Users
                .Where(u => u.Role == "Enseignant")
                .Select(u => new EnseignantViewModel
                {
                    Id = u.Id,
                    NomComplet = $"{u.Nom} {u.Prenom}"
                })
                .ToListAsync();
        }

        // Models internes

        public class UpdateCourseInput
        {
            public int Id { get; set; }
            public string Titre { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string EnseignantId { get; set; } = string.Empty;
        }

        public class AdminCoursViewModel
        {
            public int Id { get; set; }
            public string Titre { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string EnseignantId { get; set; } = string.Empty;
            public string EnseignantNom { get; set; } = string.Empty;
            public List<EtudiantViewModel> Etudiants { get; set; } = new();
        }

        public class EtudiantViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string NomComplet { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        public class EnseignantViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string NomComplet { get; set; } = string.Empty;
        }
    }
}
