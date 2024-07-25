namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class DepartmentResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserModel User { get; set; }

        public DepartmentResponseModel(Department model)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            IsActive = model.IsActive;
            UpdatedOn = model.UpdatedOn;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
