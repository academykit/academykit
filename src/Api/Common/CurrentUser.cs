namespace Lingtren.Api.Common
{
    using Lingtren.Domain.Enums;
    public class CurrentUser
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public UserRole Role { get; set; }
    }
}
