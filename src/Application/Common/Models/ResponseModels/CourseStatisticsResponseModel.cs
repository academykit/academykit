namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class CourseStatisticsResponseModel
    {
        public int TotalLessons { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalAssignments { get; set; }
        public int TotalLectures { get; set; }
        public int TotalExams { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalDocuments { get; set; }
        public (
            string LessonSlug,
            string Passcode,
            DateTime? StartDate,
            Guid ZoomId
        ) Meetings1 { get; set; }
        public IEnumerable<MeetingDashboardResponseModel> MeetingsList { get; set; }
    }

    public class MeetingDashboardResponseModel
    {
        public string LessonSlug { get; set; }
        public string Passcode { get; set; }
        public DateTime? StartDate { get; set; }
        public Guid ZoomId { get; set; }
    }
}
