using AcademyKit.Domain.Entities;

namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Domain.Enums;

    public class AssessmentQuestionResponseModel
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }
        public string AssessmentName { get; set; }
        public string QuestionName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }

        public string Hints { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public UserModel User { get; set; }
        public IList<AssessmentQuestionOptionResponseModel> assessmentQuestionOptions { get; set; }

        public AssessmentQuestionResponseModel() { }

        public AssessmentQuestionResponseModel(AssessmentQuestion assessmentQuestion)
        {
            Id = assessmentQuestion.Id;
            AssessmentId = assessmentQuestion.AssessmentId;
            AssessmentName = assessmentQuestion.Assessment?.Title;
            QuestionName = assessmentQuestion.Name;
            Order = assessmentQuestion.Order;
            IsActive = assessmentQuestion.IsActive;
            Type = assessmentQuestion.Type;
            Description = assessmentQuestion.Description;
            Hints = assessmentQuestion.Hints;
            User =
                assessmentQuestion.User != null
                    ? new UserModel(assessmentQuestion.User)
                    : new UserModel();
            assessmentQuestionOptions = new List<AssessmentQuestionOptionResponseModel>();
            if (
                assessmentQuestion.Type == AssessmentTypeEnum.SingleChoice
                || assessmentQuestion.Type == AssessmentTypeEnum.MultipleChoice
            )
            {
                assessmentQuestion
                    .AssessmentOptions?.ToList()
                    .ForEach(item =>
                        assessmentQuestionOptions.Add(
                            new AssessmentQuestionOptionResponseModel(item)
                        )
                    );
            }
        }
    }
}
