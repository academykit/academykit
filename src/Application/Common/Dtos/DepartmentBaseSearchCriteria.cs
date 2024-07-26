namespace AcademyKit.Application.Common.Dtos
{
    public class DepartmentBaseSearchCriteria : BaseSearchCriteria
    {
        public bool? IsActive { get; set; }
        public string departmentName { get; set; }
    }
}
