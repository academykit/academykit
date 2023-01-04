namespace Lingtren.Application.Common.Dtos
{
    public class FeedbackBaseSearchCriteria : BaseSearchCriteria
    {
        public string LessonIdentity { get; set; }
        public Guid? UserId { get; set; }
    }
}
