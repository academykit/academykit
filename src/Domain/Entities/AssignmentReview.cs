namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class AssignmentReview : AuditableEntity
    {
        public Guid AssignmentSubmissionId { get; set; }
        public AssignmentSubmission AssignmentSubmission { get; set; }
        public string Mark { get; set; }
        public string Review { get; set; }
        public User User { get; set; }
        public bool IsDelete { get; set; }
    }
}