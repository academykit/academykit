namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Domain.Entities;

    public class QuestionPoolResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int? QuestionCount { get; set; }
        public UserModel User { get; set; }

        public QuestionPoolResponseModel(QuestionPool model)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            IsDeleted = model.IsDeleted;
            QuestionCount = model.QuestionPoolQuestions?.Count;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
