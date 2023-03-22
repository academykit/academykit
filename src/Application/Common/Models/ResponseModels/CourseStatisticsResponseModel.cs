using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
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
        public List<Lesson> LiveSessionStats { get; set; }

    }
}