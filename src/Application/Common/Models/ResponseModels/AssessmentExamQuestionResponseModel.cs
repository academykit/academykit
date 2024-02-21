namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Enums;

    public class AssessmentExamQuestionResponseModel
    {
        public Guid QuestionId { get; set; }
        public string QuestionName { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public IList<AssessmentOptionsExamResponseModel> assessmentQuestionOptions { get; set; }
    }
}
