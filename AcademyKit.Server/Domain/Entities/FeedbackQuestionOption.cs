namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class FeedbackQuestionOption : AuditableEntity
    {
        public Guid FeedbackId { get; set; }
        public Feedback Feedback { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }
        public User User { get; set; }
    }
}
