namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    public class UserModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public UserRole Role { get; set; }

        public UserModel(User user)
        {
            Id = user.Id;
            FullName = user.FullName;
            ImageUrl = user.ImageUrl;
            Email = user.Email;
            MobileNumber = user.MobileNumber;
            Role = user.Role;
        }

        public UserModel()
        {
        }
    }
}