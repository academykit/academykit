using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public class SkillsCriteriaRequestModel
    {
        public SkillAssessmentRule SkillAssessmentRule { get; set; }
        public decimal Percentage { get; set; }
        public Guid? SkillId { get; set; }
    }
}
