namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class CourseCertificate : AuditableEntity
    {
        public string Title { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string SampleUrl { get; set; }
        public User User { get; set; }

    }
}
