namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Enums;

    public class StorageSettingResponseModel
    {
        public StorageType Type { get; set; }
        public IList<SettingValue> Values { get; set; }
        public bool IsActive { get; set; }
    }
}
