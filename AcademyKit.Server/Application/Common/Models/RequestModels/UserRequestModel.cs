namespace AcademyKit.Application.Common.Models.RequestModels
{
    using AcademyKit.Domain.Enums;

    public class UserRequestModel
    {
        public string MemberId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public UserRole Role { get; set; }
        public string Profession { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public string PublicUrls { get; set; }
        public Guid? DepartmentId { get; set; }
        public UserStatus Status { get; set; }
    }
}
