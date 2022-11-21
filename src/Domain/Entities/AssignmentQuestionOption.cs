namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class AssignmentQuestionOption : AuditableEntity
    {
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public string Option { get; set; }
        public bool IsCorrect { get; set; }
        public int Order { get; set; }
        public User User { get; set; }
    }
}