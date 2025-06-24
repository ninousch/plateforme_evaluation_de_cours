using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using frontend.Models;
using frontend.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace frontend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // ✅ Lister tous les utilisateurs (admin uniquement)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserAdminDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Aucun";

                userList.Add(new UserAdminDto
                {
                    Id = user.Id,
                    Nom = user.Nom ?? "",
                    Prenom = user.Prenom ?? "",
                    Email = user.Email,
                    Role = role
                });
            }

            return Ok(userList);
        }

        // ✅ Modifier le rôle d’un utilisateur (admin uniquement)
        [HttpPost("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("Utilisateur non trouvé");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            user.Role = newRole;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Rôle mis à jour avec succès" });
        }

        // ✅ Lister uniquement les enseignants
        [HttpGet("enseignants")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEnseignants()
        {
            var allUsers = _userManager.Users.ToList();
            var enseignants = new List<object>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Enseignant"))
                {
                    enseignants.Add(new
                    {
                        user.Id,
                        user.Nom,
                        user.Prenom
                    });
                }
            }

            return Ok(enseignants);
        }

        // ✅ Connexion simple avec cookies
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Email ou mot de passe incorrect");

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Unauthorized("Email ou mot de passe incorrect");

            return Ok(new { message = "Connexion réussie" });
        }
    }
}
