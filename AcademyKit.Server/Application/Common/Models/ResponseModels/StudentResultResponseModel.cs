namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class StudentResultResponseModel
    {
        public int AttemptCount { get; set; }
        public UserModel User { get; set; }
        public IList<QuestionSetResultDetailModel> QuestionSetSubmissions { get; set; }
        public bool HasExceededAttempt { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class QuestionSetResultDetailModel
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
