
using Microsoft.AspNetCore.Mvc.RazorPages;

public class MesCoursModel : PageModel
{
    public List<string> CoursInscrits { get; set; } = new List<string>
    {
        "Mathématiques", "Informatique", "Économie"
    };

    public void OnGet() {}
}
