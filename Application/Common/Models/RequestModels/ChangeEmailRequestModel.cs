namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class ChangeEmailRequestModel
    {
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string ConfirmEmail { get; set; }
        public string Password { get; set; }
    }
}
