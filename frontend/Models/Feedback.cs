using System.ComponentModel.DataAnnotations;

namespace frontend.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Note { get; set; }

        public string Commentaire { get; set; }

        [Required]
        public int CoursId { get; set; }
        public Cours Cours { get; set; }

        [Required]
        public int SessionId { get; set; }
        public Session Session { get; set; }

        public string EtudiantId { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        public DateTime DateSoumission { get; set; } = DateTime.UtcNow;

        // ✅ Champs supplémentaires issus du formulaire
        public string? Connaissances { get; set; }

        public string? Clarte { get; set; }

        public string? Organisation { get; set; }

        public string? Interet { get; set; }

        public string? FeedbackNotes { get; set; }

        public string? Equite { get; set; }

        public string? Accessibilite { get; set; }

        public string? Respect { get; set; }

        public string? Pertinence { get; set; }

        public string? Impact { get; set; }

        public string? CommentaireFinal { get; set; }
    }
}
