using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using frontend.Models;
namespace frontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]  
    public class DashboardAdminModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public DashboardAdminModel(SignInManager<ApplicationUser> signInManager)
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

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
