namespace Lingtren.Domain.Entities
{
    public class MeetingReport
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime JoinTime { get; set; }
        public DateTime? LeftTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
