namespace CourseFeedbackAPI.Dtos
{
    public class FeedbackSessionDto
    {
        public int CourseId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public bool IsActive { get; set; }
    }
}