namespace Lingtren.Application.Common.Models.RequestModels
{
    using Lingtren.Domain.Enums;

    public class UserRequestModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public UserRole Role { get; set; }
        public string Profession { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public string PublicUrls { get; set; }
        public bool IsActive { get; set; }
    }
}
