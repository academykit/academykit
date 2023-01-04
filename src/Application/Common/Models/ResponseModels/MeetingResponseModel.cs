namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class MeetingResponseModel
    {
        public Guid Id { get; set; }
        public long? MeetingNumber { get; set; }
        public string Passcode { get; set; }
        public Guid ZoomLicenseId { get; set; }
        public int Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public UserModel User { get; set; }
        public MeetingResponseModel(Meeting model, bool showPasscode = false)
        {
            Id = model.Id;
            ZoomLicenseId = model.ZoomLicenseId;
            MeetingNumber = showPasscode ? model.MeetingNumber : 0;
            Passcode = showPasscode ? model.Passcode : string.Empty;
            Duration = model.Duration;
            StartDate = model.StartDate;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }

    public class MeetingJoinResponseModel
    {
        public string Slug { get; set; }
        public string RoomName { get; set; }

        /// <summary>
        /// Get or set zoom signature jwt token
        /// </summary>
        public string JwtToken { get; set; }

        /// <summary>
        /// Get or set zoom zak token
        /// </summary>
        public string ZAKToken { get; set; }
        public string SdkKey { get; set; }
        public long? MeetingId { get; set; }
        public string Passcode { get; set; }
        public UserModel User { get; set; }
    }
}
