namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class AssessmentQuestionOptionResponseModel
    {
        public Guid Id { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }

        public bool IsCorrect { get; set; }

        public AssessmentQuestionOptionResponseModel(AssessmentOptions model)
        {
            Id = model.Id;
            Option = model.Option;
            Order = model.Order;
            IsCorrect = model.IsCorrect;
        }

        public AssessmentQuestionOptionResponseModel() { }
    }
}
