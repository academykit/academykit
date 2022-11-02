namespace Lingtren.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime LoginAt { get; set; }
        public string DeviceId { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
    }
}
