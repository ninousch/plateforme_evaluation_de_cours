using System.ComponentModel.DataAnnotations;
using frontend.Models;
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
    public int SessionId { get; set; }    // référence à Session
    public Session Session { get; set; }

    public string EtudiantId { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime DateSoumission { get; set; }
}

}
