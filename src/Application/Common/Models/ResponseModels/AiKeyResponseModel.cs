using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AiKeyResponseModel
    {
        public string Key { get; set; }
        public bool? IsActive { get; set; }

        public AiKeyResponseModel(AIKey model)
        {
            Key = model?.Key == null ? "" : model.Key;
            IsActive = model?.IsActive;
        }
    }
}
