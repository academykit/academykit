namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class GroupResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
        public int CourseCount { get; set; }
        public UserModel User { get; set; }
        public GroupResponseModel(Group model, int memberCount = 0, int courseCount = 0)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            IsActive = model.IsActive;
            MemberCount = memberCount;
            CourseCount = courseCount;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
