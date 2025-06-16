using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseFeedbackAPI.Models
{
    public class FeedbackSession
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateFin { get; set; }

        public bool IsActive { get; set; }
    }
}