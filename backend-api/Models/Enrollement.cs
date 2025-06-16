namespace CourseFeedbackAPI.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public User? User { get; set; }
        public Course? Course { get; set; }
    }
}