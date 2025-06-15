namespace CourseFeedbackAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EnseignantId { get; set; } = string.Empty; // FK vers User.Id

        // Optionnel : navigation property
        public User? Enseignant { get; set; }
    }
}