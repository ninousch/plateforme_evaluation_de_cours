using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using frontend.Models;

namespace frontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateUserModel(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public CreateUserDto User { get; set; } = new();

        public string? Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "Veuillez remplir correctement tous les champs.";
                return Page();
            }

            var existingUser = await _userManager.FindByEmailAsync(User.Email);
            if (existingUser != null)
            {
                Message = "Un utilisateur avec cet email existe déjà.";
                return Page();
            }

            var newUser = new ApplicationUser
            {
                UserName = User.Email,
                Email = User.Email,
                Nom = User.Nom,
                Prenom = User.Prenom,
                Role = User.Role
            };

            var result = await _userManager.CreateAsync(newUser, User.Password);

            if (!result.Succeeded)
            {
                Message = string.Join("; ", result.Errors.Select(e => e.Description));
                return Page();
            }

            // Ajouter au rôle
            await _userManager.AddToRoleAsync(newUser, User.Role);

            Message = "Utilisateur créé avec succès.";
            return Page();
        }

        public class CreateUserDto
        {
            [Required]
            public string Nom { get; set; } = string.Empty;

            [Required]
            public string Prenom { get; set; } = string.Empty;

            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;

            [Required]
            public string Role { get; set; } = string.Empty;
        }
    }
}
