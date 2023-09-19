using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Dtos
{
    public class EnrollmentBaseSearchCriteria : BaseSearchCriteria
    {
        public string CourseIdentity { get; set; }

        public EnrollmentMemberStatusEnum EnrollmentStatus { get; set; }
    }
}
