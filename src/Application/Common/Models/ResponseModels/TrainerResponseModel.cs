using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class TrainerResponseModel
    {
        public Guid UserId { get; set; }
        public UserRole Role { get; set; }
        public string Email { get; set; }

        public TrainerResponseModel() { }

        public TrainerResponseModel(User user)
        {
            UserId = user.Id;
            Role = user.Role;
            Email = user.Email;
        }
    }
}
