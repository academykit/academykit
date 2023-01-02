using System.Runtime.ConstrainedExecution;
namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class MeetingReportResponseModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string JoinedTime { get; set; } = string.Empty;
        public string LeftTime { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public Guid LessonId { get; set; }
    }
}