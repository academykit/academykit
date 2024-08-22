namespace AcademyKit.Infrastructure.Configurations
{
    using System.Text;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Infrastructure.Security;
    using AcademyKit.Server.Infrastructure.Configurations;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

    public static class ConfigurationExtension
    {
        public static void AddJWTConfigurationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<JWT>(configuration.GetSection("JWT"));
            services.Configure<ApplicationInfo>(configuration.GetSection("Application"));
            services.Configure<Google>(configuration.GetSection("Google"));
            services.Configure<Microsoft>(configuration.GetSection("Microsoft"));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
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
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (
                                context.Exception.GetType() == typeof(SecurityTokenExpiredException)
                            )
                            {
                                context.Response.Headers.Append("IS-TOKEN-EXPIRED", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGoogle(
                    GoogleDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.ClientId = configuration.GetSection("Google:ClientId").Value;
                        options.ClientSecret = configuration
                            .GetSection("Google:ClientSecret")
                            .Value;
                    }
                )
                .AddMicrosoftAccount(
                    MicrosoftAccountDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.ClientId = configuration.GetSection("Microsoft:ClientId").Value;
                        options.ClientSecret = configuration
                            .GetSection("Microsoft:ClientSecret")
                            .Value;
                    }
                )
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                    "ApiKey",
                    null
                );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo { Title = "Academy Kit Auth Api", Version = "v1" }
                );
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer\"",
                    }
                );
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                );
            });
        }
    }
}
