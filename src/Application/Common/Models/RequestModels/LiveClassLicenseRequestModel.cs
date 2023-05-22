using System.ComponentModel.DataAnnotations;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public class LiveClassLicenseRequestModel
    {
       
        public  string? LessonIdentity { get; set; }
        [Required]
        public DateTime StartDateTime { get; set; }
        [Required]
        public int Duration { get; set; }
    }
}
