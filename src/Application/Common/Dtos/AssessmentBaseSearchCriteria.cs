using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Dtos
{
    public class AssessmentBaseSearchCriteria : BaseSearchCriteria
    {
        public Guid UserId { get; set; }

        public AssessmentStatus? AssessmentStatus { get; set; }
    }
}
