using CourseFeedbackAPI.Data;
using CourseFeedbackAPI.Dtos;
using CourseFeedbackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class FeedbackSessionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedbackSessionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/feedbacksession
        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            var sessions = await _context.FeedbackSessions
                .Include(fs => fs.Course)
                .ToListAsync();
            return Ok(sessions);
        }

        // POST: api/feedbacksession
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] FeedbackSessionDto dto)
        {
            // Vérifier qu'il n'y a pas déjà une session active pour ce cours
            if (dto.IsActive)
            {
                var existe = await _context.FeedbackSessions
                    .AnyAsync(fs => fs.CourseId == dto.CourseId && fs.IsActive);
                if (existe)
                    return BadRequest("Il existe déjà une session active pour ce cours.");
            }

            var session = new FeedbackSession
            {
                CourseId = dto.CourseId,
                DateDebut = dto.DateDebut,
                DateFin = dto.DateFin,
                IsActive = dto.IsActive
            };

            _context.FeedbackSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(session);
        }

        // PUT: api/feedbacksession/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(int id, [FromBody] FeedbackSessionDto dto)
        {
            var session = await _context.FeedbackSessions.FindAsync(id);
            if (session == null)
                return NotFound();

            if (dto.IsActive && !session.IsActive)
            {
                // Si on veut activer cette session, vérifie qu'il n'y a pas déjà une session active pour ce cours
                var existe = await _context.FeedbackSessions
                    .AnyAsync(fs => fs.CourseId == dto.CourseId && fs.IsActive && fs.Id != id);
                if (existe)
                    return BadRequest("Il existe déjà une session active pour ce cours.");
            }

            session.CourseId = dto.CourseId;
            session.DateDebut = dto.DateDebut;
            session.DateFin = dto.DateFin;
            session.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return Ok(session);
        }

        // DELETE: api/feedbacksession/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var session = await _context.FeedbackSessions.FindAsync(id);
            if (session == null)
                return NotFound();

            _context.FeedbackSessions.Remove(session);
            await _context.SaveChangesAsync();

            return Ok("Session supprimée");
        }

        // GET: api/feedbacksession/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            var session = await _context.FeedbackSessions
                .Include(fs => fs.Course)
                .FirstOrDefaultAsync(fs => fs.Id == id);
            if (session == null)
                return NotFound();
            return Ok(session);
        }
    }
}