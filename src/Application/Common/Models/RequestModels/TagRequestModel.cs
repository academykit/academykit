namespace AcademyKit.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;

    public class TagRequestModel
    {
        [Required]
        public string Name { get; set; }
    }
}
