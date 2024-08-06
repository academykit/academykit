namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;

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
        public CourseEnrollmentStatus? UserStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsUnlimitedEndDate { get; set; }

        public UserModel User { get; set; }
        public IList<CourseTagResponseModel> Tags { get; set; }
        public IList<SectionResponseModel> Sections { get; set; }
        public decimal? Percentage { get; set; }
        public bool IsEligible { get; set; }

        public IList<TrainingEligibilityCriteriaResponseModel> TrainingEligibilities { get; set; }

        public CourseResponseModel(
            Course model,
            CourseEnrollmentStatus? userStatus,
            bool isEligible = false,
            bool fetchSection = false
        )
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
            UserStatus = userStatus;
            CreatedOn = model.CreatedOn;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            IsUnlimitedEndDate = model.IsUnlimitedEndDate;
            IsEligible = isEligible;
            Tags = new List<CourseTagResponseModel>();
            Sections = new List<SectionResponseModel>();
            User = model.User != null ? new UserModel(model.User) : new UserModel();
            TrainingEligibilities = new List<TrainingEligibilityCriteriaResponseModel>(); // Initialize TrainingEligibilities

            if (model.CourseTags != null)
            {
                model
                    .CourseTags.ToList()
                    .ForEach(item => Tags.Add(new CourseTagResponseModel(item)));
            }
            if (fetchSection)
            {
                model
                    .Sections.ToList()
                    .ForEach(item =>
                        Sections.Add(new SectionResponseModel(item, fetchLesson: true))
                    );
            }
            if (model.TrainingEligibilities != null)
            {
                model
                    .TrainingEligibilities.ToList()
                    .ForEach(item =>
                        TrainingEligibilities.Add(
                            new TrainingEligibilityCriteriaResponseModel(item)
                        )
                    );
            }
        }

        public CourseResponseModel() { }
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
            TagName = courseTag.Tag?.Name;
        }
    }

    public class TrainingEligibilityCriteriaResponseModel
    {
        public Guid Id { get; set; }

        public TrainingEligibilityEnum Eligibility { get; set; }
        public Guid? EligibilityId { get; set; }

        public TrainingEligibilityCriteriaResponseModel(TrainingEligibility model)
        {
            Id = model.Id;
            Eligibility = model.TrainingEligibilityEnum;
            EligibilityId = model.EligibilityId;
        }
    }
}
