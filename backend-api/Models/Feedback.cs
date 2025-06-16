using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseFeedbackAPI.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Required]
        public int FeedbackSessionId { get; set; }
        public FeedbackSession? FeedbackSession { get; set; }

        [Required]
        [Range(1,5)]
        public int Note { get; set; }

        public string? Commentaire { get; set; }

        // Pour l'anonymat : UserId n'est pas exposé publiquement.
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}