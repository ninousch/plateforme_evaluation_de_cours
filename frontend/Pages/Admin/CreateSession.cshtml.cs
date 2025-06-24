using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using frontend.Data;
using frontend.Models;
using System.ComponentModel.DataAnnotations;

namespace frontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateSessionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateSessionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SessionDto Session { get; set; } = new();

        public string? Message { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "Veuillez corriger les erreurs du formulaire.";
                return Page();
            }

            var nouvelleSession = new Session
            {
                Nom = Session.Nom,
                Description = Session.Description,
                DateDebut = Session.DateDebut,
                DateFin = Session.DateFin
            };

            _context.Sessions.Add(nouvelleSession);
            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ Session créée avec succès.";
            return RedirectToPage("/Admin/Sessions");
        }

        public class SessionDto
        {
            [Required]
            public string Nom { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            [Required]
            public DateTime DateDebut { get; set; }

            [Required]
            public DateTime DateFin { get; set; }
        }
    }
}
