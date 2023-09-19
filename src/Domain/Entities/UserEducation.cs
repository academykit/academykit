namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class UserEducation : AuditableEntity
    {
        public required string InstitutionName { get; set; }

        public required string Degree { get; set; }

        public required string Specialization { get; set; }

        public DateOnly CompletionDate { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
