using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using frontend.Data;
using frontend.Models;
using System.Security.Claims;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/inscriptions")]
    [Authorize(Roles = "Admin,Etudiant")]
    public class InscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> InscrireEtudiant(int coursId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await _context.Inscriptions.AnyAsync(i => i.EtudiantId == userId && i.CoursId == coursId))
                return BadRequest("Déjà inscrit à ce cours");

            var inscription = new Inscription
            {
                EtudiantId = userId,
                CoursId = coursId
            };

            _context.Inscriptions.Add(inscription);
            await _context.SaveChangesAsync();

            return Ok(inscription);
        }
    }
}
