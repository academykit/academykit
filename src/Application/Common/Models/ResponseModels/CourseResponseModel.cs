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
        public CourseStatus Status { get; set; }
        public Language Language { get; set; }
        public int Duration { get; set; }
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public IList<CourseTagResponseModel> Tags { get; set; }
        public IList<SectionResponseModel> Sections { get; set; }
        public UserModel User { get; set; }

        public CourseResponseModel(Course model, bool fetchSection = false)
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
            Tags = new List<CourseTagResponseModel>();
            Sections = new List<SectionResponseModel>();
            User = model.User != null ? new UserModel(model.User) : new UserModel();
            model.CourseTags.ToList().ForEach(item => Tags.Add(new CourseTagResponseModel(item)));
            if (fetchSection)
            {
                model.Sections.ToList().ForEach(item => Sections.Add(new SectionResponseModel(item,fetchLesson:true)));
            }
        }
    }

    public class CourseTagResponseModel
    {
        public Guid Id { get; set; }
        public Guid TagId { get; set; }
        public string TagName { get; set; }
        public CourseTagResponseModel(CourseTag courseTag)
        {
            Id = courseTag.Id;
            TagId = courseTag.TagId;
            TagName = courseTag.Tag.Name;
        }
    }
}
