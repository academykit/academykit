namespace Lingtren.Infrastructure.Configurations
{
    using Application.Common.Validators;
    using FluentValidation;
    using Hangfire;
    using Hangfire.MySql;
    using Infrastructure.Services;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Persistence;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql
            (configuration.GetConnectionString("DefaultConnection"), MySqlServerVersion.LatestSupportedServerVersion),
            ServiceLifetime.Scoped);

            services.AddHangfireServer(x => x.WorkerCount = 2).AddHangfire(x =>
            {
                x.UseFilter(new AutomaticRetryAttribute());
                if (environment.IsProduction())
                {
                    x.UseStorage(new MySqlStorage(configuration.GetConnectionString("Hangfireconnection"), new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 50000,
                        TablesPrefix = "Hangfire"
                    }));
                }
                else
                {
                    GlobalConfiguration.Configuration.UseInMemoryStorage();
                }
            });


            #region Service DI

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<ILevelService, LevelService>();
            services.AddTransient<IZoomSettingService, ZoomSettingService>();
            services.AddTransient<ISMTPSettingService, SMTPSettingService>();
            services.AddTransient<IGeneralSettingService, GeneralSettingService>();
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IGroupMemberService, GroupMemberService>();
            services.AddTransient<ISectionService, SectionService>();
            services.AddTransient<ILessonService, LessonService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IZoomLicenseService, ZoomLicenseService>();
            services.AddTransient<ICourseTeacherService, CourseTeacherService>();
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IQuestionPoolService, QuestionPoolService>();
            services.AddTransient<IQuestionPoolTeacherService, QuestionPoolTeacherService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddTransient<IQuestionSetService, QuestionSetService>();
            services.AddTransient<IAssignmentService, AssignmentService>();
            services.AddTransient<IWatchHistoryService, WatchHistoryService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IFileServerService, FileServerService>();
            services.AddTransient<IAmazonS3Service, AmazonS3Service>();
            services.AddTransient<IFeedbackService, FeedbackService>();
            services.AddTransient<IWebhookService, WebhookService>();
            services.AddTransient<IHangfireJobService, HangfireJobService>();
            services.AddTransient<ICertificateService, CertificateService>();
            services.AddTransient<IVideoService, VideoService>();
            services.AddTransient<ILogsService, LogsService>();
            services.AddTransient<IPhysicalLessonServices, PhysicalLessonService>();
            services.AddTransient<IDynamicImageGenerator, DynamicImageGenerator>();
            services.AddTransient<IEnrollmentService, EnrollmentService>();

            #endregion Service DI

            #region Validator DI
            services.AddSingleton<IValidator<SettingValue>, SettingValueValidator>();
            services.AddSingleton<IValidator<StorageSettingRequestModel>, StorageSettingRequestModelValidation>();
            services.AddSingleton<IValidator<CertificateRequestModel>, CertificateRequestModelValidator>();
            services.AddSingleton<IValidator<LiveClassLicenseRequestModel>, ZoomLicenseIdValidator>();
            services.AddSingleton<IValidator<LoginRequestModel>, LoginValidator>();
            services.AddSingleton<IValidator<UserRequestModel>, UserValidator>();
            services.AddSingleton<IValidator<GroupRequestModel>, GroupValidator>();
            services.AddSingleton<IValidator<ZoomSettingRequestModel>, ZoomSettingValidator>();
            services.AddSingleton<IValidator<SMTPSettingRequestModel>, SMTPSettingValidator>();
            services.AddSingleton<IValidator<GeneralSettingRequestModel>, GeneralSettingValidator>();
            services.AddSingleton<IValidator<DepartmentRequestModel>, DepartmentValidator>();
            services.AddSingleton<IValidator<ZoomLicenseRequestModel>, ZoomLicenseValidator>();
            services.AddSingleton<IValidator<SectionRequestModel>, SectionValidator>();
            services.AddSingleton<IValidator<CourseStatusRequestModel>, CourseStatusValidator>();
            services.AddSingleton<IValidator<CourseRequestModel>, CourseValidator>();
            services.AddSingleton<IValidator<CourseTeacherRequestModel>, CourseTeacherValidator>();
            services.AddSingleton<IValidator<LessonRequestModel>, LessonValidator>();
            services.AddSingleton<IValidator<ChangePasswordRequestModel>, ChangePasswordValidator>();
            services.AddSingleton<IValidator<ResetPasswordRequestModel>, ResetPasswordValidator>();
            services.AddSingleton<IValidator<QuestionPoolRequestModel>, QuestionPoolValidator>();
            services.AddSingleton<IValidator<QuestionPoolTeacherRequestModel>, QuestionPoolTeacherValidator>();
            services.AddSingleton<IValidator<QuestionRequestModel>, QuestionValidator>();
            services.AddSingleton<IValidator<AssignmentRequestModel>, AssignmentValidator>();
            services.AddSingleton<IValidator<WatchHistoryRequestModel>, WatchHistoryValidator>();
            services.AddSingleton<IValidator<ChangeEmailRequestModel>, ChangeEmailValidator>();
            services.AddSingleton<IValidator<LevelRequestModel>, LevelValidator>();
            services.AddSingleton<IValidator<CommentRequestModel>, CommentValidator>();
            services.AddSingleton<IValidator<AssignmentReviewRequestModel>, AssignmentReviewValidator>();
            services.AddSingleton<IValidator<MediaRequestModel>, MediaValidator>();
            services.AddSingleton<IValidator<FeedbackRequestModel>, FeedbackValidator>();
            services.AddSingleton<IValidator<IList<FeedbackSubmissionRequestModel>>, FeedbackSubmissionValidator>();
            services.AddSingleton<IValidator<SignatureRequestModel>, SignatureValidator>();
            services.AddSingleton<IValidator<CourseCertificateRequestModel>, CourseCertificateValidator>();
            services.AddSingleton<IValidator<PhysicalLessonReviewRequestModel>, PhysicalLessonReviewRequestModelValidator>();

            #endregion Validator DI

            return services;
        }
    }
}