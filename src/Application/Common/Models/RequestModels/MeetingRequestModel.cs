namespace Lingtren.Application.Common.Models.RequestModels
{
    public class MeetingRequestModel
    {
        public DateTime MeetingStartDate { get; set; }
        public int MeetingDuration { get; set; }
        public Guid? ZoomLicenseId { get; set; }
    }
}
