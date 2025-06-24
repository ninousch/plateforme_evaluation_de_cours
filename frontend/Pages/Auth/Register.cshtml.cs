using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using frontend.Models;
using System.Threading.Tasks;
using System;

namespace frontend.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public RegisterDto RegisterDto { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Formulaire invalide. Veuillez vérifier les champs.";
                return Page();
            }

            try
            {
                var user = new ApplicationUser
                {
                    UserName = RegisterDto.Email,
                    Email = RegisterDto.Email,
                    Nom = RegisterDto.Nom,
                    Prenom = RegisterDto.Prenom,
                    Role = RegisterDto.Role
                };

                var result = await _userManager.CreateAsync(user, RegisterDto.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    ErrorMessage = "Erreur lors de l'inscription.";
                    return Page();
                }

                // Vérifie et ajoute le rôle
                if (!await _roleManager.RoleExistsAsync(RegisterDto.Role))
                    await _roleManager.CreateAsync(new IdentityRole(RegisterDto.Role));

                await _userManager.AddToRoleAsync(user, RegisterDto.Role);

                SuccessMessage = "Inscription réussie. Vous pouvez maintenant vous connecter.";
                return RedirectToPage("/Auth/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'inscription");
                ErrorMessage = "Une erreur est survenue. Veuillez réessayer plus tard.";
                return Page();
            }
        }
    }

    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Etudiant";
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
    }
}
