using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using frontend.Models;
using System.Threading.Tasks;
using System.Linq;

namespace frontend.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginDto LoginDto { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Veuillez remplir tous les champs.";
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(LoginDto.Email);
            if (user == null)
            {
                ErrorMessage = "Utilisateur introuvable.";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, LoginDto.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ErrorMessage = "Mot de passe incorrect.";
                return Page();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return role switch
            {
                "Admin" => RedirectToPage("/Admin/DashboardAdmin"),
                "Enseignant" => RedirectToPage("/Teacher/Dashboard"),
                "Etudiant" => RedirectToPage("/Student/DashboardStudent"),
                _ => RedirectToPage("/Index")
            };
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
