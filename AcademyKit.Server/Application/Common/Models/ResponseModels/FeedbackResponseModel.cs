using AcademyKit.Domain.Entities;

namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Domain.Enums;

    public class FeedbackResponseModel
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string LessonName { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public FeedbackTypeEnum Type { get; set; }
        public UserModel User { get; set; }
        public Guid? FeedbackSubmissionId { get; set; }
        public string Answer { get; set; }
        public int? Rating { get; set; }
        public UserModel Student { get; set; }
        public IList<FeedbackQuestionOptionResponseModel> FeedbackQuestionOptions { get; set; }
        public bool IsTrainee { get; set; }

        public FeedbackResponseModel() { }

        public FeedbackResponseModel(Feedback feedback)
        {
            Id = feedback.Id;
            LessonId = feedback.LessonId;
            LessonName = feedback.Lesson?.Name;
            Name = feedback.Name;
            Order = feedback.Order;
            IsActive = feedback.IsActive;
            Type = feedback.Type;
            User = feedback.User != null ? new UserModel(feedback.User) : new UserModel();
            FeedbackQuestionOptions = new List<FeedbackQuestionOptionResponseModel>();
            if (
                feedback.Type == FeedbackTypeEnum.SingleChoice
                || feedback.Type == FeedbackTypeEnum.MultipleChoice
            )
            {
                feedback
                    .FeedbackQuestionOptions?.ToList()
                    .ForEach(item =>
                        FeedbackQuestionOptions.Add(new FeedbackQuestionOptionResponseModel(item))
                    );
            }
        }
    }
}
