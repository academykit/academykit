namespace Lingtren.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    using Lingtren.Application.Common.Dtos;
    using Microsoft.AspNetCore.Http;

    public class MediaRequestModel
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public MediaType Type { get; set; }
    }
}