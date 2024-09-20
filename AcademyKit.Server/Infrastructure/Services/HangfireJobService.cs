﻿using AcademyKit.Application.Common.Dtos;
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

    private const string EMAIL_PLACEHOLDER = "{EmailAddress}";
    private const string PASSWORD_PLACEHOLDER = "{Password}";
    private const string USER_NAME_PLACEHOLDER = "{UserName}";
    private const string APP_PLACEHOLDER = "{AppUrl}";
    private const string EMAIL_SIGNATURE_PLACEHOLDER = "{EmailSignature}";
    private const string TRAINING_NAME_PLACEHOLDER = "{TrainingName}";
    private const string TRAINING_SLUG_PLACEHOLDER = "{TrainingSlug}";
    private const string TRAINER_NAME_PLACEHOLDER = "{TrainerName}";
    private const string TRAINING_START_DATE_PLACEHOLDER = "{TrainingStartDate}";
    private const string EXAM_START_DATE_PLACEHOLDER = "{ExamStartDate}";
    private const string EXAM_START_TIME_PLACEHOLDER = "{ExamStartTime}";
    private const string EXAM_END_DATE_PLACEHOLDER = "{ExamEndDate}";
    private const string EXAM_END_TIME_PLACEHOLDER = "{ExamEndTime}";
    private const string CERTIFICATE_TITLE_PLACEHOLDER = "{CertificateTitle}";
    private const string CERTIFICATE_ISSUE_DATE_PLACEHOLDER = "{CertificateIssueDate}";
    private const string COMPANY_NAME_PLACEHOLDER = "{CompanyName}";
    private const string COMPANY_ADDRESS_PLACEHOLDER = "{CompanyAddress}";
    private const string COMPANY_NUMBER_PLACEHOLDER = "{CompanyPhoneNumber}";
    private const string MESSAGE_PLACEHOLDER = "{Message}";
    private const string GROUP_NAME_PLACEHOLDER = "{GroupName}";
    private const string GROUP_SLUG_PLACEHOLDER = "{GroupSlug}";
    private const string ASSESSMENT_TITLE_PLACEHOLDER = "{AssessmentTitle}";

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
            { EMAIL_PLACEHOLDER, "" },
            { PASSWORD_PLACEHOLDER, "" },
            { USER_NAME_PLACEHOLDER, "" },
            { APP_PLACEHOLDER, _appUrl },
            { TRAINING_NAME_PLACEHOLDER, "" },
            { TRAINING_SLUG_PLACEHOLDER, "" },
            { TRAINER_NAME_PLACEHOLDER, "" },
            { TRAINING_START_DATE_PLACEHOLDER, "" },
            { EXAM_START_DATE_PLACEHOLDER, "" },
            { EXAM_START_TIME_PLACEHOLDER, "" },
            { EXAM_END_DATE_PLACEHOLDER, "" },
            { EXAM_END_TIME_PLACEHOLDER, "" },
            { EMAIL_SIGNATURE_PLACEHOLDER, "" },
            { CERTIFICATE_TITLE_PLACEHOLDER, "" },
            { CERTIFICATE_ISSUE_DATE_PLACEHOLDER, "" },
            { COMPANY_NAME_PLACEHOLDER, "" },
            { COMPANY_ADDRESS_PLACEHOLDER, "" },
            { COMPANY_NUMBER_PLACEHOLDER, "" },
            { MESSAGE_PLACEHOLDER, "" },
            { GROUP_NAME_PLACEHOLDER, "" },
            { GROUP_SLUG_PLACEHOLDER, "" },
            { ASSESSMENT_TITLE_PLACEHOLDER, "" }
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

            _placeholders[TRAINING_NAME_PLACEHOLDER] = courseName;

            foreach (var user in users)
            {
                _placeholders[USER_NAME_PLACEHOLDER] = user.FirstName;
                await SendEmailAsync(
                    getSubject: () => $"Training Review Status",
                    getMessage: () =>
                        $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                        + $"Training <a href='{APP_PLACEHOLDER}/settings/courses'>'{TRAINING_NAME_PLACEHOLDER}'</a> is under review. "
                        + "Kindly provide feedback and assessment. Your input is vital for quality assurance. Thank you.<br><br>"
                        + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                    user.Email,
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

            _placeholders[TRAINING_NAME_PLACEHOLDER] = course.Name;
            _placeholders[MESSAGE_PLACEHOLDER] = message;

            foreach (var teacher in course.CourseTeachers)
            {
                if (!string.IsNullOrEmpty(teacher.User?.Email))
                {
                    _placeholders[USER_NAME_PLACEHOLDER] = teacher.User.FirstName;

                    await SendEmailAsync(
                        getSubject: () => $"Training Rejection",
                        getMessage: () =>
                            $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                            + $"We regret to inform you that your training, {TRAINING_NAME_PLACEHOLDER} has been rejected for the following reason:<br><br>"
                            + $"{MESSAGE_PLACEHOLDER}<br><br>"
                            + "However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. "
                            + "Once you have addressed the identified issues, please resubmit the training program for further review.<br><br>"
                            + $"Thank you for your understanding and cooperation.<br><br>"
                            + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                        teacher.User.Email,
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

            _placeholders[USER_NAME_PLACEHOLDER] = firstName;
            _placeholders[PASSWORD_PLACEHOLDER] = password;
            _placeholders[EMAIL_PLACEHOLDER] = emailAddress;

            await SendEmailAsync(
                getSubject: () => "Account Created",
                getMessage: () =>
                    $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                    + $"Your account has been created in the <a href='{APP_PLACEHOLDER}'><u style='color:blue;'>LMS</u></a>.<br><br>"
                    + $"Here are the login details for your LMS account:<br><br>"
                    + $"Email: {EMAIL_PLACEHOLDER}<br>"
                    + $"Password: {PASSWORD_PLACEHOLDER}<br><br>"
                    + $"Please use the above login credentials to access your account.<br><br>"
                    + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                emailAddress,
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
                _placeholders[USER_NAME_PLACEHOLDER] = emailDto.FullName;
                _placeholders[PASSWORD_PLACEHOLDER] = emailDto.Password;
                _placeholders[EMAIL_PLACEHOLDER] = emailDto.Email;

                await SendEmailAsync(
                    getSubject: () => "Account Created",
                    getMessage: () =>
                        $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                        + $"Your account has been created in the <a href='{APP_PLACEHOLDER}'><u style='color:blue;'>LMS</u></a>.<br><br>"
                        + $"Here are the login details for your LMS account:<br><br>"
                        + $"Email: {EMAIL_PLACEHOLDER}<br>"
                        + $"Password: {PASSWORD_PLACEHOLDER}<br><br>"
                        + $"Please use the above login credentials to access your account.<br><br>"
                        + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                    emailDto.Email,
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

            _placeholders[GROUP_NAME_PLACEHOLDER] = groupName;
            _placeholders[GROUP_SLUG_PLACEHOLDER] = groupSlug;

            foreach (var user in users)
            {
                _placeholders[USER_NAME_PLACEHOLDER] = user.FirstName;

                var fullName = string.IsNullOrEmpty(user.MiddleName)
                    ? $"{user.FirstName} {user.LastName}"
                    : $"{user.FirstName} {user.MiddleName} {user.LastName}";

                await SendEmailAsync(
                    getSubject: () => "New Group Member",
                    getMessage: () =>
                        $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                        + $"You have been added to the {GROUP_NAME_PLACEHOLDER}. Now you can find the Training Materials which has been created for this group.<br><br>"
                        + $"Link to the group: <a href='{APP_PLACEHOLDER}/groups/{GROUP_SLUG_PLACEHOLDER}'><u style='color:blue;'>Click here</u></a><br><br>"
                        + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                    user.Email,
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
                _placeholders[GROUP_NAME_PLACEHOLDER] = group.Name;
                _placeholders[GROUP_SLUG_PLACEHOLDER] = group.Slug;

                foreach (var member in group.GroupMembers)
                {
                    _placeholders[USER_NAME_PLACEHOLDER] = member.User?.FirstName;

                    var fullName = string.IsNullOrEmpty(member.User?.MiddleName)
                        ? $"{member.User?.FirstName} {member.User?.LastName}"
                        : $"{member.User?.FirstName} {member.User?.MiddleName} {member.User?.LastName}";

                    await SendEmailAsync(
                        getSubject: () => "New training published",
                        getMessage: () =>
                            $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                            + $"You have new {TRAINING_NAME_PLACEHOLDER} training available for the {GROUP_NAME_PLACEHOLDER} group. "
                            + $"Please, go to {GROUP_NAME_PLACEHOLDER} group or "
                            + $"<a href='{APP_PLACEHOLDER}/trainings/{TRAINING_SLUG_PLACEHOLDER}'><u style='color:blue;'>Click Here</u></a> to find the training there.<br><br>"
                            + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                        member.User?.Email,
                        MailType.TrainingPublish
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

            _placeholders[TRAINING_NAME_PLACEHOLDER] = course.Name;
            _placeholders[TRAINING_SLUG_PLACEHOLDER] = course.Slug;

            foreach (var teacher in teachers)
            {
                _placeholders[USER_NAME_PLACEHOLDER] = teacher.FirstName;

                await SendEmailAsync(
                    getSubject: () => "New Training Enrollment",
                    getMessage: () =>
                        $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                        + $"A new user has enrolled in your {TRAINING_NAME_PLACEHOLDER} course. Here are the details:"
                        + $"<ul><li>Training: {TRAINING_NAME_PLACEHOLDER}</li><li>Enrolled User: {USER_NAME_PLACEHOLDER}</li><li>User Email: {EMAIL_PLACEHOLDER}</li></ul>"
                        + $"Thank you for your attention to this enrollment. We appreciate your dedication to providing an exceptional learning experience.<br><br>"
                        + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                    teacher.Email,
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

            _placeholders[TRAINING_NAME_PLACEHOLDER] = courseName;

            foreach (var user in certificateUserIssuedDtos)
            {
                _placeholders[USER_NAME_PLACEHOLDER] = user.UserName;

                await SendEmailAsync(
                    getSubject: () => "Certificate Issued",
                    getMessage: () =>
                        $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                        + $"We are happy to inform you that your Certificate of Achievement for {TRAINING_NAME_PLACEHOLDER} has been issued "
                        + $"and is now available in your profile on the application. "
                        + $"Please log in to your account and navigate to your profile to view and download your certificate.<br><br>"
                        + $"We hope you find the training helpful.<br><br>"
                        + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                    user?.Email,
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

            _placeholders[TRAINING_NAME_PLACEHOLDER] = courseName;
            _placeholders[TRAINING_SLUG_PLACEHOLDER] = courseSlug;

            foreach (var user in enrolledUsers)
            {
                _placeholders[USER_NAME_PLACEHOLDER] = user.FirstName;

                await SendEmailAsync(
                    getSubject: () => "New Content Added",
                    getMessage: () =>
                        $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                        + $"Your enrolled training entitled <a href='{APP_PLACEHOLDER}/trainings/{TRAINING_SLUG_PLACEHOLDER}'>"
                        + $"<u style='color:blue;'>{TRAINING_NAME_PLACEHOLDER}</u></a> has been updated with new content. "
                        + $"We encourage you to visit the training page and "
                        + $"explore the new materials to enhance your learning experience.<br><br>"
                        + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                    user.Email,
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

            _placeholders[USER_NAME_PLACEHOLDER] = fullName;
            _placeholders[EMAIL_PLACEHOLDER] = newEmail;

            await SendEmailAsync(
                getSubject: () => "Notification: Email Address Change",
                getMessage: () =>
                    $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                    + $"A recent change has been made to the email address associated with your account to {EMAIL_PLACEHOLDER}.<br>"
                    + $"Please check your email for the login credentials. If you encounter any difficulties, "
                    + $"please contact your administrator immediately.<br><br>"
                    + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                oldEmail,
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

            _placeholders[USER_NAME_PLACEHOLDER] = assessment.User.FirstName;
            _placeholders[ASSESSMENT_TITLE_PLACEHOLDER] = assessment.Title;

            await SendEmailAsync(
                getSubject: () => $"Assessment Review Status",
                getMessage: () =>
                    $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                    + $"Assessment <a href='{APP_PLACEHOLDER}/settings/courses'>{ASSESSMENT_TITLE_PLACEHOLDER}</a> published successfully. Thank you.<br><br>"
                    + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                assessment.User.Email
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

            _placeholders[USER_NAME_PLACEHOLDER] = assessment.User.FirstName;
            _placeholders[ASSESSMENT_TITLE_PLACEHOLDER] = assessment.Title;
            _placeholders[MESSAGE_PLACEHOLDER] = assessment.Message;

            await SendEmailAsync(
                getSubject: () => $"Assessment Review Status",
                getMessage: () =>
                    $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                    + $"We regret to inform you that your Assessment, {ASSESSMENT_TITLE_PLACEHOLDER} has been rejected for the following reason:<br><br>"
                    + $"{MESSAGE_PLACEHOLDER}<br><br>"
                    + $"However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. "
                    + $"Once you have addressed the identified issues, please resubmit the assessment for further review.<br><br>"
                    + $"Thank you for your understanding and cooperation.<br><br>"
                    + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                assessment.User.Email
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

            _placeholders[ASSESSMENT_TITLE_PLACEHOLDER] = assessment.Title;

            foreach (var admin in admins)
            {
                _placeholders[USER_NAME_PLACEHOLDER] = admin.FirstName;
                await SendEmailAsync(
                    getSubject: () => $"Assessment Review Request",
                    getMessage: () =>
                        $"Dear {USER_NAME_PLACEHOLDER},<br><br>"
                        + $"Assessment <a href='{APP_PLACEHOLDER}/settings/courses'>{ASSESSMENT_TITLE_PLACEHOLDER}</a> is requested for review. Thank you.<br><br>"
                        + $"{EMAIL_SIGNATURE_PLACEHOLDER}",
                    admin.Email
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
    /// <param name="mailType">The type of mail being sent.</param>
    private async Task SendEmailAsync(
        Func<string> getSubject,
        Func<string> getMessage,
        string recipientEmail,
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
            model.Subject = getSubject();
            model.Message = getMessage();
        }
        else
        {
            model.Subject = template.Subject;
            model.Message = template.Message;
        }

        _placeholders[COMPANY_ADDRESS_PLACEHOLDER] = setting?.CompanyAddress ?? string.Empty;
        _placeholders[COMPANY_NAME_PLACEHOLDER] = setting?.CompanyName ?? string.Empty;
        _placeholders[COMPANY_NUMBER_PLACEHOLDER] = setting?.CompanyContactNumber ?? string.Empty;
        _placeholders[EMAIL_SIGNATURE_PLACEHOLDER] =
            setting != null && !string.IsNullOrWhiteSpace(setting.EmailSignature)
                ? setting.EmailSignature
                : $"Best regards,<br>{COMPANY_NAME_PLACEHOLDER}";

        foreach (var placeholder in _placeholders)
        {
            model.Message = model.Message.Replace(placeholder.Key, placeholder.Value);
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
