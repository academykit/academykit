namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class Signature : AuditableEntity
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string FileUrl { get; set; }
        public User User { get; set; }
    }
}