namespace Lingtren.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    public class LevelRequestModel
    {
        [Required]
        public string Name { get; set; }
    }
}