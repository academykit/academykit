namespace Lingtren.Application.Common.Models.RequestModels
{
    public class WatchHistoryRequestModel
    {
        public string CourseIdentity { get; set; }
        public string LessonIdentity { get; set; }
        public int WatchedPercentage { get; set; }
    }
}
