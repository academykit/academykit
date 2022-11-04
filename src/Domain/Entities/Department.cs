namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class Department : AuditableEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
        public IList<User> Users { get; set; }
    }
}
