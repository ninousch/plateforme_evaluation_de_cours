using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using frontend.Models;

namespace frontend.Models
{
    public class Cours
    {
        public int Id { get; set; }

        [Required]
        public string Titre { get; set; }

        public string Description { get; set; }
        public DateTime DatePublication { get; set; }
        // Enseignant assigné
        public string EnseignantId { get; set; }
        public ApplicationUser Enseignant { get; set; }

        public ICollection<Inscription> Inscriptions { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
