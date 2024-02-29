using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public class AiKeyRequestModel
    {
        public string Key { get; set; }
        public bool IsActive { get; set; }
        public AiModelEnum AiModel { get; set; }
    }
}
