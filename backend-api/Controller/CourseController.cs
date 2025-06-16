using CourseFeedbackAPI.Data;
using CourseFeedbackAPI.Dtos;
using CourseFeedbackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CourseController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/course
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Enseignant)
                .ToListAsync();

            return Ok(courses.Select(c => new
            {
                c.Id,
                c.Titre,
                c.Description,
                Enseignant = c.Enseignant == null ? null : new { c.Enseignant.Id, c.Enseignant.Nom, c.Enseignant.Prenom }
            }));
        }

        // POST: api/course
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDto dto)
        {
            // Vérifie si l'utilisateur assigné est enseignant
            var enseignant = await _userManager.FindByIdAsync(dto.EnseignantId);
            if (enseignant == null || !(await _userManager.IsInRoleAsync(enseignant, "Enseignant")))
            {
                return BadRequest("L'utilisateur assigné doit exister et avoir le rôle Enseignant.");
            }

            var course = new Course
            {
                Titre = dto.Titre,
                Description = dto.Description,
                EnseignantId = dto.EnseignantId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cours créé avec succès.", course.Id });
        }

        // PUT: api/course/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound("Cours introuvable.");
            }

            // Vérifie si l'utilisateur assigné est enseignant
            var enseignant = await _userManager.FindByIdAsync(dto.EnseignantId);
            if (enseignant == null || !(await _userManager.IsInRoleAsync(enseignant, "Enseignant")))
            {
                return BadRequest("L'utilisateur assigné doit exister et avoir le rôle Enseignant.");
            }

            course.Titre = dto.Titre;
            course.Description = dto.Description;
            course.EnseignantId = dto.EnseignantId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cours modifié avec succès." });
        }

        // DELETE: api/course/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound("Cours introuvable.");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cours supprimé avec succès." });
        }
    }
}