namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class DepartmentResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
