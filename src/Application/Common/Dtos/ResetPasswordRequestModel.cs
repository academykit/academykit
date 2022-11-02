namespace Lingtren.Application.Common.Dtos
{
    using System.ComponentModel.DataAnnotations;
    public class ResetPasswordRequestModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public string PasswordChangeToken { get; set; }
    }
}
