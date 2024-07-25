using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Lingtren.Infrastructure.Configurations
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _appUrl;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string HangFireCookieName = "HangFireCookie";
        private static readonly int CookieExpirationMinutes = 60;
        private readonly IConfiguration configuration;

        public HangfireAuthorizationFilter(IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _appUrl = configuration.GetSection("AppUrls:App").Value;
            this.configuration = configuration;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var setCookie = false;

            string access_token;
            // try to get token from query string
            if (httpContext.Request.Query.ContainsKey("access_token"))
            {
                access_token = httpContext.Request.Query["access_token"].FirstOrDefault();
                setCookie = true;
            }
            else
            {
                access_token = httpContext.Request.Cookies[HangFireCookieName];
            }

            if (string.IsNullOrEmpty(access_token))
            {
                httpContext?.Response?.Redirect(_appUrl);
            }

            try
            {
                SecurityToken validatedToken = null;
                var hand = new JwtSecurityTokenHandler();
                var claims = hand.ValidateToken(
                    access_token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration.GetValue<string>("JWT:Issuer"),
                        ValidAudience = configuration.GetValue<string>("JWT:Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT:Key") ?? "")
                        )
                    },
                    out validatedToken
                );

                // check if user claims contains the role of either Admin or SuperAdmin
                if (!claims.IsInRole("Admin") && !claims.IsInRole("SuperAdmin"))
                {
                    httpContext?.Response?.Redirect(_appUrl);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error during dashboard hangfire jwt validation process");
                httpContext?.Response?.Redirect(_appUrl);
            }

            if (setCookie && httpContext != null)
            {
                httpContext.Response.Cookies.Append(
                    HangFireCookieName,
                    access_token,
                    new CookieOptions()
                    {
                        Expires = DateTime.Now.AddMinutes(CookieExpirationMinutes)
                    }
                );
            }

            return true;
        }
    }
}
