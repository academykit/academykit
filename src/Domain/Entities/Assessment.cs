namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class Assessment : AuditableEntity
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Retakes { get; set; }
        public AssessmentStatus AssessmentStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public int Weightage { get; set; }
        public string Message { get; set; }
        public IList<EligibilityCreation> EligibilityCreationsCompleted { get; set; }
        public IList<SkillsCriteria> SkillsCriteria { get; set; }
        public IList<AssessmentQuestion> AssessmentQuestions { get; set; }
        public IList<AssessmentSubmission> AssessmentSubmissions { get; set; }
        public IList<EligibilityCreation> EligibilityCreations { get; set; }
        public IList<AssessmentResult> AssessmentResults { get; set; }
        public User User { get; set; }
    }
}
