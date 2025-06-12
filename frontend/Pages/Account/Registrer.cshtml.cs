using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;                      // Pour IActionResult, BindProperty
using Microsoft.AspNetCore.Mvc.RazorPages;          // Pour PageModel
using Microsoft.AspNetCore.Identity;                // Pour IdentityUser, UserManager, SignInManager, IdentityRole, RoleManager
using System.ComponentModel.DataAnnotations;        // Pour Required, DataType

namespace frontend.Pages.Account
{
   public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            // Créer le rôle s'il n'existe pas
            if (!await _roleManager.RoleExistsAsync(Input.Role))
                await _roleManager.CreateAsync(new IdentityRole(Input.Role));

            await _userManager.AddToRoleAsync(user, Input.Role);
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Rediriger selon le rôle
            return Input.Role switch
            {
                "Admin" => RedirectToPage("/Admin/Dashboard"),
                "Enseignant" => RedirectToPage("/Enseignant/Dashboard"),
                "Etudiant" => RedirectToPage("/Etudiant/Dashboard"),
                _ => RedirectToPage("/Index")
            };
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }
}

}