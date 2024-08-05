namespace AcademyKit.Application.Common.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class ForgotPasswordRequestModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
