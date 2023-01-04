namespace Lingtren.Application.Common.Dtos
{
    using System.ComponentModel.DataAnnotations;
    public class VerifyResetTokenModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
