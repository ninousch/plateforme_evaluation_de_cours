using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using frontend.Models;

namespace frontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<UserAdminDto> Users { get; set; } = new();

        [BindProperty]
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var allUsers = _userManager.Users.ToList();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserAdminDto
                {
                    Id = user.Id,
                    Nom = user.Nom ?? "",
                    Prenom = user.Prenom ?? "",
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "Aucun"
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ErrorMessage = "Utilisateur introuvable.";
                return Page();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ErrorMessage = "Erreur lors de la suppression.";
                return Page();
            }

            SuccessMessage = "Utilisateur supprimé avec succès.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(string id, string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ErrorMessage = "Utilisateur introuvable.";
                return Page();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            user.Role = newRole;
            await _userManager.UpdateAsync(user);

            SuccessMessage = "Rôle modifié avec succès.";
            return RedirectToPage();
        }

        public class UserAdminDto
        {
            public string Id { get; set; } = null!;
            public string Nom { get; set; } = null!;
            public string Prenom { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Role { get; set; } = null!;
        }
    }
}
