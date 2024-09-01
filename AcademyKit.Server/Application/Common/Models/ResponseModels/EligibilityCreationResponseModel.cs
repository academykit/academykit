namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;

    public class EligibilityCreationResponseModel
    {
        public Guid Id { get; set; }
        public UserRole Role { get; set; }
        public Guid? SkillId { get; set; }
        public string SkillName { get; set; }
        public Guid? AssessmentId { get; set; }
        public string AssessmentName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Guid? TrainingId { get; set; }
        public string TrainingName { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsEligible { get; set; }

        public EligibilityCreationResponseModel() { }

        public EligibilityCreationResponseModel(EligibilityCreation model)
        {
            Id = model.Id;
            Role = model.Role;
            SkillId = model.SkillId;
            SkillName = model.Skills?.SkillName;
            AssessmentId = model.CompletedAssessmentId;
            AssessmentName = model.CompletedAssessment?.Title;
            DepartmentId = model.DepartmentId;
            DepartmentName = model.Department?.Name;
            TrainingId = model.TrainingId;
            TrainingName = model.Course?.Name;
            GroupId = model.GroupId;
            GroupName = model.Group?.Name;
        }
    }
}
