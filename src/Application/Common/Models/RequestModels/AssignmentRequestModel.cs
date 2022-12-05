namespace Lingtren.Application.Common.Models.RequestModels
{
    using Lingtren.Domain.Enums;

    public class AssignmentRequestModel
    {
        public Guid LessonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public IList<string> FileUrls { get; set; }
        public IList<AssignmentQuestionOptionRequestModel> Answers { get; set; }
    }
    public class AssignmentQuestionOptionRequestModel
    {
        public string Option { get; set; }
        public bool IsCorrect { get; set; }
    }
}
