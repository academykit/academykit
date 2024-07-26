namespace AcademyKit.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    using AcademyKit.Domain.Enums;

    public class StorageTypeRequestModel
    {
        [Required]
        public StorageType Type { get; set; }
    }
}
