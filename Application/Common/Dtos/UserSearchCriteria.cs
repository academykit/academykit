namespace AcademyKit.Application.Common.Dtos
{
    using AcademyKit.Domain.Enums;

    public class UserSearchCriteria : BaseSearchCriteria
    {
        public UserStatus? Status { get; set; }
        public UserRole? Role { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
