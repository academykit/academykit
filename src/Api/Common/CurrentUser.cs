namespace AcademyKit.Api.Common
{
    using AcademyKit.Domain.Enums;

    public class CurrentUser
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public UserRole Role { get; set; }
    }
}
