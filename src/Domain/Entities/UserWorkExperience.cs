namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class UserWorkExperience : AuditableEntity
    {
        public required string CompanyName { get; set; }

        public required string JobTitle { get; set; }

        public required string JobDescription { get; set; }

        public DateOnly JoinedDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
