namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class DashboardResponseModel
    {
        public int TotalUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalGroups { get; set; }
        public int TotalTrainers { get; set; }
        public int TotalTrainings { get; set; }
        public int TotalActiveTrainings { get; set; }
        public int TotalCompletedTrainings { get; set; }
        public int TotalEnrolledCourses { get; set; }
        public int TotalInProgressCourses { get; set; }
        public int TotalCompletedCourses { get; set; }
        
    }
}
