using Lingtren.Domain.Common;
using Lingtren.Domain.Enums;

namespace Lingtren.Domain.Entities
{
    public class AIKey : AuditableEntity
    {
        public string Key { get; set; }
        public AiModelEnum AiModel { get; set; }
        public bool IsActive { get; set; }
    }
}
