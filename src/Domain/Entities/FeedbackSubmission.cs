namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class FeedbackSubmission : AuditableEntity
    {
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public Guid FeedbackId { get; set; }
        public Feedback Feedback { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string SelectedOption { get; set; }
        public string Answer { get; set; }
        public int? Rating { get; set; }
    }
}
