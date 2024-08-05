namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class WatchHistoryResponseModel
    {
        public Guid Id { get; set; }
        public string CourseSlug { get; set; }
        public string CourseName { get; set; }
        public string CourseThumbnail { get; set; }
        public string LessonSlug { get; set; }
        public string LessonName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime WatchedDate { get; set; }
        public UserModel Teacher { get; set; }
    }
}
