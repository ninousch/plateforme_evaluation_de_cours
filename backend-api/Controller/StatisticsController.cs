using CourseFeedbackAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Enseignant")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Moyenne et détails des feedbacks d'un cours
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseStatistics(int courseId)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => f.CourseId == courseId)
                .ToListAsync();

            if (!feedbacks.Any())
                return NotFound("Aucun feedback pour ce cours.");

            var moyenne = feedbacks.Average(f => f.Note);

            return Ok(new
            {
                CourseId = courseId,
                Moyenne = moyenne,
                Feedbacks = feedbacks.Select(f => new
                {
                    f.Note,
                    f.Commentaire,
                    f.FeedbackSessionId
                })
            });
        }

        // Moyennes et feedbacks pour tous les cours d'un enseignant
        [HttpGet("teacher/{enseignantId}")]
        public async Task<IActionResult> GetTeacherStatistics(string enseignantId)
        {
            var cours = await _context.Courses
                .Where(c => c.EnseignantId == enseignantId)
                .Include(c => c.Feedbacks)
                .ToListAsync();

            if (!cours.Any())
                return NotFound("Aucun cours trouvé pour cet enseignant.");

            var result = cours.Select(c => new
            {
                CourseId = c.Id,
                c.Titre,
                Moyenne = c.Feedbacks.Any() ? c.Feedbacks.Average(f => f.Note) : 0,
                Feedbacks = c.Feedbacks.Select(f => new
                {
                    f.Note,
                    f.Commentaire,
                    f.FeedbackSessionId
                })
            });

            return Ok(result);
        }
    }
}