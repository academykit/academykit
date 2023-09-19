namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class MeetingReportResponseModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public DateTime StartDate { get; set; }
        public string JoinedTime { get; set; }
        public string LeftTime { get; set; }
        public int Duration { get; set; }
        public Guid LessonId { get; set; }
    }
}
