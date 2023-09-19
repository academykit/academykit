namespace Lingtren.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    using Lingtren.Domain.Enums;
    public class StorageTypeRequestModel
    {
        [Required]
        public StorageType Type { get; set; }
    }
}
