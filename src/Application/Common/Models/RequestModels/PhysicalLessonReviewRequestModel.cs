namespace Lingtren.Application.Common.Models.RequestModels
{
    public class PhysicalLessonReviewRequestModel
    {
        public string Identity { get; set; }
        public bool IsPassed { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
    }
}