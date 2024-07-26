namespace AcademyKit.Application.Common.Dtos
{
    public class AssessmentQuestionBaseSearchCriteria : BaseSearchCriteria
    {
        public Guid? AssessmentIdentity { get; set; }
        public string AssessmentSlug { get; set; }
        public Guid? UserId { get; set; }
    }
}
