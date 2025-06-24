using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using frontend.Data;
using frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace frontend.Pages.Admin
{
    public class ManageInscriptionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageInscriptionsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Propriétés pour les listes à afficher
        public List<Cours> TousLesCours { get; set; } = new();
        public List<ApplicationUser> Etudiants { get; set; } = new();

        // ✅ Pour les messages d'erreur ou de succès
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        // ✅ Propriétés de formulaire
        [BindProperty]
        public int CoursId { get; set; }

        [BindProperty]
        public string EtudiantId { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await ChargerDonneesAsync();
        }

        public async Task<IActionResult> OnPostInscrireAsync()
        {
            if (string.IsNullOrWhiteSpace(EtudiantId) || CoursId == 0)
            {
                ErrorMessage = "Veuillez sélectionner un étudiant et un cours.";
                await ChargerDonneesAsync();
                return Page();
            }

            var existe = await _context.Inscriptions
                .AnyAsync(i => i.CoursId == CoursId && i.EtudiantId == EtudiantId);

            if (existe)
            {
                ErrorMessage = "⚠️ Cet étudiant est déjà inscrit à ce cours.";
            }
            else
            {
                var inscription = new Inscription
                {
                    CoursId = CoursId,
                    EtudiantId = EtudiantId
                };

                _context.Inscriptions.Add(inscription);
                await _context.SaveChangesAsync();

                SuccessMessage = "✅ Étudiant inscrit avec succès.";
            }

            await ChargerDonneesAsync();
            return Page();
        }

        private async Task ChargerDonneesAsync()
        {
            TousLesCours = await _context.Cours
                .Include(c => c.Enseignant)
                .Include(c => c.Inscriptions)
                    .ThenInclude(i => i.Etudiant)
                .ToListAsync();

            Etudiants = await _context.Users
                .Where(u => u.Role == "Etudiant")
                .ToListAsync();
        }
    }
}
