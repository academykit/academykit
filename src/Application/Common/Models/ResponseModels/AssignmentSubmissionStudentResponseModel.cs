namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssignmentSubmissionStudentResponseModel
    {
        public UserModel User { get; set; }
        public Guid LessonId { get; set; }
        public string LessonSlug { get; set; }
        public AssignmentReviewResponseModel AssignmentReview { get; set; }
        public IList<AssignmentResponseModel> Assignments { get; set; }
    }
    public class AssignmentReviewResponseModel
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public Guid UserId { get; set; }
        public string Mark { get; set; }
        public string Review { get; set; }
        public UserModel User { get; set; }
        public UserModel? Teacher { get; set; }
    }
}
