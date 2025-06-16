using CourseFeedbackAPI.Data;
using CourseFeedbackAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Etudiant")]
    public class EnrollmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST : /api/enrollment (un étudiant s'inscrit à un cours)
        [HttpPost("{courseId}")]
        [Authorize(Roles = "Etudiant")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var already = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
            if (already)
                return BadRequest("Déjà inscrit.");

            var enrollment = new Enrollment
            {
                UserId = userId!,
                CourseId = courseId
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return Ok("Inscription réussie !");
        }

        // GET : /api/enrollment/my (voir ses propres inscriptions)
        [HttpGet("my")]
        [Authorize(Roles = "Etudiant")]
        public async Task<IActionResult> MyEnrollments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .Select(e => new { e.CourseId, e.Course!.Titre, e.Course.Description })
                .ToListAsync();
            return Ok(courses);
        }
    }
}