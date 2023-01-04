namespace Lingtren.Application.Common.Dtos
{
    using Lingtren.Domain.Enums;

    public class GroupBaseSearchCriteria : BaseSearchCriteria
    {
        public UserRole Role { get; set; }
    }
}
