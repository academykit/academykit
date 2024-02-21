namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssessmentSubmissionRequestModel
    {
        public Guid AssessmentQuestionId { get; set; }
        public List<Guid> Answers { get; set; }
    }
}
