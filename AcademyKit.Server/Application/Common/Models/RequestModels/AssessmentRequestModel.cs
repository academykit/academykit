namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class AssessmentRequestModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Retakes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public int Weightage { get; set; }
        public int? PassPercentage { get; set; }
        public IList<SkillsCriteriaRequestModel> SkillsCriteriaRequestModels { get; set; }
        public IList<EligibilityCreationRequestModel> EligibilityCreationRequestModels { get; set; }
    }
}
