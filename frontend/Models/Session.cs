using System;
using System.Collections.Generic;

namespace frontend.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }

        // Utile pour vérifier si la session est active
        public bool IsActive => DateTime.UtcNow >= DateDebut && DateTime.UtcNow <= DateFin;

        // Relation 1-n avec Feedback
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
