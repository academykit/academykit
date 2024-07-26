using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class AssessmentQuestionRequestModel
    {
        public string QuestionName { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public IList<AssessmentQuestionOptionRequestModel> assessmentQuestionOptions { get; set; }
    }

    public class AssessmentQuestionOptionRequestModel
    {
        public string Option { get; set; }
        public bool IsCorrect { get; set; }
    }
}
