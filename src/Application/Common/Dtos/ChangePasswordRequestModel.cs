namespace AcademyKit.Application.Common.Dtos
{
    public class ChangePasswordRequestModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
