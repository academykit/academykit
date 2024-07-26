using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class FeedbackRequestModel
    {
        public Guid LessonId { get; set; }
        public string Name { get; set; }
        public FeedbackTypeEnum Type { get; set; }
        public IList<FeedbackQuestionOptionRequestModel> Answers { get; set; }
    }

    public class FeedbackQuestionOptionRequestModel
    {
        public string Option { get; set; }
    }
}
