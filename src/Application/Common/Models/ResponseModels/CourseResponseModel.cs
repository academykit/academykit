namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    public class CourseResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public Status Status { get; set; }
        public Language Language { get; set; }
        public int Duration { get; set; }
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public UserModel User { get; set; }

        public CourseResponseModel(Course model)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            ThumbnailUrl = model.ThumbnailUrl;
            Description = model.Description;
            GroupId = model.GroupId;
            GroupName = model.Group?.Name;
            Status = model.Status;
            Language = model.Language;
            Duration = model.Duration;
            LevelId = model.LevelId;
            LevelName = model.Level?.Name;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
