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
}
