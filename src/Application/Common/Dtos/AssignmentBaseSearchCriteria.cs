namespace AcademyKit.Application.Common.Dtos
{
    public class AssignmentBaseSearchCriteria : BaseSearchCriteria
    {
        public string LessonIdentity { get; set; }
        public Guid? UserId { get; set; }
    }
}
