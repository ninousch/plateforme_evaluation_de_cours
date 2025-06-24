using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using frontend.Data;
using frontend.Models;
using frontend.DTOs;
using System.Security.Claims;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/cours")]
    public class CoursController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 Liste tous les cours
        [HttpGet]
        public async Task<IActionResult> GetCours()
        {
            var cours = await _context.Cours
                .Include(c => c.Enseignant)
                .ToListAsync();

            return Ok(cours);
        }

        // 🟢 Création de cours (Admin uniquement)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCours(Cours cours)
        {
            var enseignant = await _context.Users.FindAsync(cours.EnseignantId);
    if (enseignant == null)
    {
        return BadRequest(new { message = "Enseignant introuvable." });
    }
            _context.Cours.Add(cours);
            cours.DatePublication = DateTime.UtcNow; // Ajoute la date de publication à la création
            _context.Cours.Add(cours);
            await _context.SaveChangesAsync();

            return Ok(cours);
        }

        // 🟢 Liste des cours avec étudiants (Admin)
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCoursPourAdmin()
        {
            var cours = await _context.Cours
                .Include(c => c.Enseignant)
                .Include(c => c.Inscriptions)
                    .ThenInclude(i => i.Etudiant)
                .ToListAsync();

            var dto = cours.Select(c => new AdminCoursDto
            {
                Id = c.Id,
                Titre = c.Titre,
                Description = c.Description,
                EnseignantNom = $"{c.Enseignant.Nom} {c.Enseignant.Prenom}",
                Etudiants = c.Inscriptions.Select(i => new EtudiantDto
                {
                    Id = i.Etudiant.Id,
                    Nom = i.Etudiant.Nom,
                    Prenom = i.Etudiant.Prenom,
                    Email = i.Etudiant.Email
                }).ToList()
            });

            return Ok(dto);
        }

        // 🔵 Statistiques pour l'enseignant connecté
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
                Titre = c.Titre,
                MoyenneNote = c.Feedbacks.Any() ? Math.Round(c.Feedbacks.Average(f => f.Note), 2) : 0,
                Commentaires = c.Feedbacks
                    .Where(f => !string.IsNullOrWhiteSpace(f.Commentaire))
                    .Select(f => f.Commentaire)
                    .ToList()
            });

            return Ok(stats);
        }

        // 🔵 Cours de l'étudiant connecté
        [HttpGet("mescours")]
        [Authorize(Roles = "Etudiant")]
        public async Task<IActionResult> GetCoursPourEtudiant()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var cours = await _context.Inscriptions
                .Where(i => i.EtudiantId == userId)
                .Select(i => new
                {
                    i.Cours.Id,
                    i.Cours.Titre,
                    i.Cours.Description,
                    i.Cours.DatePublication,
                    EnseignantNom = i.Cours.Enseignant.Nom,
                    EnseignantPrenom = i.Cours.Enseignant.Prenom
                })
                .ToListAsync();

            return Ok(cours);
        }
    }
}
