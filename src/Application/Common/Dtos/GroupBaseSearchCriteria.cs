namespace AcademyKit.Application.Common.Dtos
{
    using AcademyKit.Domain.Enums;

    public class GroupBaseSearchCriteria : BaseSearchCriteria
    {
        public UserRole Role { get; set; }
        public string DepartmentIdentity { get; set; }
    }
}
