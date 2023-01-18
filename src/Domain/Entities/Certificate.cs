namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class Certificate : AuditableEntity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string Institute { get; set; }
        public int? Duration { get; set; }
        public User User { get; set; }
        public bool IsVerified { get; set; }
    }
}