using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using frontend.Data;
using Microsoft.EntityFrameworkCore;

namespace frontend.Controllers
{
    [ApiController]
    [Route("api/statistiques")]
    public class StatistiquesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatistiquesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("cours/{id}")]
        [Authorize(Roles = "Admin,Enseignant")]
        public IActionResult GetStats(int id)
        {
            var feedbacks = _context.Feedbacks
                .Where(f => f.CoursId == id)
                .ToList();

            if (!feedbacks.Any())
                return NotFound("Aucun feedback trouvé pour ce cours.");

            var moyenne = feedbacks.Average(f => f.Note);
            var commentaires = feedbacks.Select(f => f.Commentaire).Where(c => !string.IsNullOrEmpty(c)).ToList();

            return Ok(new
            {
                Moyenne = moyenne,
                Nombre = feedbacks.Count,
                Commentaires = commentaires
            });
        }
    }
}
