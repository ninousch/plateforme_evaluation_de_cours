using System;
using System.ComponentModel.DataAnnotations;
using frontend.Models;
namespace frontend.Models
{
    public class Inscription
    {
        public int Id { get; set; }

        [Required]
        public string EtudiantId { get; set; }
        public ApplicationUser Etudiant { get; set; }

        [Required]
        public int CoursId { get; set; }
        public Cours Cours { get; set; }

        public DateTime DateInscription { get; set; } = DateTime.Now;
    }
}
