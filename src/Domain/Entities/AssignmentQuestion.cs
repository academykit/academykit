namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class AssignmentQuestion : AuditableEntity
    {
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
        public int Order { get; set; }
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public User User { get; set; }
        public IList<AssignmentMCQSubmission> AssignmentMCQSubmissions { get; set; }
    }
}