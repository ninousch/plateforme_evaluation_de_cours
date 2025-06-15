namespace CourseFeedbackAPI.Dtos
{
    public class RegisterDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Nom { get; set; } = "";
        public string Prenom { get; set; } = "";
        public string Role { get; set; }  = ""; // "Admin", "Etudiant", "Enseignant"
    }
}