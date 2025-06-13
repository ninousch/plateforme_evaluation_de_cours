using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursFeedback.API.Data;
using CoursFeedback.API.Models;

namespace CoursFeedback.API.Controllers
{
    [ApiController]
    [Route("api/feedback/sessions")]
    [Authorize(Roles = "Admin")]
    public class FeedbackSessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public FeedbackSessionsController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.FeedbackSessions
                                .Include(f => f.Course)
                                .OrderByDescending(f => f.StartDate)
                                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var session = await _db.FeedbackSessions
                                   .Include(f => f.Course)
                                   .FirstOrDefaultAsync(f => f.Id == id);
            if (session == null) return NotFound();
            return Ok(session);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackSession model)
        {
            if (model.StartDate > model.EndDate)
                return BadRequest("La date de début doit être antérieure à la date de fin.");

            if (model.IsActive &&
                await _db.FeedbackSessions.AnyAsync(f =>
                    f.CourseId == model.CourseId && f.IsActive))
            {
                return BadRequest("Une session active existe déjà pour ce cours.");
            }

            _db.FeedbackSessions.Add(model);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOne), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] FeedbackSession model)
        {
            if (id != model.Id) return BadRequest("ID invalide.");
            if (model.StartDate > model.EndDate)
                return BadRequest("La date de début doit être antérieure à la date de fin.");

            if (model.IsActive &&
                await _db.FeedbackSessions.AnyAsync(f =>
                    f.CourseId == model.CourseId && f.IsActive && f.Id != id))
            {
                return BadRequest("Une session active existe déjà pour ce cours.");
            }

            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _db.FeedbackSessions.FindAsync(id);
            if (session == null) return NotFound();

            _db.FeedbackSessions.Remove(session);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
