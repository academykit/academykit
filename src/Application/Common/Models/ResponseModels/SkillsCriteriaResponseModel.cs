namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;

    public class SkillsCriteriaResponseModel
    {
        public Guid Id { get; set; }
        public decimal Percentage { get; set; }
        public Guid? SkillId { get; set; }
        public SkillAssessmentRule SkillAssessmentRule { get; set; }

        public string SkillTypeName { get; set; }
        public bool IsEligible { get; set; }

        public SkillsCriteriaResponseModel(SkillsCriteria model)
        {
            Id = model.Id;
            SkillAssessmentRule = model.SkillAssessmentRule;
            Percentage = model.Percentage;
            SkillId = model.SkillId;
            SkillTypeName = model.SkillType.SkillName;
        }
    }
}
