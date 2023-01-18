namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class Assignment : AuditableEntity
    {
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public User User { get; set; }
        public IList<AssignmentAttachment> AssignmentAttachments { get; set; }
        public IList<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public IList<AssignmentQuestionOption> AssignmentQuestionOptions { get; set; }
    }
}