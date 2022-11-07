namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class Meeting : AuditableEntity
    {
        public long MeetingNumber { get; set; }
        public string PassCode { get; set; }
        public Guid ZoomLicenseId { get; set; }
        public ZoomLicense ZoomLicense { get; set; }
        public DateTime? StartDate { get; set; }
        public IList<MeetingReport> MeetingReports { get; set; }
        public IList<Lesson> Lessons { get; set; }
        public User User { get; set; }
    }
}