namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class QuestionPoolResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public UserModel User { get; set; }

        public QuestionPoolResponseModel(QuestionPool model)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            IsDeleted = model.IsDeleted;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
