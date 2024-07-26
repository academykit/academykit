namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;

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
