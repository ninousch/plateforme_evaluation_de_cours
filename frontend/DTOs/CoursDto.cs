// DTO de coursnamespace backend_api.DTOs
using frontend.DTOs;
namespace frontend.Dtos
{
    public class CoursDto
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public string Enseignant { get; set; }
        public DateTime DatePublication { get; set; }
    }
}
