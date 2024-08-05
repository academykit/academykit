namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class FeedbackSubmissionStudentResponseModel
    {
        public UserModel User { get; set; }
        public Guid LessonId { get; set; }
        public string LessonSlug { get; set; }
    }
}
