using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Net.Http.Json;
using frontend.Data;
using frontend.Models;
using Microsoft.EntityFrameworkCore;

namespace frontend.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class SessionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SessionsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Nom { get; set; }

        [BindProperty]
        public DateTime DateDebut { get; set; }

        [BindProperty]
        public DateTime DateFin { get; set; }

        public List<Session> Sessions { get; set; } = new();

        public async Task OnGetAsync()
        {
            Sessions = await _context.Sessions.ToListAsync();
        }

       public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
    {
        await OnGetAsync();
        return Page();
    }

    var session = new Session
    {
        Nom = Nom,
        DateDebut = DateDebut,
        DateFin = DateFin
    };

    _context.Sessions.Add(session);
    await _context.SaveChangesAsync();

    TempData["Message"] = "✅ Session créée avec succès.";
    return RedirectToPage();
}

        

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    
}

}
