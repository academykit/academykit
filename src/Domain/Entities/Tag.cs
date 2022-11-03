namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Entities;
    public class Tag : AuditableEntity
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
    }
}