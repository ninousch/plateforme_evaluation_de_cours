using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;                      // Pour IActionResult, BindProperty
using Microsoft.AspNetCore.Mvc.RazorPages;          // Pour PageModel
using Microsoft.AspNetCore.Identity;                // Pour IdentityUser, UserManager, SignInManager, IdentityRole, RoleManager
using System.ComponentModel.DataAnnotations;  
namespace frontend.Pages.Account
{
    public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return RedirectToPage("/Admin/Dashboard");
            if (roles.Contains("Enseignant"))
                return RedirectToPage("/Enseignant/Dashboard");
            if (roles.Contains("Etudiant"))
                return RedirectToPage("/Etudiant/Dashboard");

            return RedirectToPage("/Index");
        }

        ErrorMessage = "Identifiants incorrects.";
        return Page();
    }
}

}