namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class CourseTag : AuditableEntity
    {
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public User User { get; set; }
    }
}
