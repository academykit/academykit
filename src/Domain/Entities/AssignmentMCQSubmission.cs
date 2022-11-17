namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class AssignmentMCQSubmission : AuditableEntity
    {
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid AssignmentQuestionId { get; set; }
        public AssignmentQuestion AssignmentQuestion { get; set; }
        public bool IsCorrect { get; set; }
        public string SelectedOption { get; set; }
    }
}