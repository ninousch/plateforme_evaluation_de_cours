using frontend.Data;
using frontend.Dtos;
using frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/feedback")]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 Ajouter un feedback (étudiant connecté)
        [HttpPost]
        [Authorize(Roles = "Etudiant")]
        public async Task<IActionResult> LaisserFeedback([FromBody] FeedbackDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var cours = await _context.Cours.FindAsync(dto.CoursId);
            if (cours == null)
                return NotFound(new { message = "Cours non trouvé" });

            var feedback = new Feedback
            {
                Note = dto.Note,
                Commentaire = dto.Commentaire,
                CoursId = dto.CoursId,
                EtudiantId = userId, // ✅ doit être string dans Feedback.cs
                SessionId = dto.SessionId,
                DateSoumission = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Feedback envoyé avec succès" });
        }

        // 🔵 Voir les feedbacks statistiques d'un enseignant
        [HttpGet("enseignant/{enseignantId}/statistiques")]
        [Authorize(Roles = "Enseignant")]
        public async Task<IActionResult> GetStatistiquesPourEnseignant(string enseignantId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            if (userId != enseignantId)
                return Forbid();

            var cours = await _context.Cours
                .Where(c => c.EnseignantId == enseignantId)
                .Include(c => c.Feedbacks)
                .ToListAsync();

            var stats = cours.Select(c => new
            {
                CoursId = c.Id,
                Titre = c.Titre,
                MoyenneNote = c.Feedbacks.Any() ? Math.Round(c.Feedbacks.Average(f => f.Note), 2) : 0,
                Commentaires = c.Feedbacks
                    .Where(f => !string.IsNullOrWhiteSpace(f.Commentaire))
                    .Select(f => f.Commentaire)
                    .ToList()
            });

            return Ok(stats);
        }
    }
}
