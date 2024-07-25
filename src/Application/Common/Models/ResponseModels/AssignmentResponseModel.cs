namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;

    public class AssignmentResponseModel
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string LessonName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public UserModel User { get; set; }
        public Guid? AssignmentSubmissionId { get; set; }
        public string Answer { get; set; }
        public UserModel Student { get; set; }
        public bool IsTrainee { get; set; }
        public IList<AssignmentAttachmentResponseModel> AssignmentAttachments { get; set; }
        public IList<AssignmentSubmissionAttachmentResponseModel> AssignmentSubmissionAttachments { get; set; }
        public IList<AssignmentQuestionOptionResponseModel> AssignmentQuestionOptions { get; set; }

        public AssignmentResponseModel() { }

        public AssignmentResponseModel(
            Assignment assignment,
            bool showHints = false,
            bool showCorrect = false
        )
        {
            Id = assignment.Id;
            LessonId = assignment.LessonId;
            LessonName = assignment.Lesson?.Name;
            Name = assignment.Name;
            Description = assignment.Description;
            Hints = showHints ? assignment.Hints : null;
            Order = assignment.Order;
            IsActive = assignment.IsActive;
            Type = assignment.Type;
            User = assignment.User != null ? new UserModel(assignment.User) : new UserModel();
            AssignmentAttachments = new List<AssignmentAttachmentResponseModel>();
            if (assignment.Type == QuestionTypeEnum.Subjective)
            {
                assignment
                    .AssignmentAttachments?.ToList()
                    .ForEach(item =>
                        AssignmentAttachments.Add(new AssignmentAttachmentResponseModel(item))
                    );
            }

            AssignmentQuestionOptions = new List<AssignmentQuestionOptionResponseModel>();
            if (
                assignment.Type == QuestionTypeEnum.SingleChoice
                || assignment.Type == QuestionTypeEnum.MultipleChoice
            )
            {
                assignment
                    .AssignmentQuestionOptions?.ToList()
                    .ForEach(item =>
                        AssignmentQuestionOptions.Add(
                            new AssignmentQuestionOptionResponseModel(item, showCorrect)
                        )
                    );
            }
        }
    }

    public class AssignmentSubmissionAttachmentResponseModel
    {
        public Guid Id { get; set; }
        public Guid AssignmentSubmissionId { get; set; }
        public string FileUrl { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public UserModel User { get; set; }
    }
}
