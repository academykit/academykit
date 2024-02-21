namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class Department : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
        public IList<User> Users { get; set; }
        public IList<EligibilityCreation> EligibilityCreations { get; set; }
    }
}
