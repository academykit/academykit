namespace Lingtren.Application.Common.Dtos
{
    using Lingtren.Domain.Enums;

    public class UserSearchCriteria : BaseSearchCriteria
    {
        public bool IsActive { get; set; } = true;
        public UserRole? Role { get; set; }
    }
}
