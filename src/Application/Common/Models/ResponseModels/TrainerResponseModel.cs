using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.ResponseModels
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
