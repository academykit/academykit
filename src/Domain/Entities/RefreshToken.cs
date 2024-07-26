namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    public class RefreshToken : AuditableEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime LoginAt { get; set; }
        public string DeviceId { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
    }
}
