namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class GroupResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public UserResponseModel User { get; set; }
        public GroupResponseModel(Group entitiy)
        {
            Id = entitiy.Id;
            Slug = entitiy.Slug;
            Name = entitiy.Name;
            IsActive = entitiy.IsActive;
            User = new UserResponseModel(entitiy.User);
        }
    }
}
