namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class GroupMemberResponseModel
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
        public UserModel User { get; set; }

        public GroupMemberResponseModel(GroupMember model)
        {
            Id = model.Id;
            GroupId = model.GroupId;
            GroupName = model.Group.Name;
            IsActive = model.IsActive;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
