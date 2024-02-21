using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssessmentResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Retakes { get; set; }
        public int RemainingAttempt { get; set; }
        public bool HasCompleted { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public int Weightage { get; set; }
        public int NoOfQuestion { get; set; }
        public AssessmentStatus AssessmentStatus { get; set; }

        public bool IsEligible { get; set; }

        public UserModel User { get; set; }
        public IList<SkillsCriteriaResponseModel> SkillsCriteriaRequestModels { get; set; }
        public IList<EligibilityCreationResponseModel> EligibilityCreationRequestModels { get; set; }

        public AssessmentResponseModel(
            Assessment model,
            bool isEligible = false,
            int existingQuestion = 0,
            bool hasCompleted = false,
            int remainingAttempt = 0
        )
        {
            Id = model.Id;
            Slug = model.Slug;
            Title = model.Title;
            Retakes = model.Retakes;
            Description = model.Description;
            RemainingAttempt = remainingAttempt;
            HasCompleted = hasCompleted;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            Duration = model.Duration * 60;
            Weightage = model.Weightage;
            NoOfQuestion = existingQuestion;
            AssessmentStatus = model.AssessmentStatus;
            SkillsCriteriaRequestModels = new List<SkillsCriteriaResponseModel>();
            EligibilityCreationRequestModels = new List<EligibilityCreationResponseModel>();
            model.EligibilityCreations
                .ToList()
                .ForEach(
                    item =>
                        EligibilityCreationRequestModels.Add(
                            new EligibilityCreationResponseModel(item)
                        )
                );
            model.SkillsCriteria
                .ToList()
                .ForEach(
                    item => SkillsCriteriaRequestModels.Add(new SkillsCriteriaResponseModel(item))
                );
            IsEligible = isEligible;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
