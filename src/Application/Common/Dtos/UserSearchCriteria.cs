namespace Lingtren.Application.Common.Dtos
{
    using Lingtren.Domain.Enums;

    public class UserSearchCriteria : BaseSearchCriteria
    {
        public UserStatus? Status { get; set; }
        public UserRole? Role { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
