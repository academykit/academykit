namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class StudentAssessmentResultResponseModel
    {
        public int AttemptCount { get; set; }
        public UserModel User { get; set; }
        public IList<AssessmentSetResultDetailModel> AssessmentSetResultDetails { get; set; }
        public bool HasExceededAttempt { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class AssessmentSetResultDetailModel
    {
        public Guid QuestionSetSubmissionId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string TotalMarks { get; set; }
        public string NegativeMarks { get; set; }
        public string ObtainedMarks { get; set; }
        public string Duration { get; set; }
        public string CompleteDuration { get; set; }
    }
}
