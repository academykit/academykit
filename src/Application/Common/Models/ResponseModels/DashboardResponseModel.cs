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

    public class DashboardCourseResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public decimal? Percentage { get; set; }
        public UserModel User { get; set; }
        public IList<UserModel>? Students { get; set; }
    }
}
