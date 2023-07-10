using Hangfire;
using HangfireBasicAuthenticationFilter;
using Lingtren.Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using NLog.Web;
using NLog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});
builder.Services.AddVersionedApiExplorer(options =>
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
    x.DefaultRequestCulture = new RequestCulture("en-US");
    x.ApplyCurrentCultureToResponseHeaders = true;
    x.SupportedCultures = new List<CultureInfo> { new("ne-NP"), new("en-US"), new("ja-JP") };
    x.SupportedUICultures = new List<CultureInfo> { new("ne-NP"), new("en-US"), new("ja-JP") };
});
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCors(options => options.AddDefaultPolicy(
               builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                ));

builder.Services.AddAuthorization();
// var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
// //var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
// builder.Logging.ClearProviders();
// builder.Host.UseNLog();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseCors(x => x
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(_ => true) // allow any origin
              .AllowCredentials());

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization = new[] { new HangfireCustomBasicAuthenticationFilter
       {
           User = builder.Configuration.GetSection("Hangfire").GetSection("User").Value,
           Pass = builder.Configuration.GetSection("Hangfire").GetSection("Password").Value
       }}
});
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapFallbackToFile("index.html");
// app.UseHttpLogging();
// app.MigrateDatabase();
app.Run();