using frontend.DTOs;
namespace frontend.Dtos
{
    public class FeedbackDto
    {
        public int Note { get; set; }           // Note entre 1 et 5
        public string Commentaire { get; set; } // Commentaire libre
        public int CoursId { get; set; }        // Cours évalué
        public int SessionId { get; set; }      // Session de feedback active
    }
}
