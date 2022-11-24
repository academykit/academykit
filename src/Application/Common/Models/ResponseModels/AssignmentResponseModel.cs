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
        public string Hint { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public AssignmentType Type { get; set; }
        public UserModel User { get; set; }
        public IList<AssignmentAttachmentResponseModel> AssignmentAttachments { get; set; }
        public IList<AssignmentQuestionOptionResponseModel> AssignmentQuestionOptions { get; set; }
        
        public AssignmentResponseModel(Assignment assignment)
        {
            Id = assignment.Id;
            LessonId = assignment.LessonId;
            LessonName = assignment.Lesson?.Name;
            Name = assignment.Name;
            Description = assignment.Description;
            Hint = assignment.Hint;
            Order = assignment.Order;
            IsActive = assignment.IsActive;
            Type = assignment.Type;
            User = assignment.User != null ? new UserModel(assignment.User): new UserModel();

            AssignmentAttachments = new List<AssignmentAttachmentResponseModel>();
            if(assignment.Type == AssignmentType.Subjective)
            {
                assignment.AssignmentAttachments?.ToList().ForEach(item => AssignmentAttachments.Add(new AssignmentAttachmentResponseModel(item)));
            }

            AssignmentQuestionOptions = new List<AssignmentQuestionOptionResponseModel>();
            if(assignment.Type == AssignmentType.MCQ)
            {
                assignment.AssignmentQuestionOptions?.ToList().ForEach(item => AssignmentQuestionOptions.Add(new AssignmentQuestionOptionResponseModel(item)));
            }
        }
    }
}
