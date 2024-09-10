using System.Text;
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
using static Dapper.SqlMapper;

namespace AcademyKit.Infrastructure.Services;

public class HangfireJobService : BaseService, IHangfireJobService
{
    private readonly string _appUrl;
    private readonly IEmailService _emailService;
    private readonly IVideoService _videoService;
    private readonly IFileServerService _fileServerService;

    private readonly string _userName = "{userName}";
    private readonly string _app = "{appUrl}";
    private readonly string _courseName = "{courseName}";
    private readonly string _courseSlug = "{courseSlug}";
    private readonly string _emailSignature = "{emailSignature}";
    private readonly string _companyName = "{companyName}";
    private readonly string _companyNumber = "{companyNumber}";
    private readonly string _password = "{password}";
    private readonly string _email = "{email}";
    private readonly string _message = "{message}";
    private readonly string _groupName = "{groupName}";
    private readonly string _groupSlug = "{groupSlug}";
    private readonly string _newEmail = "{newEmail}";

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
    }

    /// <summary>
    /// Handle to send course review mail
    /// </summary>
    /// <param name="courseName"> the course name </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
    /// <returns> the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendCourseReviewMailAsync(string courseName, PerformContext context = null)
    {
        try
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

            var setting = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);

            var template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.MailType == MailType.TrainingReview && p.IsActive
                )
                .ConfigureAwait(false);

            var commonHtml =
                $"Training "
                + @$"<a href = '{_appUrl}/settings/courses'>""{courseName}""</a> is under review. Kindly provide feedback and assessment. Your input is vital for quality assurance. Thank you.<br><br>";
            foreach (var user in users)
            {
                var model = new EmailRequestDto { To = user.Email };

                if (template == null)
                {
                    var html = $"Dear {user.FirstName},<br><br>";
                    html += commonHtml;
                    html += $"Best regards, <br> {setting.CompanyName}";

                    model.Subject = $"Training Review Status - {courseName}";
                    model.Message = html;
                }
                else
                {
                    model.Subject = template.Subject;
                    model.Message = template
                        .Message.Replace(_userName, user.FirstName)
                        .Replace(_app, _appUrl)
                        .Replace(_courseName, courseName)
                        .Replace(_emailSignature, $"Best regards, <br> {setting.CompanyName}");
                }

                await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Handle to send course rejected mail
    /// </summary>
    /// <param name="courseId"> the course id </param>
    /// <param name="message"> the message </param>
    /// <param name="context"> the instance of <see cref="PerformContext" /> .</param>
    /// <returns> the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task CourseRejectedMailAsync(
        Guid courseId,
        string message,
        PerformContext context = null
    )
    {
        try
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

            var template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.MailType == MailType.TrainingReject && p.IsActive
                )
                .ConfigureAwait(false);

            var setting = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();

            var commonHtml =
                $"We regret to inform you that your training, {course.Name} has been rejected for the following reason:<br><br>"
                + $"{message}<br><br>"
                + $"However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. "
                + $"Once you have addressed the identified issues, please resubmit the training program for further review.<br><br>"
                + $"Thank you for your understanding and cooperation.<br><br>"
                + $"Best regards,<br> {setting.CompanyName}";

            foreach (var teacher in course.CourseTeachers)
            {
                if (!string.IsNullOrEmpty(teacher.User?.Email))
                {
                    var model = new EmailRequestDto { To = teacher.User.Email, };
                    if (template == null)
                    {
                        var html = $"Dear {teacher?.User.FirstName},<br><br>";
                        html += commonHtml;

                        model.Subject = $"Training Rejection - {course.Name}";
                        model.Message = html;
                    }
                    else
                    {
                        model.Subject = template.Subject;
                        model.Message = template
                            .Message.Replace(_userName, teacher?.User.FirstName)
                            .Replace(_courseName, course.Name)
                            .Replace(_message, message)
                            .Replace(_emailSignature, $"Best regards, <br> {setting.CompanyName}");
                    }
                    await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Email for account created and password
    /// </summary>
    /// <param name="emailAddress">the email address of the receiver</param>
    /// <param name="firstName">the first name of the receiver</param>
    /// <param name="password">the login password of the receiver</param>
    /// <param name="companyName"> the company name </param>
    /// <returns></returns>
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
        try
        {
            if (context == null)
            {
                throw new ArgumentException("Context not found.");
            }

            var template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.MailType == MailType.UserCreate && p.IsActive
                )
                .ConfigureAwait(false);

            var html =
                $"Dear {firstName},<br><br>"
                + $"Your account has been created in the <a href = '{_appUrl}'><u  style='color:blue;'>LMS</u></a>.<br><br>"
                + $"Here are the login details for your LMS account:<br><br>"
                + $"Email:{emailAddress}<br>"
                + $"Password:{password}<br><br>"
                + $"Please use the above login credentials to access your account.<br><br>"
                + $"Best regards,<br> {companyName}<br>{companyNumber}";

            var mail = new EmailRequestDto { To = emailAddress, };
            if (template == null)
            {
                mail.Subject = "Account Created";
                mail.Message = html;
            }
            else
            {
                mail.Subject = template.Subject;
                mail.Message = template
                    .Message.Replace(_userName, firstName)
                    .Replace(_app, _appUrl)
                    .Replace(_email, emailAddress)
                    .Replace(_password, password)
                    .Replace(_companyName, companyName)
                    .Replace(_companyNumber, companyNumber);
            }

            await _emailService.SendMailWithHtmlBodyAsync(mail).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while attempting to send change email address mail."
            );
        }
    }

    /// <summary>
    /// Handle to send email to imported user async
    /// </summary>
    /// <param name="dtos"> the list of <see cref="UserEmailDto" /> .</param>
    /// <returns> the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendEmailImportedUserAsync(
        IList<UserEmailDto> dtos,
        PerformContext context = null
    )
    {
        try
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.MailType == MailType.UserCreate && p.IsActive
                )
                .ConfigureAwait(false);

            foreach (var emailDto in dtos)
            {
                var model = new EmailRequestDto { To = emailDto.Email, };
                if (template == null)
                {
                    var html =
                        $"Dear {emailDto.FullName},<br><br>"
                        + $"Your account has been created in the <a href = '{_appUrl}'><u  style='color:blue;'>LMS</u></a>.<br><br>"
                        + $"Here are the login details for your LMS account:<br><br>"
                        + $"Email:{emailDto.Email}<br>"
                        + $"Password:{emailDto.Password}<br><br>"
                        + $"Please use the above login credentials to access your account.<br><br>"
                        + $"Best regards,<br> {emailDto.CompanyName}<br>{emailDto.CompanyNumber}";

                    model.Subject = "Account Created";
                    model.Message = html;
                }
                else
                {
                    model.Subject = template.Subject;
                    model.Message = template
                        .Message.Replace(_userName, emailDto.FullName)
                        .Replace(_app, _appUrl)
                        .Replace(_email, emailDto.Email)
                        .Replace(_password, emailDto.Password)
                        .Replace(_companyName, emailDto.CompanyName)
                        .Replace(_companyNumber, emailDto.CompanyNumber);
                }

                await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Handle to update information of lesson video
    /// </summary>
    /// <param name="lessonId"> the lesson id </param>
    /// <param name="videoUrl"> the video url </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/></param>
    /// <returns> the task complete </returns>
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
            if (lesson.Type != LessonType.Video || lesson.Type != LessonType.RecordedVideo)
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

    /// <summary>
    /// Handle to send mail to new group member
    /// </summary>
    /// <param name="groupName"> the group name </param>
    /// <param name="groupSlug"> the group slug </param>
    /// <param name="userIds"> the list of <see cref="Guid" /> .</param>
    /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
    /// <returns> the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendMailNewGroupMember(
        string groupName,
        string groupSlug,
        IList<Guid> userIds,
        PerformContext context = null
    )
    {
        try
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
            var template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.MailType == MailType.GroupMemberAdd && p.IsActive
                )
                .ConfigureAwait(false);

            var setting = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();
            var commonHtml =
                $"You have been added to the {groupName}. Now you can find the Training Materials which has been created for this              group. <br><br>"
                + $"Link to the group : <a href = '{_appUrl}/groups/{groupSlug}' ><u  style='color:blue;'> Click here </u> </a>"
                + $"<br>Thank You, <br> {setting.CompanyName}";
            foreach (var user in users)
            {
                var fullName = string.IsNullOrEmpty(user.MiddleName)
                    ? $"{user.FirstName} {user.LastName}"
                    : $"{user.FirstName} {user.MiddleName} {user.LastName}";

                var model = new EmailRequestDto { To = user.Email, };
                if (template == null)
                {
                    var html = $"Dear {fullName},<br><br>";
                    html += commonHtml;

                    model.Subject = "New Group Member";
                    model.Message = html;
                }
                else
                {
                    model.Subject = template.Subject;
                    model.Message = template
                        .Message.Replace(_userName, fullName)
                        .Replace(_groupName, groupName)
                        .Replace(_app, _appUrl)
                        .Replace(_groupSlug, groupSlug)
                        .Replace(_emailSignature, $"<br>Thank You, <br> {setting.CompanyName}");
                }

                await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Handle to send group course published mail
    /// </summary>
    /// <param name="groupId"> the group id</param>
    /// <param name="courseName"> the course name </param>
    /// <param name="courseSlug"> the course slug </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/></param>
    /// <returns> the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task GroupCoursePublishedMailAsync(
        Guid groupId,
        string courseName,
        string courseSlug,
        PerformContext context = null
    )
    {
        try
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var settings = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();
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
                    var html = $"Dear {fullName},<br><br>";
                    html +=
                        $"You have new {courseName} training available for the {group.Name} group. Please, go to {group.Name} group or "
                        + @$"<a href ='{_appUrl}/trainings/{courseSlug}'><u style='color:blue;'>Click Here </u></a> to find the training there. <br>";
                    html += $"<br><br>Thank You, <br> {settings.CompanyName}";
                    var model = new EmailRequestDto
                    {
                        To = member.User?.Email,
                        Subject = "New training published",
                        Message = html,
                    };
                    await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    ///<summary>
    ///Handle to send course enrollment mail
    ///</summary>
    ///<param name="courseName"> the course name</param>
    ///<param name="UserId">the list of <see cref="Guid" /></param>
    ////// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
    ///<returns>the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendCourseEnrollmentMailAsync(
        string userName,
        string userEmail,
        Guid courseId,
        string courseName,
        PerformContext context = null
    )
    {
        try
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

            var setting = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();

            var template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.MailType == MailType.TrainingEnrollment && p.IsActive
                )
                .ConfigureAwait(false);

            var commonHtml =
                $"A new user has enrolled in your {courseName} course. Here are the details:"
                + $"<ul><li>Training: {courseName}</li><li>Enrolled User: {userName}</li> <li>User Email:{userEmail}</li></ul>"
                + $"Thank you for your attention to this enrollment. We appreciate your dedication to providing an exceptional learning experience.<br><br>"
                + $"Best regards, <br> {setting.CompanyName}";
            foreach (var teacher in course.CourseTeachers)
            {
                var model = new EmailRequestDto { To = teacher.User?.Email, };
                if (template == null)
                {
                    var html = $"Dear {teacher.User.FirstName},<br><br>";
                    html += commonHtml;

                    model.Subject = "New Training Enrollment";
                    model.Message = html;
                }
                else
                {
                    model.Subject = template.Subject;
                    model.Message = template
                        .Message.Replace(_userName, teacher.User.FirstName)
                        .Replace(_courseName, courseName)
                        .Replace(_userName, userName)
                        .Replace(_email, userEmail)
                        .Replace(_emailSignature, $"Best regards, <br> {setting.CompanyName}");
                }

                await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Handle to send user certificate issue mail
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="courseName"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendCertificateIssueMailAsync(
        string courseName,
        IList<CertificateUserIssuedDto> certificateUserIssuedDtos,
        PerformContext context = null
    )
    {
        try
        {
            if (context == null)
            {
                throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
            }

            var setting = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();

            var template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.MailType == MailType.CertificateIssue && p.IsActive
                )
                .ConfigureAwait(false);

            var commonHtml =
                $"We are happy to inform you that your Certificate of Achievement for {courseName} has been issued "
                + $"and is now available in your profile on the application."
                + $"Please log in to your account and navigate to your profile to view and download your certificate.<br><br>"
                + $"we hope you find the training helpful.<br><br>"
                + $"Best regards, <br> {setting.CompanyName}";

            foreach (var user in certificateUserIssuedDtos)
            {
                var model = new EmailRequestDto { To = user?.Email, };
                if (template == null)
                {
                    var html = $"Dear {user.UserName},<br><br>";
                    html += commonHtml;

                    model.Subject = "Certificate Issued";
                    model.Message = html;
                }
                else
                {
                    model.Subject = template.Subject;
                    model.Message = template
                        .Message.Replace(_userName, user.UserName)
                        .Replace(_courseName, courseName)
                        .Replace(_emailSignature, $"Best regards, <br> {setting.CompanyName}");
                }

                await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// handle to send lesson edit email
    /// </summary>
    /// <param name="courseName"></param>
    /// <param name="courseSlug"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendLessonAddedMailAsync(
        string courseName,
        string courseSlug,
        PerformContext context = null
    )
    {
        try
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

            var setting = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();

            var template = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.MailType == MailType.AddLesson && p.IsActive
                )
                .ConfigureAwait(false);

            var commonHtml =
                $"Your enrolled training entitled  <a href='{_appUrl}/trainings/{courseSlug}'>"
                + $"<u style='color:blue;'>'{courseName}'</u></a>  has been updated with new content. "
                + $"We encourage you to visit the training page and "
                + $"explore the new materials to enhance your learning experience.<br><br>"
                + $"<br><br>Thank You, <br> {setting.CompanyName}";

            foreach (var users in course.CourseEnrollments.AsList())
            {
                var firstName = users.User.FirstName;
                var model = new EmailRequestDto { To = users.User?.Email, };

                if (template == null)
                {
                    var html = $"Dear {firstName},<br><br>";
                    html += commonHtml;

                    model.Subject = "New Content";
                    model.Message = html;
                }
                else
                {
                    model.Subject = template.Subject;
                    model.Message = template
                        .Message.Replace(_userName, firstName)
                        .Replace(_app, _appUrl)
                        .Replace(_courseSlug, courseSlug)
                        .Replace(_courseName, courseName)
                        .Replace(_emailSignature, $"<br><br>Thank You, <br> {setting.CompanyName}");
                }

                await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// handel to send email update mail
    /// </summary>
    /// <param name="fullName">Users full name</param>
    /// <param name="newEmail">Users new email</param>
    /// <param name="oldEmail">Users old email</param>
    /// <param name="context"></param>
    /// <returns></returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task AccountUpdatedMailAsync(
        string fullName,
        string newEmail,
        string oldEmail,
        PerformContext context = null
    )
    {
        try
        {
            if (context == null)
            {
                throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
            }

            await SendChangeEmailAsync(fullName, newEmail, oldEmail).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Hand to send the change email
    /// </summary>
    /// <param name="fullName">the user full name</param>
    /// <param name="newEmail">the new changed email</param>
    /// <param name="oldEmail">the old email</param>
    /// <returns>the task completes</returns>
    private async Task SendChangeEmailAsync(string fullName, string newEmail, string oldEmail)
    {
        var setting = await _unitOfWork
            .GetRepository<GeneralSetting>()
            .GetFirstOrDefaultAsync()
            .ConfigureAwait(false);

        var template = await _unitOfWork
            .GetRepository<MailNotification>()
            .GetFirstOrDefaultAsync(predicate: p =>
                p.MailType == MailType.ChangedEmail && p.IsActive
            )
            .ConfigureAwait(false);

        var model = new EmailRequestDto { To = oldEmail, };
        if (template == null)
        {
            var html =
                $"Dear {fullName}<br><br>"
                + $"A recent change has been made to the email address associated with your account to {newEmail}<br>."
                + $"Please check your email for the login credentials. If you encounter any difficulties, "
                + $"please contact your administrator immediately."
                + $"<br><br>Thank You, <br> {setting.CompanyName}";

            model.Subject = "Notification: Email Address Change";
            model.Message = html;
        }
        else
        {
            model.Subject = template.Subject;
            model.Message = template
                .Message.Replace(_userName, fullName)
                .Replace(_newEmail, newEmail)
                .Replace(_emailSignature, $"<br><br>Thank You, <br> {setting.CompanyName}");
        }
        await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
    }

    /// <summary>
    /// Used to send email send mail
    /// </summary>
    /// <param name="newEmail">user's new email id</param>
    /// <param name="oldEmail">user's old email </param>
    /// <param name="fullName">users full name</param>
    /// <param name="context"></param>
    /// <returns></returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendEmailChangedMailAsync(
        string newEmail,
        string oldEmail,
        string fullName,
        PerformContext context = null
    )
    {
        try
        {
            if (context == null)
            {
                throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
            }

            await SendChangeEmailAsync(fullName, newEmail, oldEmail).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Handle to send course review mail
    /// </summary>
    /// <param name="assessmentId"> the course name </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
    /// <returns> the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendAssessmentAcceptMailAsync(
        Guid assessmentId,
        PerformContext context = null
    )
    {
        try
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

            var settings = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();

            var html = $"Dear {assessment.User.FirstName},<br><br>";
            html +=
                $"Assessment "
                + @$"<a href = '{_appUrl}/settings/courses'>""{assessment.Title}""</a> published successfully. Thank you.<br><br>";
            html += $"Best regards, <br> {settings.CompanyName}";
            var model = new EmailRequestDto
            {
                To = assessment.User.Email,
                Subject = $"Assessment Review Status - {assessment.AssessmentStatus}",
                Message = html
            };
            await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Handle to send course review mail
    /// </summary>
    /// <param name="assessmentId"> the course name </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
    /// <returns> the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendAssessmentRejectMailAsync(
        Guid assessmentId,
        PerformContext context = null
    )
    {
        try
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

            var settings = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();
            var html = $"Dear {assessment?.User.FirstName},<br><br>";
            html +=
                $"We regret to inform you that your Assessment, {assessment.Title} has been rejected for the following reason:<br><br>";
            html += $"{assessment.Message}<br><br>";
            html +=
                $"However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. Once you have addressed the identified issues, please resubmit the training program for further review.<br><br>";
            html += $"Thank you for your understanding and cooperation.<br><br>";
            html += $"Best regards,<br> {settings.CompanyName}";
            var model = new EmailRequestDto
            {
                To = assessment.User.Email,
                Subject = $"Assessment Review Status - {assessment.AssessmentStatus}",
                Message = html
            };
            await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Handle to send course review mail
    /// </summary>
    /// <param name="assessmentId"> the course name </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
    /// <returns> the task complete </returns>
    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task SendAssessmentReviewMailAsync(
        Guid assessmentId,
        IList<User> user,
        PerformContext context = null
    )
    {
        try
        {
            if (context == null)
            {
                throw new ArgumentException(_localizer.GetString("ContextNotFound"));
            }

            var assessment = await _unitOfWork
                .GetRepository<Assessment>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Id == assessmentId,
                    include: source =>
                        source
                            .Where(x =>
                                x.User.Role == UserRole.SuperAdmin || x.User.Role == UserRole.Admin
                            )
                            .Include(x => x.User)
                )
                .ConfigureAwait(false);
            if (assessment.Id != assessmentId)
            {
                return;
            }

            // all admins and super admin
            var users = user.Where(x => x.Role == UserRole.SuperAdmin || x.Role == UserRole.Admin);

            var settings = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync();

            foreach (var assessments in users)
            {
                var firstName = assessments.FirstName;
                var emails = assessments.Email;

                var html = $"Dear {firstName},<br><br>";
                html +=
                    $"Assessment "
                    + @$"<a href = '{_appUrl}/settings/courses'>""{assessment.Title}""</a> is requested for review. Thank you.<br><br>";
                html += $"Best regards, <br> {settings.CompanyName}";

                var model = new EmailRequestDto
                {
                    To = emails,
                    Subject = $"Request for Assessment Review - {assessment.AssessmentStatus}",
                    Message = html
                };
                await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }
}
