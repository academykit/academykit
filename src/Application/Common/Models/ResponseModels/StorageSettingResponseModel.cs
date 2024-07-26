namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Domain.Enums;

    public class StorageSettingResponseModel
    {
        public StorageType Type { get; set; }
        public IList<SettingValue> Values { get; set; }
        public bool IsActive { get; set; }
    }
}
