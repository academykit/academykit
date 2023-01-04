namespace Lingtren.Application.Common.Models.RequestModels
{
    public class AssignmentSubmissionRequestModel
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public List<Guid> SelectedOption { get; set; }
        public string Answer { get; set; }
        public IList<string> AttachmentUrls { get; set; }
    }
}
