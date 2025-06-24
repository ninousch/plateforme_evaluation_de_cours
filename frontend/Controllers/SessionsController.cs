using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using frontend.Data;
using frontend.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace frontend.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    [Authorize(Roles = "Admin")]
    public class SessionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SessionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Créer une nouvelle session
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Session session)
        {
            if (session == null)
                return BadRequest("Session invalide.");

            if (string.IsNullOrEmpty(session.Nom))
                return BadRequest("Le nom de la session est obligatoire.");

            if (session.DateDebut >= session.DateFin)
                return BadRequest("La date de début doit être antérieure à la date de fin.");

            try
            {
                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();
                return Ok(session);
            }
            catch (Exception ex)
            {
                // Ici tu peux logger l'exception ex si tu as un système de log
                return StatusCode(500, "Erreur serveur.");
            }
        }

        // Activer manuellement une session
        [HttpPost("activate/{id}")]
        public async Task<IActionResult> Activate(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
                return NotFound();

            session.DateDebut = DateTime.UtcNow;
            session.DateFin = DateTime.UtcNow.AddDays(7); // Ajuste la durée si besoin

            await _context.SaveChangesAsync();
            return Ok(new { message = "Session activée avec succès", session });
        }

        // Fermer manuellement une session
        [HttpPost("close/{id}")]
        public async Task<IActionResult> Close(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
                return NotFound();

            session.DateFin = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Session fermée avec succès", session });
        }

        // Récupérer toutes les sessions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sessions = await _context.Sessions.ToListAsync();
            return Ok(sessions);
        }

        // Récupérer uniquement les sessions actives
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var now = DateTime.UtcNow;
            var activeSessions = await _context.Sessions
                .Where(s => s.DateDebut <= now && s.DateFin >= now)
                .ToListAsync();

            return Ok(activeSessions);
        }
    }
}
