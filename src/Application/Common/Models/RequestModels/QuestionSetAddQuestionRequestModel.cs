namespace Lingtren.Application.Common.Models.RequestModels
{
    public class QuestionSetAddQuestionRequestModel
    {
        public List<Guid> QuestionPoolQuestionIds { get; set; }

        public QuestionSetAddQuestionRequestModel()
        {
            QuestionPoolQuestionIds = new List<Guid>();
        }
    }
}
