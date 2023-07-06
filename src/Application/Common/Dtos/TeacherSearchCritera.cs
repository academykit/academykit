
namespace Lingtren.Application.Common.Dtos
{
    public class TeacherSearchCriteria:BaseSearchCriteria
    {
        /// <summary>
        /// course id or slug of specific course
        /// </summary>
        public string CourseIdentity { get; set; }

        public string Search { get; set; }
    }
}
