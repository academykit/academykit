namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class AssessmentOptions : AuditableEntity
    {
        public Guid AssessmentQuestionId { get; set; }
        public AssessmentQuestion AssessmentQuestion { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }
        public User User { get; set; }
        public bool IsCorrect { get; set; }
    }
}
