using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AiKeyResponseModel
    {
        public string Key { get; set; }
        public bool? IsActive { get; set; }
        public AiModelEnum? AiModel { get; set; }

        public AiKeyResponseModel(AIKey model)
        {
            Key = model?.Key == null ? "" : model.Key;
            IsActive = model?.IsActive;
            AiModel = model?.AiModel;
        }
    }
}
