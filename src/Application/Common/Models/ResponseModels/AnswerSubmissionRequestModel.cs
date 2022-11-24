namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AnswerSubmissionRequestModel
    {
        public Guid QuestionSetQuestionId { get; set; }
        public List<Guid> Answers { get; set; }
    }
}
