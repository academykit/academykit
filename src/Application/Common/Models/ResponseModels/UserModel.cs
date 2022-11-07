namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class UserModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }

        public UserModel(User user)
        {
            Id = user.Id;
            FullName = user.FullName;
            ImageUrl = user.ImageUrl;
        }

        public UserModel()
        {

        }
    }
}