namespace AcademyKit.Application.Common.Dtos
{
    public class ResetPasswordRequestModel
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string PasswordChangeToken { get; set; }
    }
}
