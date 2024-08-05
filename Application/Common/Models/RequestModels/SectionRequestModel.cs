using System.ComponentModel.DataAnnotations;

namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class SectionRequestModel
    {
        [Required(ErrorMessage = " Section name is Required")]
        public string Name { get; set; } = string.Empty;
    }
}
