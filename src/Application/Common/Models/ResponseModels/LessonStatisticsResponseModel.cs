using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class LessonStatisticsResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public string CourseSlug { get; set; }
        public string CourseName { get; set; }
        public Guid SectionId { get; set; }
        public string SectionSlug { get; set; }
        public string SectionName { get; set; }
        public int EnrolledStudent { get; set; }
        public int LessonWatched { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class LessonStudentResponseModel
    {
        public Guid LessonId { get; set; }
        public string LessonSlug { get; set; }
        public string LessonName { get; set; }
        public LessonType LessonType { get; set; }
        public Guid? QuestionSetId { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsPassed { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserModel User { get; set; }
    }
}
