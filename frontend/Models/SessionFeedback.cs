using System;
using System.ComponentModel.DataAnnotations;

namespace frontend.Models
{
    public class SessionFeedback
    {
        public int Id { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateFin { get; set; }

        public bool EstActive { get; set; }
    }
}
