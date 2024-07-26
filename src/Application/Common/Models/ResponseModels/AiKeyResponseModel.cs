using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Models.ResponseModels
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
