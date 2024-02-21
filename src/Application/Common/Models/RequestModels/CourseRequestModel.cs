namespace Lingtren.Application.Common.Models.RequestModels
{
    using Lingtren.Domain.Enums;

    public class CourseRequestModel
    {
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public Guid? GroupId { get; set; }
        public Language Language { get; set; }
        public int Duration { get; set; }
        public Guid LevelId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsUnlimitedEndDate { get; set; }

        public IList<Guid> TagIds { get; set; } = new List<Guid>();
        public IList<TrainingEligibilityCriteriaRequestModel> TrainingEligibilities { get; set; }
    }

    public class TrainingEligibilityCriteriaRequestModel
    {
        public TrainingEligibilityEnum Eligibility { get; set; }
        public Guid? EligibilityId { get; set; }
    }
}
