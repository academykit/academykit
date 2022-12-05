using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssignmentQuestionOptionResponseModel
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public string Option { get; set; }
        public bool IsCorrect { get; set; }
        public int Order { get; set; }
        public UserModel User { get; set; }

        public AssignmentQuestionOptionResponseModel(AssignmentQuestionOption model)
        {
            Id = model.Id;
            AssignmentId = model.AssignmentId;
            AssignmentName = model.Assignment?.Name;
            Option = model.Option;
            IsCorrect = model.IsCorrect;
            Order = model.Order;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
