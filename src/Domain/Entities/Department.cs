namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class Department : AuditableEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
