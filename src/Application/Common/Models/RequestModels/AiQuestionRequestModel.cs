namespace Lingtren.Application.Common.Models.RequestModels
{
    public class AiQuestionRequestModel
    {
        public List<QuestionData> Questions { get; set; }
    }

    public class QuestionData
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string Answer { get; set; }
    }
}
