using Microsoft.AspNetCore.Identity;

namespace CourseFeedbackAPI.Models
{
    public class User : IdentityUser
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Role { get; set; }
    }
}
