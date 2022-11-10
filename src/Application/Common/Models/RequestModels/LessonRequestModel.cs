namespace Lingtren.Application.Common.Models.RequestModels
{
    using Lingtren.Domain.Enums;

    public class LessonRequestModel
    {
        public string SectionIdentity { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string DocumentUrl { get; set; }
        public int Order { get; set; }
        public bool IsPreview { get; set; }
        public LessonType Type { get; set; }
        public DateTime MeetingStartDate { get; set; }
        public int MeetingDuration { get; set; }
        public Guid? ZoomLicenseId { get; set; }

    }
}
