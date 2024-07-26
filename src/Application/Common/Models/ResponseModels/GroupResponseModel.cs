namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;

    public class GroupResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? MemberCount { get; set; }
        public int? CourseCount { get; set; }
        public int? AttachmentCount { get; set; }
        public UserModel User { get; set; }

        public GroupResponseModel(Group model)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            IsActive = model.IsActive;
            MemberCount = model.GroupMembers?.Count(x => x.IsActive);
            CourseCount = model.Courses?.Count(x =>
                x.Status == CourseStatus.Published
                || x.Status == CourseStatus.Completed
                || x.IsUpdate
            );
            AttachmentCount = model.GroupFiles?.Count;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
