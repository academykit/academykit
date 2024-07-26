namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class AssignmentAttachment : AuditableEntity
    {
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public string FileUrl { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public User User { get; set; }
    }
}
