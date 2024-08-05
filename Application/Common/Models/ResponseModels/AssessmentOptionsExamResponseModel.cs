namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class AssessmentOptionsExamResponseModel
    {
        public Guid OptionId { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }
        public bool? IsCorrect { get; set; } = null;
    }
}
