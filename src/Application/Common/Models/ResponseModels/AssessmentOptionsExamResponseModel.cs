namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class AssessmentOptionsExamResponseModel
    {
        public Guid OptionId { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }
        public bool? IsCorrect { get; set; } = null;
    }
}
