namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;

    public class AssessmentQuestion : AuditableEntity
    {
        public Guid AssessmentId { get; set; }
        public Assessment Assessment { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public User User { get; set; }
        public IList<AssessmentOptions> AssessmentOptions { get; set; }
        public IList<AssessmentSubmissionAnswer> AssessmentSubmissionAnswers { get; set; }
    }
}
