using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Localization;
using Hangfire;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Infrastructure.Services;

/// <summary>
/// Service for handling Hangfire background jobs related to the Academy Kit application.
/// </summary>
public class HangfireJobService : BaseService, IHangfireJobService
{
    private readonly string _appUrl;
    private readonly IEmailService _emailService;
    private readonly IVideoService _videoService;
    private readonly IFileServerService _fileServerService;
    private readonly Dictionary<string, string> _placeholders;

    private const string EMAIL_PLACEHOLDER = "email";
    private const string USER_NAME_PLACEHOLDER = "userName";
    private const string APP_PLACEHOLDER = "app";
    private const string EMAIL_SIGNATURE_PLACEHOLDER = "emailSignature";
    private const string COURSE_NAME_PLACEHOLDER = "courseName";
    private const string COURSE_SLUG_PLACEHOLDER = "courseSlug";
    private const string COMPANY_NAME_PLACEHOLDER = "companyName";
    private const string COMPANY_NUMBER_PLACEHOLDER = "companyNumber";
    private const string PASSWORD_PLACEHOLDER = "password";
    private const string MESSAGE_PLACEHOLDER = "message";
    private const string GROUP_NAME_PLACEHOLDER = "groupName";
    private const string GROUP_SLUG_PLACEHOLDER = "groupSlug";
    private const string NEW_EMAIL_PLACEHOLDER = "newEmail";
    private const string ASSESSMENT_TITLE_PLACEHOLDER = "assessmentTitle";

    /// <summary>
    /// Initializes a new instance of the <see cref="HangfireJobService"/> class.
    /// </summary>
    public HangfireJobService(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        ILogger<HangfireJobService> logger,
        IEmailService emailService,
        IStringLocalizer<ExceptionLocalizer> localizer,
        IVideoService videoService,
        IFileServerService fileServerService
    )
        : base(unitOfWork, logger, localizer)
    {
        _emailService = emailService;
        _appUrl = configuration.GetSection("AppUrls:App").Value;
        _videoService = videoService;
        _fileServerService = fileServerService;

        _placeholders = new Dictionary<string, string>
        {
            { USER_NAME_PLACEHOLDER, "{userName}" },
            { APP_PLACEHOLDER, "{appUrl}" },
            { COURSE_NAME_PLACEHOLDER, "{courseName}" },
            { COURSE_SLUG_PLACEHOLDER, "{courseSlug}" },
            { EMAIL_SIGNATURE_PLACEHOLDER, "{emailSignature}" },
            { COMPANY_NAME_PLACEHOLDER, "{companyName}" },
            { COMPANY_NUMBER_PLACEHOLDER, "{companyNumber}" },
            { PASSWORD_PLACEHOLDER, "{password}" },
            { EMAIL_PLACEHOLDER, "{email}" },
            { MESSAGE_PLACEHOLDER, "{message}" },
            { GROUP_NAME_PLACEHOLDER, "{groupName}" },
            { GROUP_SLUG_PLACEHOLDER, "{groupSlug}" },
            { NEW_EMAIL_PLACEHOLDER, "{newEmail}" }
        };
    }

    #region Email Hangfire Services

    #region Public Methods
    /// <summary>
    /// Sends a course review email to administrators and super administrators.
    /// </summary>
    /// <param name="courseName">The name of the course being reviewed.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendCourseReviewMailAsync(string courseName, PerformContext context = null)
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var users = await _unitOfWork
                .GetRepository<User>()
                .GetAllAsync(predicate: p =>
                    p.Role == UserRole.Admin || p.Role == UserRole.SuperAdmin
                )
                .ConfigureAwait(false);

            if (users.Count == default)
            {
                return;
            }

