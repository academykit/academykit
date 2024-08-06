namespace AcademyKit.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    using AcademyKit.Domain.Enums;

    public class StorageSettingRequestModel
    {
        [Required]
        public StorageType Type { get; set; }

        [Required]
        public IList<SettingValue> Values { get; set; }
    }
}
