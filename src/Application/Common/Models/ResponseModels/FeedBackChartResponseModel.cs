namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Enums;

    public class FeedBackChartResponseModel
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string LessonName { get; set; }
        public Guid FeedbackId { get; set; }
        public string FeedbackName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public FeedbackTypeEnum Type { get; set; }
        public int SingleChoiceCount { get; set; }
        public int MultipleChoiceCount { get; set; }
        public int RatingCount { get; set; }
        public int AnswerCount { get; set; }
        public UserModel User { get; set; }
        public IList<FeedBackAnswerResponseModel> SubjectiveAnswer { get; set; }
        public FeedbackRatingResponseModel Rating { get; set; }
        public IList<FeedBackChartOptionsResponseModel> FeedbackQuestionOptions { get; set; }

        public FeedBackChartResponseModel() { }
    }
}
