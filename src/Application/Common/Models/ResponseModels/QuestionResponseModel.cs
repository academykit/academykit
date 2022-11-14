namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;

    public class QuestionResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public UserModel User { get; set; }
        public QuestionResponseModel(Question question)
        {
            Id = question.Id;
            Name = question.Name;
            Type = question.Type;
            Description = question.Description;
            Hints = question.Hints;
            User = question.User != null ? new UserModel(question.User) : new UserModel();
        }
    }
}
