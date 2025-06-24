using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using frontend.Models;

namespace frontend.Pages.Student
{
    [Authorize(Roles = "Etudiant")]
    public class DashboardStudentModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DashboardStudentModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public string? UserEmail { get; set; }
        public string[]? UserRoles { get; set; }

        public void OnGet()
        {
            UserEmail = User.FindFirstValue(ClaimTypes.Email);

            var rolesClaims = User.FindAll(ClaimTypes.Role);
            UserRoles = rolesClaims.Select(c => c.Value).ToArray();

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Response.Redirect("/Auth/Login");
            }
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
