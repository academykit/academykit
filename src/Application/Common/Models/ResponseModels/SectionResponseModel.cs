namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class SectionResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public  UserModel User { get; set; }

        public SectionResponseModel(Section model)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}