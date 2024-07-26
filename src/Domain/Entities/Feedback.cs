namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class Feedback : AuditableEntity
    {
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public FeedbackTypeEnum Type { get; set; }
        public User User { get; set; }
        public IList<FeedbackSubmission> FeedbackSubmissions { get; set; }
        public IList<FeedbackQuestionOption> FeedbackQuestionOptions { get; set; }
    }
}
