using frontend.DTOs;
namespace frontend.DTOs
{
    public class AdminStatsDto
    {
        public int TotalUtilisateurs { get; set; }
        public int TotalEtudiants { get; set; }
        public int TotalEnseignants { get; set; }
        public int TotalCours { get; set; }
        public int TotalSessions { get; set; }
        public int TotalFeedbacks { get; set; }
        public double MoyenneGlobale { get; set; }
    }
}
