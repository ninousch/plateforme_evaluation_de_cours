using frontend.DTOs;
namespace frontend.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Etudiant", "Enseignant", "Admin"
        public string Nom { get; set; } // 👈 ajoute ceci
        public string Prenom { get; set; }
    }
}
