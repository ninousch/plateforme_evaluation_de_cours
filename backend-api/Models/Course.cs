using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoursFeedback.API.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        // Propriété de navigation vers les sessions
        public ICollection<FeedbackSession> FeedbackSessions { get; set; }
    }
}
