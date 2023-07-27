
using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Dtos
{
    public class TeacherSearchCriteria:BaseSearchCriteria
    {
        /// <summary>
        /// course id or slug of specific course
        /// </summary>
        public string? Identity {get; set; }

        /// <summary>
        ///specify type of enum 
        /// </summary>
        public TrainingTypeEnum LessonType { get; set; }
    }
}
