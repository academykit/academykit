using System.Security.Claims;
using System.Text.Encodings.Web;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AcademyKit.Infrastructure.Security
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDbContext _dbContext;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ApplicationDbContext dbContext
        )
            : base(options, logger, encoder)
        {
            _dbContext = dbContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-API-Key", out var apiKeyValues))
            {
                return AuthenticateResult.Fail("Missing API Key");
            }

            var providedApiKey = apiKeyValues.FirstOrDefault();
            var matchedApiKey = await _dbContext
                .Set<ApiKey>()
                .Where((row) => row.Key == providedApiKey)
                .Include((row) => row.User)
                .FirstOrDefaultAsync();

            if (matchedApiKey == null)
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "APIKeyUser"),
                new Claim("uid", matchedApiKey.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, matchedApiKey.User.FirstName),
                new Claim(ClaimTypes.Email, matchedApiKey.User.Email),
                new Claim("mobile_number", matchedApiKey.User.MobileNumber),
                new Claim(ClaimTypes.Role, matchedApiKey.User.Role.ToString()),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            throw new AuthenticationFailureException(
                "Authentication failed. Invalid or missing API key."
            );
        }
    }
}
