using Microsoft.AspNetCore.Identity;

namespace CourseFeedbackAPI.Models
{
    public class User : IdentityUser
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}