            foreach (var user in users)
            {
                await SendEmailAsync(
                    getSubject: _ => $"Training Review Status - {courseName}",
                    getMessage: (name, companyName) =>
                        $"Dear {name},<br><br>"
                        + $"Training <a href='{_appUrl}/settings/courses'>'{courseName}'</a> is under review. "
                        + "Kindly provide feedback and assessment. Your input is vital for quality assurance. Thank you.<br><br>"
                        + $"Best regards,<br>{companyName}",
                    user.Email,
                    user.FirstName,
                    new Dictionary<string, string> { { COURSE_NAME_PLACEHOLDER, courseName } },
                    MailType.TrainingReview
                );
            }
        });
    }

    /// <summary>
    /// Sends a course rejection email to course teachers.
    /// </summary>
    /// <param name="courseId">The ID of the rejected course.</param>
    /// <param name="message">The rejection message.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task CourseRejectedMailAsync(
        Guid courseId,
        string message,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var course = await _unitOfWork
                .GetRepository<Course>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Id == courseId,
                    include: source =>
                        source.Include(x => x.CourseTeachers).ThenInclude(x => x.User)
                )
                .ConfigureAwait(false);

            if (course == default)
            {
                throw new EntityNotFoundException(_localizer.GetString("CourseNotFound"));
            }

            foreach (var teacher in course.CourseTeachers)
            {
                if (!string.IsNullOrEmpty(teacher.User?.Email))
                {
                    await SendEmailAsync(
                        getSubject: _ => $"Training Rejection - {course.Name}",
                        getMessage: (name, companyName) =>
                            $"Dear {name},<br><br>"
                            + $"We regret to inform you that your training, {course.Name} has been rejected for the following reason:<br><br>"
                            + $"{message}<br><br>"
                            + "However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. "
                            + "Once you have addressed the identified issues, please resubmit the training program for further review.<br><br>"
                            + $"Thank you for your understanding and cooperation.<br><br>"
                            + $"Best regards,<br>{companyName}",
                        teacher.User.Email,
                        teacher.User.FirstName,
                        new Dictionary<string, string>
                        {
                            { COMPANY_NAME_PLACEHOLDER, course.Name },
                            { MESSAGE_PLACEHOLDER, message }
                        },
                        MailType.TrainingReject
                    );
                }
            }
        });
    }

    /// <summary>
    /// Sends an email to a newly created user with their account details.
    /// </summary>
    /// <param name="emailAddress">The user's email address.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="password">The user's initial password.</param>
    /// <param name="companyName">The company name.</param>
    /// <param name="companyNumber">The company number.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendUserCreatedPasswordEmail(
        string emailAddress,
        string firstName,
        string password,
        string companyName,
        string companyNumber,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException("Context not found.");
            }

            await SendEmailAsync(
                getSubject: _ => "Account Created",
                getMessage: (name, company) =>
                    $"Dear {name},<br><br>"
                    + $"Your account has been created in the <a href='{_appUrl}'><u style='color:blue;'>LMS</u></a>.<br><br>"
                    + $"Here are the login details for your LMS account:<br><br>"
                    + $"Email: {emailAddress}<br>"
                    + $"Password: {password}<br><br>"
                    + $"Please use the above login credentials to access your account.<br><br>"
                    + $"Best regards,<br>{company}<br>{companyNumber}",
                emailAddress,
                firstName,
                new Dictionary<string, string>
                {
                    { EMAIL_PLACEHOLDER, emailAddress },
                    { PASSWORD_PLACEHOLDER, password },
                    { COMPANY_NAME_PLACEHOLDER, companyName },
                    { COMPANY_NUMBER_PLACEHOLDER, companyNumber }
                },
                MailType.UserCreate
            );
        });
    }

    /// <summary>
    /// Sends emails to imported users with their account details.
    /// </summary>
    /// <param name="dtos">A list of user email DTOs containing user information.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendEmailImportedUserAsync(
        IList<UserEmailDto> dtos,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            foreach (var emailDto in dtos)
            {
                await SendEmailAsync(
                    getSubject: _ => "Account Created",
                    getMessage: (name, company) =>
                        $"Dear {name},<br><br>"
                        + $"Your account has been created in the <a href='{_appUrl}'><u style='color:blue;'>LMS</u></a>.<br><br>"
                        + $"Here are the login details for your LMS account:<br><br>"
                        + $"Email: {emailDto.Email}<br>"
                        + $"Password: {emailDto.Password}<br><br>"
                        + $"Please use the above login credentials to access your account.<br><br>"
                        + $"Best regards,<br>{emailDto.CompanyName}<br>{emailDto.CompanyNumber}",
                    emailDto.Email,
                    emailDto.FullName,
                    new Dictionary<string, string>
                    {
                        { EMAIL_PLACEHOLDER, emailDto.Email },
                        { PASSWORD_PLACEHOLDER, emailDto.Password },
                        { COMPANY_NAME_PLACEHOLDER, emailDto.CompanyName },
                        { COMPANY_NUMBER_PLACEHOLDER, emailDto.CompanyNumber }
                    },
                    MailType.UserCreate
                );
            }
        });
    }

    /// <summary>
    /// Sends an email to new group members.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <param name="groupSlug">The slug of the group.</param>
    /// <param name="userIds">A list of user IDs for the new group members.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendMailNewGroupMember(
        string groupName,
        string groupSlug,
        IList<Guid> userIds,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var users = await _unitOfWork
                .GetRepository<User>()
                .GetAllAsync(predicate: p => userIds.Contains(p.Id))
                .ConfigureAwait(false);

            if (users.Count == default)
            {
                return;
            }

            foreach (var user in users)
            {
                var fullName = string.IsNullOrEmpty(user.MiddleName)
                    ? $"{user.FirstName} {user.LastName}"
                    : $"{user.FirstName} {user.MiddleName} {user.LastName}";

                await SendEmailAsync(
                    getSubject: _ => "New Group Member",
                    getMessage: (name, company) =>
                        $"Dear {name},<br><br>"
                        + $"You have been added to the {groupName}. Now you can find the Training Materials which has been created for this group.<br><br>"
                        + $"Link to the group: <a href='{_appUrl}/groups/{groupSlug}'><u style='color:blue;'>Click here</u></a><br><br>"
                        + $"Thank You,<br>{company}",
                    user.Email,
                    fullName,
                    new Dictionary<string, string>
                    {
                        { GROUP_NAME_PLACEHOLDER, groupName },
                        { GROUP_SLUG_PLACEHOLDER, groupSlug }
                    },
                    MailType.GroupMemberAdd
                );
            }
        });
    }

    /// <summary>
    /// Sends an email to group members when a new course is published.
    /// </summary>
    /// <param name="groupId">The ID of the group.</param>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="courseSlug">The slug of the course.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task GroupCoursePublishedMailAsync(
        Guid groupId,
        string courseName,
        string courseSlug,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var group = await _unitOfWork
                .GetRepository<Domain.Entities.Group>()
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Id == groupId,
                    include: source => source.Include(x => x.GroupMembers).ThenInclude(x => x.User)
                )
                .ConfigureAwait(false);

            if (group.GroupMembers.Count != default)
            {
                foreach (var member in group.GroupMembers)
                {
                    var fullName = string.IsNullOrEmpty(member.User?.MiddleName)
                        ? $"{member.User?.FirstName} {member.User?.LastName}"
                        : $"{member.User?.FirstName} {member.User?.MiddleName} {member.User?.LastName}";

                    await SendEmailAsync(
                        getSubject: _ => "New training published",
                        getMessage: (name, company) =>
                            $"Dear {name},<br><br>"
                            + $"You have new {courseName} training available for the {group.Name} group. "
                            + $"Please, go to {group.Name} group or "
                            + $"<a href='{_appUrl}/trainings/{courseSlug}'><u style='color:blue;'>Click Here</u></a> to find the training there.<br><br>"
                            + $"Thank You,<br>{company}",
                        member.User?.Email,
                        fullName,
                        new Dictionary<string, string>
                        {
                            { COURSE_NAME_PLACEHOLDER, courseName },
                            { COURSE_SLUG_PLACEHOLDER, courseSlug }
                        },
                        MailType.None
                    );
                }
            }
        });
    }

    /// <summary>
    /// Sends an email to course teachers when a new user enrolls in their course.
    /// </summary>
    /// <param name="userName">The name of the enrolled user.</param>
    /// <param name="userEmail">The email of the enrolled user.</param>
    /// <param name="courseId">The ID of the course.</param>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendCourseEnrollmentMailAsync(
        string userName,
        string userEmail,
        Guid courseId,
        string courseName,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
            }

            var course = await _unitOfWork
                .GetRepository<Course>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Id == courseId,
                    include: source =>
                        source.Include(x => x.CourseTeachers).ThenInclude(x => x.User)
                )
                .ConfigureAwait(false);

            if (course.CourseTeachers == null)
            {
                throw new ArgumentException(_localizer.GetString("TeacherNotFound"));
            }

            var teachers = course
                .CourseTeachers.Select(teacher => teacher.User)
                .Where(user => user != null);
            foreach (var teacher in teachers)
            {
                await SendEmailAsync(
                    getSubject: _ => "New Training Enrollment",
                    getMessage: (name, company) =>
                        $"Dear {name},<br><br>"
                        + $"A new user has enrolled in your {courseName} course. Here are the details:"
                        + $"<ul><li>Training: {courseName}</li><li>Enrolled User: {userName}</li><li>User Email: {userEmail}</li></ul>"
                        + $"Thank you for your attention to this enrollment. We appreciate your dedication to providing an exceptional learning experience.<br><br>"
                        + $"Best regards,<br>{company}",
                    teacher.Email,
                    teacher.FirstName,
                    new Dictionary<string, string>
                    {
                        { COURSE_NAME_PLACEHOLDER, courseName },
                        { USER_NAME_PLACEHOLDER, userName },
                        { EMAIL_PLACEHOLDER, userEmail }
                    },
                    MailType.TrainingEnrollment
                );
            }
        });
    }

    /// <summary>
    /// Sends an email to users when their certificates are issued.
    /// </summary>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="certificateUserIssuedDtos">A list of DTOs containing user information for certificate issuance.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendCertificateIssueMailAsync(
        string courseName,
        IList<CertificateUserIssuedDto> certificateUserIssuedDtos,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
            }

            foreach (var user in certificateUserIssuedDtos)
            {
                await SendEmailAsync(
                    getSubject: _ => "Certificate Issued",
                    getMessage: (name, company) =>
                        $"Dear {name},<br><br>"
                        + $"We are happy to inform you that your Certificate of Achievement for {courseName} has been issued "
                        + $"and is now available in your profile on the application. "
                        + $"Please log in to your account and navigate to your profile to view and download your certificate.<br><br>"
                        + $"We hope you find the training helpful.<br><br>"
                        + $"Best regards,<br>{company}",
                    user?.Email,
                    user?.UserName,
                    new Dictionary<string, string> { { COURSE_NAME_PLACEHOLDER, courseName } },
                    MailType.CertificateIssue
                );
            }
        });
    }

    /// <summary>
    /// Sends an email to enrolled users when new content is added to a course.
    /// </summary>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="courseSlug">The slug of the course.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendLessonAddedMailAsync(
        string courseName,
        string courseSlug,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
            }

            var course = await _unitOfWork
                .GetRepository<Course>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Name == courseName,
                    include: source =>
                        source.Include(x => x.CourseEnrollments).ThenInclude(x => x.User)
                )
                .ConfigureAwait(false);

            if (course.CourseEnrollments == null)
            {
                throw new ArgumentException("No enrollments");
            }

            var enrolledUsers = course
                .CourseEnrollments.Select(enrollment => enrollment.User)
                .Where(user => user != null && !string.IsNullOrEmpty(user.Email));
            foreach (var user in enrolledUsers)
            {
                await SendEmailAsync(
                    getSubject: _ => "New Content Added",
                    getMessage: (name, company) =>
                        $"Dear {name},<br><br>"
                        + $"Your enrolled training entitled <a href='{_appUrl}/trainings/{courseSlug}'>"
                        + $"<u style='color:blue;'>{courseName}</u></a> has been updated with new content. "
                        + $"We encourage you to visit the training page and "
                        + $"explore the new materials to enhance your learning experience.<br><br>"
                        + $"Thank You,<br>{company}",
                    user.Email,
                    user.FirstName,
                    new Dictionary<string, string>
                    {
                        { COURSE_NAME_PLACEHOLDER, courseName },
                        { COURSE_SLUG_PLACEHOLDER, courseSlug }
                    },
                    MailType.AddLesson
                );
            }
        });
    }

    /// <summary>
    /// Sends an email to a user when their account email is updated.
    /// </summary>
    /// <param name="fullName">The full name of the user.</param>
    /// <param name="newEmail">The new email address.</param>
    /// <param name="oldEmail">The old email address.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendEmailChangedMailAsync(
        string fullName,
        string newEmail,
        string oldEmail,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
            }

            await SendEmailAsync(
                getSubject: _ => "Notification: Email Address Change",
                getMessage: (name, company) =>
                    $"Dear {name},<br><br>"
                    + $"A recent change has been made to the email address associated with your account to {newEmail}.<br>"
                    + $"Please check your email for the login credentials. If you encounter any difficulties, "
                    + $"please contact your administrator immediately.<br><br>"
                    + $"Thank You,<br>{company}",
                oldEmail,
                fullName,
                new Dictionary<string, string> { { NEW_EMAIL_PLACEHOLDER, newEmail } },
                MailType.ChangedEmail
            );
        });
    }

    /// <summary>
    /// Sends an email to administrators when an assessment is accepted.
    /// </summary>
    /// <param name="assessmentId">The ID of the accepted assessment.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendAssessmentAcceptMailAsync(
        Guid assessmentId,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var assessment = await _unitOfWork
                .GetRepository<Assessment>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Id == assessmentId,
                    include: source => source.Include(x => x.User)
                )
                .ConfigureAwait(false);

            if (assessment.Id != assessmentId)
            {
                return;
            }

            await SendEmailAsync(
                getSubject: _ => $"Assessment Review Status - {assessment.AssessmentStatus}",
                getMessage: (name, company) =>
                    $"Dear {name},<br><br>"
                    + $"Assessment <a href='{_appUrl}/settings/courses'>{assessment.Title}</a> published successfully. Thank you.<br><br>"
                    + $"Best regards,<br>{company}",
                assessment.User.Email,
                assessment.User.FirstName,
                new Dictionary<string, string>
                {
                    { ASSESSMENT_TITLE_PLACEHOLDER, assessment.Title }
                },
                MailType.None
            );
        });
    }

    /// <summary>
    /// Sends an email to administrators when an assessment is rejected.
    /// </summary>
    /// <param name="assessmentId">The ID of the rejected assessment.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendAssessmentRejectMailAsync(
        Guid assessmentId,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var assessment = await _unitOfWork
                .GetRepository<Assessment>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Id == assessmentId,
                    include: source => source.Include(x => x.User)
                )
                .ConfigureAwait(false);

            if (assessment.Id != assessmentId)
            {
                return;
            }

            await SendEmailAsync(
                getSubject: _ => $"Assessment Review Status - {assessment.AssessmentStatus}",
                getMessage: (name, company) =>
                    $"Dear {name},<br><br>"
                    + $"We regret to inform you that your Assessment, {assessment.Title} has been rejected for the following reason:<br><br>"
                    + $"{assessment.Message}<br><br>"
                    + $"However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. "
                    + $"Once you have addressed the identified issues, please resubmit the assessment for further review.<br><br>"
                    + $"Thank you for your understanding and cooperation.<br><br>"
                    + $"Best regards,<br>{company}",
                assessment.User.Email,
                assessment.User.FirstName,
                new Dictionary<string, string>
                {
                    { ASSESSMENT_TITLE_PLACEHOLDER, assessment.Title },
                    { MESSAGE_PLACEHOLDER, assessment.Message }
                },
                MailType.None
            );
        });
    }

    /// <summary>
    /// Sends an email to administrators when an assessment is submitted for review.
    /// </summary>
    /// <param name="assessmentId">The ID of the assessment submitted for review.</param>
    /// <param name="user">The list of users to notify.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendAssessmentReviewMailAsync(
        Guid assessmentId,
        IList<User> user,
        PerformContext context = null
    )
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var assessment = await _unitOfWork
                .GetRepository<Assessment>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == assessmentId)
                .ConfigureAwait(false);

            if (assessment.Id != assessmentId)
            {
                return;
            }

            var admins = user.Where(x => x.Role == UserRole.SuperAdmin || x.Role == UserRole.Admin);

            foreach (var admin in admins)
            {
                await SendEmailAsync(
                    getSubject: _ =>
                        $"Request for Assessment Review - {assessment.AssessmentStatus}",
                    getMessage: (name, company) =>
                        $"Dear {name},<br><br>"
                        + $"Assessment <a href='{_appUrl}/settings/courses'>{assessment.Title}</a> is requested for review. Thank you.<br><br>"
                        + $"Best regards,<br>{company}",
                    admin.Email,
                    admin.FirstName,
                    new Dictionary<string, string>
                    {
                        { ASSESSMENT_TITLE_PLACEHOLDER, assessment.Title }
                    },
                    MailType.None
                );
            }
        });
    }

    #endregion Public Methods

    #region  Private Methods
    /// <summary>
    /// Sends an email using a template or custom message.
    /// </summary>
    /// <param name="getSubject">Function to get the email subject.</param>
    /// <param name="getMessage">Function to get the email message.</param>
    /// <param name="recipientEmail">The recipient's email address.</param>
    /// <param name="recipientName">The recipient's name.</param>
    /// <param name="additionalPlaceholders">Additional placeholders for the email template.</param>
    /// <param name="mailType">The type of mail being sent.</param>
    private async Task SendEmailAsync(
        Func<string, string> getSubject,
        Func<string, string, string> getMessage,
        string recipientEmail,
        string recipientName,
        Dictionary<string, string> additionalPlaceholders = null,
        MailType mailType = MailType.None
    )
    {
        var model = new EmailRequestDto { To = recipientEmail };
        var setting = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();

        MailNotification template = null;
        if (mailType != MailType.None)
        {
            template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p => p.MailType == mailType && p.IsActive)
                .ConfigureAwait(false);
        }

        if (template == null)
        {
            model.Subject = getSubject(recipientName);
            model.Message = getMessage(recipientName, setting.CompanyName);
        }
        else
        {
            model.Subject = template.Subject;
            model.Message = template.Message;

            var placeholders = new Dictionary<string, string>(_placeholders)
            {
                { USER_NAME_PLACEHOLDER, recipientName },
                { APP_PLACEHOLDER, _appUrl },
                { EMAIL_SIGNATURE_PLACEHOLDER, $"Best regards, <br> {setting.CompanyName}" }
            };

            if (additionalPlaceholders != null)
            {
                foreach (var placeholder in additionalPlaceholders)
                {
                    placeholders[placeholder.Key] = placeholder.Value;
                }
            }

            foreach (var placeholder in placeholders)
            {
                model.Message = model.Message.Replace(placeholder.Key, placeholder.Value);
            }
        }

        await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
    }
    #endregion Private Methods

    #endregion Email Hangfire Services

    /// <summary>
    /// Updates the information of a lesson video, including its duration.
    /// </summary>
    /// <param name="lessonId">The ID of the lesson.</param>
    /// <param name="context">The Hangfire performance context.</param>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task LessonVideoUploadedAsync(Guid lessonId, PerformContext context = null)
    {
        await ExecuteAsync(async () =>
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var lesson = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == lessonId)
                .ConfigureAwait(false);

            if (lesson.Type != LessonType.Video && lesson.Type != LessonType.RecordedVideo)
            {
                if (string.IsNullOrEmpty(lesson.VideoUrl))
                {
                    throw new ArgumentException(_localizer.GetString("FileNotFound"));
                }

                var videoPath = await _fileServerService
                    .GetFileLocalPathAsync(lesson.VideoUrl)
                    .ConfigureAwait(true);
                var duration = await _videoService.GetVideoDuration(videoPath).ConfigureAwait(true);
                lesson.Duration = duration;
                _unitOfWork.GetRepository<Lesson>().Update(lesson);
                _videoService.DeleteTempFile(videoPath);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        });
    }
}
