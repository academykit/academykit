using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class EligibilityCreationRequestModel
    {
        public UserRole Role { get; set; }
        public Guid? SkillId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? AssessmentId { get; set; }
        public Guid? TrainingId { get; set; }
    }
}
