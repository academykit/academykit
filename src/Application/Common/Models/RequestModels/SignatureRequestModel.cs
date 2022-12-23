namespace Lingtren.Application.Common.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    public class SignatureRequestModel
    {
        [Required]
        public string CourseIdentity { get; set; }

        [Required]
        public IList<SignatureFileRequestModel> Signatures { get; set; }
    }
}
