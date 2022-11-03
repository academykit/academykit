namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class Meeting : AuditableEntity
    {
        public long MeetingNumber { get; set; }
        public string PassCode { get; set; }
        public Guid LiveSessionId { get; set; }
        public LiveSession LiveSession { get; set; }
        public User User { get; set; }
        public IList<LiveSessionReport> LiveSessionReports { get; set; }
    }
}