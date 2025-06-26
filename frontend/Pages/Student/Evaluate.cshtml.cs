using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using frontend.Data;
using frontend.Models;
using System.Security.Claims;

namespace frontend.Pages.Student
{
    public class EvaluateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EvaluateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FeedbackDto Feedback { get; set; } = new();

        public List<CourseDto> CoursList { get; set; } = new();
        public Session? SessionActive { get; set; }
        public string ErrorMessage { get; set; } = "";

        public class CourseDto
        {
            public int Id { get; set; }
            public string Titre { get; set; } = string.Empty;
        }

        public class FeedbackDto
        {
            [Range(1, 5)]
            public int Note { get; set; }

            public string? Commentaire { get; set; }

            [Required]
            public int CoursId { get; set; }

            [Required]
            public int SessionId { get; set; }

            // Champs supplémentaires à évaluer
            public string? Connaissances { get; set; }
            public string? Clarte { get; set; }
            public string? Organisation { get; set; }
            public string? Interet { get; set; }
            public string? FeedbackNotes { get; set; }
            public string? Equite { get; set; }
            public string? Accessibilite { get; set; }
            public string? Respect { get; set; }
            public string? Pertinence { get; set; }
            public string? Impact { get; set; }
            public string? CommentaireFinal { get; set; }
        }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                ErrorMessage = "Vous devez être connecté pour évaluer un cours.";
                return;
            }

            SessionActive = await _context.Sessions
                .Where(s => s.DateDebut <= DateTime.Now && s.DateFin >= DateTime.Now)
                .OrderByDescending(s => s.DateDebut)
                .FirstOrDefaultAsync();

            if (SessionActive == null)
            {
                ErrorMessage = "Aucune session active actuellement.";
                return;
            }

            var coursInscrits = await _context.Inscriptions
                .Where(i => i.EtudiantId == userId)
                .Select(i => i.CoursId)
                .ToListAsync();

            var coursEvaluésIds = await _context.Feedbacks
                .Where(f => f.EtudiantId == userId && f.SessionId == SessionActive.Id)
                .Select(f => f.CoursId)
                .ToListAsync();

            CoursList = await _context.Cours
                .Where(c => coursInscrits.Contains(c.Id) && !coursEvaluésIds.Contains(c.Id))
                .Select(c => new CourseDto { Id = c.Id, Titre = c.Titre })
                .ToListAsync();

            Feedback.SessionId = SessionActive.Id;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CoursList = await _context.Cours
                .Select(c => new CourseDto { Id = c.Id, Titre = c.Titre })
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ErrorMessage = "Vous devez être connecté pour soumettre une évaluation.";
                return Page();
            }

            var session = await _context.Sessions.FindAsync(Feedback.SessionId);
            if (session == null || session.DateDebut > DateTime.Now || session.DateFin < DateTime.Now)
            {
                ErrorMessage = "La session d’évaluation n’est plus active.";
                return Page();
            }

            var estInscrit = await _context.Inscriptions
                .AnyAsync(i => i.EtudiantId == userId && i.CoursId == Feedback.CoursId);

            if (!estInscrit)
            {
                ErrorMessage = "Vous n'êtes pas inscrit à ce cours.";
                return Page();
            }

            var existeDejaEvaluation = await _context.Feedbacks
                .AnyAsync(f => f.EtudiantId == userId && f.CoursId == Feedback.CoursId && f.SessionId == Feedback.SessionId);

            if (existeDejaEvaluation)
            {
                ErrorMessage = "Vous avez déjà évalué ce cours.";
                return Page();
            }

            var feedback = new Feedback
            {
                Note = Feedback.Note,
                Commentaire = Feedback.Commentaire ?? "",
                CoursId = Feedback.CoursId,
                SessionId = Feedback.SessionId,
                EtudiantId = userId,
                DateCreation = DateTime.UtcNow,

                // Champs supplémentaires
                Connaissances = Feedback.Connaissances,
                Clarte = Feedback.Clarte,
                Organisation = Feedback.Organisation,
                Interet = Feedback.Interet,
                FeedbackNotes = Feedback.FeedbackNotes,
                Equite = Feedback.Equite,
                Accessibilite = Feedback.Accessibilite,
                Respect = Feedback.Respect,
                Pertinence = Feedback.Pertinence,
                Impact = Feedback.Impact,
                CommentaireFinal = Feedback.CommentaireFinal
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ Votre évaluation a été enregistrée avec succès.";
            return RedirectToPage();
        }
    }
}
