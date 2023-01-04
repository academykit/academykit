namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class QuestionSetSubmissionResponseModel
    {
        public Guid Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public int Duration { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<QuestionResponseModel> Questions { get; set; }
    }
}