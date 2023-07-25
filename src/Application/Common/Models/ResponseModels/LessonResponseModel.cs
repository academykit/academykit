namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    public class LessonResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string DocumentUrl { get; set; }
        public int Order { get; set; }
        public int Duration { get; set; }
        public bool IsMandatory { get; set; }
        public LessonType Type { get; set; }
        public bool IsDeleted { get; set; }
        public CourseStatus Status { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public Guid SectionId { get; set; }
        public string SectionName { get; set; }
        public Guid? MeetingId { get; set; }
        public Guid? QuestionSetId { get; set; }
        public UserModel User { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsPassed { get; set; }
        public string? NextLessonSlug { get; set; }
        public bool? HasResult { get; set; }
        public bool? HasFeedbackSubmitted { get; set; }
        public bool? HasSubmittedAssigment { get; set;}
        public bool? HasReviewedAssignment { get; set; }
        public int? RemainingAttempt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool AssignmentExpired { get; set; }
        public MeetingResponseModel? Meeting { get; set; }
        public QuestionSetResponseModel? QuestionSet { get; set; }
        public AssignmentReviewResponseModel? AssignmentReview { get; set; }

        public LessonResponseModel(Lesson model)
        {
            Id = model.Id;
            Name = model.Name;
            Slug = model.Slug;
            Description = model.Description;
            VideoUrl = model.VideoUrl;
            ThumbnailUrl = model.ThumbnailUrl;
            DocumentUrl = model.DocumentUrl;
            Order = model.Order;
            IsDeleted = model.IsDeleted;
            IsMandatory = model.IsMandatory;
            Type = model.Type;
            IsDeleted = model.IsDeleted;
            Status = model.Status;
            CourseId = model.CourseId;
            CourseName = model.Course?.Name;
            SectionId = model.SectionId;
            SectionName = model.Section?.Name;
            MeetingId = model.MeetingId;
            QuestionSetId = model.QuestionSetId;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
            Meeting = model.Meeting == null ? null : new MeetingResponseModel(model.Meeting);
            QuestionSet = model.QuestionSet == null ? null : new QuestionSetResponseModel(model.QuestionSet);
        }
        public LessonResponseModel()
        {
        }
    }
}
