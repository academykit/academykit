using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssignmentQuestionOptionResponseModel
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public string Option { get; set; }
        public bool? IsCorrect { get; set; }
        public bool? IsSelected { get; set; }
        public int Order { get; set; }
        public UserModel User { get; set; }

        public AssignmentQuestionOptionResponseModel(AssignmentQuestionOption model, bool showCorrect = false)
        {
            Id = model.Id;
            AssignmentId = model.AssignmentId;
            AssignmentName = model.Assignment?.Name;
            Option = model.Option;
            IsCorrect = showCorrect ? model.IsCorrect : null;
            Order = model.Order;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
        public AssignmentQuestionOptionResponseModel()
        {
        }
    }
}
