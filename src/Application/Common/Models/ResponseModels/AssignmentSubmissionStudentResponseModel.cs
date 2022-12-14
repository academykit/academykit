namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssignmentSubmissionStudentResponseModel
    {
        public UserModel User { get; set; }
        public Guid LessonId { get; set; }
        public string LessonSlug { get; set; }
    }
}
