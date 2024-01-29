namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class FeedBackChartOptionsResponseModel
    {
        public Guid Id { get; set; }
        public Guid FeedbackId { get; set; }
        public string FeedbackName { get; set; }
        public string Option { get; set; }
        public bool? IsSelected { get; set; }
        public int Order { get; set; }
        public int SelectedCount { get; set; }

        public FeedBackChartOptionsResponseModel(FeedbackQuestionOption model)
        {
            Id = model.Id;
            FeedbackId = model.FeedbackId;
            FeedbackName = model.Feedback?.Name;
            Option = model.Option;
            Order = model.Order;
            SelectedCount = 1;
        }

        public FeedBackChartOptionsResponseModel() { }
    }
}
