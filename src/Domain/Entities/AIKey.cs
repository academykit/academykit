using Lingtren.Domain.Common;

namespace Lingtren.Domain.Entities
{
    public class AIKey : AuditableEntity
    {
        public string Key { get; set; }
        public bool IsActive { get; set; }
    }
}
