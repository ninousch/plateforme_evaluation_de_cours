using CourseFeedbackAPI.Data;
using CourseFeedbackAPI.Dtos;
using CourseFeedbackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Etudiant")]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public FeedbackController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Vérifie inscription
            var inscrit = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == dto.CourseId);
            if (!inscrit)
                return BadRequest("Vous n'êtes pas inscrit à ce cours.");

            // Session valide et active
            var session = await _context.FeedbackSessions
                .FirstOrDefaultAsync(s => s.Id == dto.FeedbackSessionId && s.CourseId == dto.CourseId && s.IsActive);
            if (session == null)
                return BadRequest("Session d’évaluation invalide ou inactive.");

            // Unicité feedback étudiant/session
            var dejaFait = await _context.Feedbacks
                .AnyAsync(f => f.UserId == userId && f.FeedbackSessionId == dto.FeedbackSessionId);
            if (dejaFait)
                return BadRequest("Vous avez déjà évalué ce cours pour cette session.");

            var feedback = new Feedback
            {
                CourseId = dto.CourseId,
                FeedbackSessionId = dto.FeedbackSessionId,
                Note = dto.Note,
                Commentaire = dto.Commentaire,
                UserId = userId!
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Pour l'anonymat, on ne retourne pas le UserId
            return Ok(new { message = "Merci pour votre feedback !" });
        }
    }
}