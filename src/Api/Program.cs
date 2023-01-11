using Hangfire;
using HangfireBasicAuthenticationFilter;
using Lingtren.Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc;

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

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCors(options => options.AddDefaultPolicy(
               builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                ));

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();