using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Dtos
{
    public class TagBaseSearchCriteria : BaseSearchCriteria
    {
        /// <summary>
        /// identity of training
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// type of training
        /// </summary>
        public TrainingTypeEnum TrainingType { get; set; }
    }
}
