namespace AcademyKit.Application.Common.Dtos
{
    public class LessonBaseSearchCriteria : BaseSearchCriteria
    {
        public string CourseIdentity { get; set; }
        public string SectionIdentity { get; set; }
    }
}
