namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class AssignmentSubmission : AuditableEntity
    {
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool IsCorrect { get; set; }
        public string SelectedOption { get; set; }
        public string Answer { get; set; }
        public IList<AssignmentSubmissionAttachment> AssignmentSubmissionAttachments { get; set; }
    }
}