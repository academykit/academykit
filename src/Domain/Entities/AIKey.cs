using AcademyKit.Domain.Common;
using AcademyKit.Domain.Enums;

namespace AcademyKit.Domain.Entities
{
    public class AIKey : AuditableEntity
    {
        public string Key { get; set; }
        public AiModelEnum AiModel { get; set; }
        public bool IsActive { get; set; }
    }
}
