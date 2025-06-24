using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using frontend.Models;
using frontend.Data;
using System.ComponentModel.DataAnnotations;

namespace frontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateCourseModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CreateCourseModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public CreateCourseDto Course { get; set; } = new();

        public List<EnseignantDto> Enseignants { get; set; } = new();

        public string? Message { get; set; }

        public async Task OnGetAsync()
        {
            var allUsers = _userManager.Users.ToList();
            Enseignants = new();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Enseignant"))
                {
                    Enseignants.Add(new EnseignantDto
                    {
                        Id = user.Id,
                        Nom = user.Nom ?? "",
                        Prenom = user.Prenom ?? ""
                    });
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // Recharge la liste des enseignants
                return Page();
            }

            var enseignant = await _userManager.FindByIdAsync(Course.EnseignantId);
            if (enseignant == null)
            {
                Message = "Enseignant introuvable.";
                await OnGetAsync();
                return Page();
            }

            var newCours = new Cours
            {
                Titre = Course.Titre,
                Description = Course.Description,
                EnseignantId = enseignant.Id
            };

            _context.Cours.Add(newCours);
            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ Cours créé avec succès.";
            return RedirectToPage("/Admin/Courses");
        }

        public class CreateCourseDto
        {
            [Required]
            public string Titre { get; set; } = string.Empty;

            [Required]
            public string Description { get; set; } = string.Empty;

            [Required]
            public string EnseignantId { get; set; } = string.Empty;
        }

        public class EnseignantDto
        {
            public string Id { get; set; } = string.Empty;
            public string Nom { get; set; } = string.Empty;
            public string Prenom { get; set; } = string.Empty;
            public string NomComplet => $"{Nom} {Prenom}";
        }
    }
}
