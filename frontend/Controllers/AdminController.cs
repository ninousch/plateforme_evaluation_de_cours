using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using frontend.Data;
using frontend.Models;
using frontend.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")] // ✅ Accès restreint à l’admin
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // ✅ GET : Statistiques globales
        [HttpGet("statistiques")]
        public async Task<IActionResult> GetStatistiques()
        {
            var totalUtilisateurs = await _userManager.Users.CountAsync();
            var totalEtudiants = await _userManager.GetUsersInRoleAsync("Etudiant");
            var totalEnseignants = await _userManager.GetUsersInRoleAsync("Enseignant");

            var totalCours = await _context.Cours.CountAsync();
            var totalSessions = await _context.Sessions.CountAsync();
            var totalFeedbacks = await _context.Feedbacks.CountAsync();
            var moyenneGlobale = await _context.Feedbacks.AnyAsync()
                ? Math.Round(await _context.Feedbacks.AverageAsync(f => f.Note), 2)
                : 0;

            var statsDto = new AdminStatsDto
            {
                TotalUtilisateurs = totalUtilisateurs,
                TotalEtudiants = totalEtudiants.Count,
                TotalEnseignants = totalEnseignants.Count,
                TotalCours = totalCours,
                TotalSessions = totalSessions,
                TotalFeedbacks = totalFeedbacks,
                MoyenneGlobale = moyenneGlobale
            };

            return Ok(statsDto);
        }

        // ✅ GET : Liste des sessions
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var sessions = await _context.Sessions
                .Select(s => new SessionDto
                {
                    Id = s.Id,
                    Titre = s.Nom,
                    Debut = s.DateDebut,
                    Fin = s.DateFin
                }).ToListAsync();

            return Ok(sessions);
        }

        // ✅ POST : Ajouter une session
        [HttpPost("sessions")]
        public async Task<IActionResult> AjouterSession([FromBody] SessionDto dto)
        {
            var session = new Session
            {
                Nom = dto.Titre,
                DateDebut = dto.Debut,
                DateFin = dto.Fin
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ DELETE : Supprimer une session
        [HttpDelete("sessions/{id}")]
        public async Task<IActionResult> SupprimerSession(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null) return NotFound();

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ GET : Tous les utilisateurs
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserAdminDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserAdminDto
                {
                    Id = user.Id,
                    Nom = user.Nom,
                    Prenom = user.Prenom,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "N/A"
                });
            }

            return Ok(result);
        }

        // ✅ DELETE : Supprimer un utilisateur
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> SupprimerUtilisateur(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return NoContent();
        }
// GET : Liste des utilisateurs avec rôle "Enseignant"
[HttpGet("users/enseignants")]
public async Task<IActionResult> GetEnseignants()
{
    var enseignants = await _userManager.GetUsersInRoleAsync("Enseignant");

    var result = enseignants.Select(user => new UserAdminDto
    {
        Id = user.Id,
        Nom = user.Nom,
        Prenom = user.Prenom,
        Email = user.Email,
        Role = "Enseignant"
    }).ToList();

    return Ok(result);
}

        // ✅ PUT : Modifier le rôle d’un utilisateur
        [HttpPut("users/{id}/changerole")]
        public async Task<IActionResult> ChangerRole(string id, [FromBody] RoleUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, dto.NewRole);

            return Ok();
        }
    }

    // ✅ DTOs utilisés (à déplacer dans un fichier DTOs si nécessaire)
    public class AdminStatsDto
    {
        public int TotalUtilisateurs { get; set; }
        public int TotalEtudiants { get; set; }
        public int TotalEnseignants { get; set; }
        public int TotalCours { get; set; }
        public int TotalSessions { get; set; }
        public int TotalFeedbacks { get; set; }
        public double MoyenneGlobale { get; set; }
    }

    public class SessionDto
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
    }

    public class UserAdminDto
    {
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class RoleUpdateDto
    {
        public string NewRole { get; set; }
    }
}
