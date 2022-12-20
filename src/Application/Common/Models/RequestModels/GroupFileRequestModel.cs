namespace Lingtren.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class GroupFileRequestModel
    {
        [Required]
        public string GroupIdentity { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}