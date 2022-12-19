namespace Lingtren.Infrastructure.Configurations
{
    using FluentValidation;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Validators;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Persistence;
    using Lingtren.Infrastructure.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql
            (configuration.GetConnectionString("DefaultConnection"), MySqlServerVersion.LatestSupportedServerVersion),
            ServiceLifetime.Scoped);

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
            services.AddTransient<IAmazonService,AmazonService>();

            #endregion Service DI

            #region Validator DI

            services.AddSingleton<IValidator<LoginRequestModel>, LoginValidator>();
            services.AddSingleton<IValidator<UserRequestModel>, UserValidator>();
            services.AddSingleton<IValidator<GroupRequestModel>, GroupValidator>();
            services.AddSingleton<IValidator<ZoomSettingRequestModel>, ZoomSettingValidator>();
            services.AddSingleton<IValidator<SMTPSettingRequestModel>, SMTPSettingValidator>();
            services.AddSingleton<IValidator<GeneralSettingRequestModel>, GeneralSettingValidator>();
            services.AddSingleton<IValidator<DepartmentRequestModel>, DepartmentValidator>();
            services.AddSingleton<IValidator<ZoomLicenseRequestModel>, ZoomLicenseValidator>();
            services.AddSingleton<IValidator<SectionRequestModel>, SectionValidator>();
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

            #endregion Validator DI

            return services;
        }
    }
}