namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class GroupResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public UserModel User { get; set; }
        public GroupResponseModel(Group model)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            IsActive = model.IsActive;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
