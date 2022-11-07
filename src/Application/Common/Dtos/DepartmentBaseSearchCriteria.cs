namespace Lingtren.Application.Common.Dtos
{
    public class DepartmentBaseSearchCriteria : BaseSearchCriteria
    {
        public bool IsActive { get; set; } = true;
    }
}
