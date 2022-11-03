namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;
    public class LiveSession : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EventType EventType { get; set; }
        public string Recurrence { get; set; }
        public Status Status { get; set; }
        public int Duration { get; set; }
        public DateTime? NearestStartTime { get; set; }
        public DateTime? NearestEndTime { get; set; }
        public User User { get; set; }
        public IList<LiveSessionModerator> LiveSessionModerators { get; set; }
        public IList<LiveSessionTag> LiveSessionTags { get; set; }
        public IList<Meeting> Meetings { get; set; }
        public IList<LiveSessionReport> LiveSessionReports { get; set; }
        public IList<LiveSessionMember> LiveSessionMembers { get; set; }
    }
}