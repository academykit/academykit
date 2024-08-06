using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Dtos
{
    public class EnrollmentBaseSearchCriteria : BaseSearchCriteria
    {
        public string CourseIdentity { get; set; }

        public EnrollmentMemberStatusEnum EnrollmentStatus { get; set; }
    }
}
