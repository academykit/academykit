namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class MeetingResponseModel
    {
        public Guid Id { get; set; }
        public long MeetingNumber { get; set; }
        public string PassCode { get; set; }
        public Guid ZoomLicenseId { get; set; }
        public int Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public UserModel User { get; set; }
        public MeetingResponseModel(Meeting model, bool showPasscode = false)
        {
            Id = model.Id;
            ZoomLicenseId = model.ZoomLicenseId;
            MeetingNumber = showPasscode ? model.MeetingNumber : 0;
            PassCode = showPasscode ? model.PassCode : string.Empty;
            Duration = model.Duration;
            StartDate = model.StartDate;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
