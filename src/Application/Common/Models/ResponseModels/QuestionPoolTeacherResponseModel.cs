namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;

    public class QuestionPoolTeacherResponseModel
    {
        public Guid Id { get; set; }
        public Guid QuestionPoolId { get; set; }
        public string QuestionPoolName { get; set; }
        public PoolRole Role { get; set; }
        public UserModel User { get; set; }
        public DateTime CreatedOn { get; set; }

        public QuestionPoolTeacherResponseModel(QuestionPoolTeacher entity)
        {
            Id = entity.Id;
            QuestionPoolId = entity.QuestionPoolId;
            QuestionPoolName = entity.QuestionPool.Name;
            Role = entity.Role;
            User = entity.User != null ? new UserModel(entity.User) : new UserModel();
            CreatedOn = entity.CreatedOn;
        }
    }
}
