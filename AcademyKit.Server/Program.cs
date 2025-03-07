﻿using System.Globalization;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Configurations;
using AcademyKit.Infrastructure.Persistence;
using AcademyKit.Infrastructure.RateLimiting;
using Asp.Versioning;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using PuppeteerSharp;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJWTConfigurationServices(builder.Configuration);
builder.Services.AddLocalization();
builder.Services.AddRequestLocalization(x =>
{
    //x.DefaultRequestCulture = new RequestCulture("en-US");
    x.DefaultRequestCulture = new RequestCulture("ne-NP");
    x.ApplyCurrentCultureToResponseHeaders = true;
    x.SupportedCultures = new List<CultureInfo> { new("ne-NP"), new("en-US"), new("ja-JP") };
    x.SupportedUICultures = new List<CultureInfo> { new("ne-NP"), new("en-US"), new("ja-JP") };
});
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
);

builder
    .Services.AddAuthorizationBuilder()
    .AddDefaultPolicy(
        "ApiKeyOrBearer",
        policy =>
        {
            policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, "ApiKey");
            policy.RequireAuthenticatedUser();
        }
    );

builder.Services.Configure<RateLimitSettings>(
    builder.Configuration.GetSection(RateLimitSettings.RateLimit)
);

builder.Services.AddHostedService<ApplicationDbInitializer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    }
);

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(x =>
    x.AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(_ => true) // allow any origin
        .AllowCredentials()
);

app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions { Authorization = [new HangfireAuthorizationFilter(app.Configuration)], }
);
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.UseSlidingWindowRateLimiting();
app.MapControllers();

app.MapFallbackToFile("index.html");

// app.UseHttpLogging();

// download the chrome browser earlier
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();

app.Run();
