namespace Lingtren.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class UserImportRequestModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
