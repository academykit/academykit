namespace Lingtren.Application.Common.Dtos
{
    using System.ComponentModel.DataAnnotations;
    public class ResetPasswordRequestModel
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string PasswordChangeToken { get; set; }
    }
}
