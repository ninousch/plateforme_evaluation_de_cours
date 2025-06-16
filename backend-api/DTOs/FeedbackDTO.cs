namespace CourseFeedbackAPI.Dtos
{
    public class FeedbackDto
    {
        public int CourseId { get; set; }
        public int FeedbackSessionId { get; set; }
        public int Note { get; set; }
        public string? Commentaire { get; set; }
    }
}