namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class AssignmentSubmissionAttachment : AuditableEntity
    {
        public Guid AssignmentSubmissionId { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string FileUrl { get; set; }
        public AssignmentSubmission AssignmentSubmission { get; set; }
        public User User { get; set; }
    }
}
