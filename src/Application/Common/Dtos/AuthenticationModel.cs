
namespace Lingtren.Application.Common.Dtos
{
    using Lingtren.Domain.Enums;

    public class AuthenticationModel
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string Token { get; set; }
        public int ExpirationDuration { get; set; }
        public string RefreshToken { get; set; }
    }
}
