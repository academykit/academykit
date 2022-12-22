namespace Lingtren.Application.Common.Models.RequestModels
{
    public class FeedbackSubmissionRequestModel
    {
        public Guid Id { get; set; }
        public Guid FeedbackId { get; set; }
        public List<Guid> SelectedOption { get; set; }
        public string Answer { get; set; }
        public int? Rating { get; set; }
    }
}
