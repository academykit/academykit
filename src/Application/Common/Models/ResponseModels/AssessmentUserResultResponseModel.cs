namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Enums;

    public class AssessmentUserResultResponseModel
    {
        public Guid QuestionSetSubmissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal NegativeMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public UserModel Teacher { get; set; }
        public UserModel User { get; set; }
        public string Duration { get; set; }
        public string CompleteDuration { get; set; }
        public IList<AssessmentAnswerResultModel> Results { get; set; }
    }

    public class AssessmentAnswerResultModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public string Attachments { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public IList<AssessmentQuestionResultOption> QuestionOptions { get; set; }
        public bool IsCorrect { get; set; }
        public int? OrderNumber { get; set; }
    }

    public class AssessmentQuestionResultOption
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}
