namespace Lingtren.Infrastructure.Configurations
{
    using Asp.Versioning.ApiExplorer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerUI;

    internal static class SwaggerExtensions
    {
        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Academy Kit",
                        Version = "v1",
                        Description = "Rest api for Learning"
                    }
                );
                options.SwaggerDoc(
                    "v2",
                    new OpenApiInfo
                    {
                        Title = "Academy Kit",
                        Version = "v2",
                        Description = "Rest api for Learning"
                    }
                );
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the bearer",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        BearerFormat = "JWT",
                        Scheme = "Bearer",
                        Type = SecuritySchemeType.ApiKey
                    }
                );
                options.AddSecurityRequirement(
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

            return services;
        }

        internal static IApplicationBuilder UseSwaggerDocumentation(
            this IApplicationBuilder app,
            IApiVersionDescriptionProvider provider
        )
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => SetupAction(options, provider));
            return app;
        }

        private static SwaggerUIOptions SetupAction(
            SwaggerUIOptions options,
            IApiVersionDescriptionProvider provider
        )
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant()
                );
            }

            return options;
        }
    }
}
