namespace Lingtren.Infrastructure.Services
{
    using System;
    using System.Text;
    using Hangfire;
    using Hangfire.Server;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using static Dapper.SqlMapper;

    public class HangfireJobService : BaseService, IHangfireJobService
    {
        private readonly string _appUrl;
        private readonly IEmailService _emailService;
        private readonly IVideoService _videoService;
        private readonly IFileServerService _fileServerService;

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
        public async Task SendCourseReviewMailAsync(
            string courseName,
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
                    .GetAllAsync(predicate: p =>
                        p.Role == UserRole.Admin || p.Role == UserRole.SuperAdmin
                    )
                    .ConfigureAwait(false);
                if (users.Count == default)
                {
                    return;
                }

                var settings = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync();
                foreach (var user in users)
                {
                    var html = $"Dear {user.FirstName},<br><br>";
                    html +=
                        $"Training "
                        + @$"<a href = '{_appUrl}/settings/courses'>""{courseName}""</a> is under review. Kindly provide feedback and assessment. Your input is vital for quality assurance. Thank you.<br><br>";
                    html += $"Best regards, <br> {settings.CompanyName}";
                    var model = new EmailRequestDto
                    {
                        To = user.Email,
                        Subject = $"Training Review Status - {courseName}",
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

                var settings = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync();
                foreach (var teacher in course.CourseTeachers)
                {
                    if (!string.IsNullOrEmpty(teacher.User?.Email))
                    {
                        var html = $"Dear {teacher?.User.FirstName},<br><br>";
                        html +=
                            $"We regret to inform you that your training, {course.Name} has been rejected for the following reason:<br><br>";
                        html += $"{message}<br><br>";
                        html +=
                            $"However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. Once you have addressed the identified issues, please resubmit the training program for further review.<br><br>";
                        html += $"Thank you for your understanding and cooperation.<br><br>";
                        html += $"Best regards,<br> {settings.CompanyName}";

                        var model = new EmailRequestDto
                        {
                            To = teacher.User.Email,
                            Subject = "Training Rejection",
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

                var html = $"Dear {firstName},<br><br>";
                html +=
                    $@"Your account has been created in the <a href = '{_appUrl}'><u  style='color:blue;'>LMS</u></a>.<br><br>";
                html += "Here are the login details for your LMS account:<br><br>";
                html += $"Email:{emailAddress}<br>";
                html += $"Password:{password}<br><br>";
                html += $"Please use the above login credentials to access your account.<br><br>";
                html += $"Best regards,<br> {companyName}<br>{companyNumber}";
                var mail = new EmailRequestDto
                {
                    To = emailAddress,
                    Subject = "Account Created",
                    Message = html
                };
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

                foreach (var emailDto in dtos)
                {
                    var html = $"Dear {emailDto.FullName},<br><br>";
                    html +=
                        $@"Your account has been created in the <a href = '{_appUrl}'><u  style='color:blue;'>LMS</u></a>.<br><br>";
                    html += "Here are the login details for your LMS account:<br><br>";
                    html += $"Email:{emailDto.Email}<br>";
                    html += $"Password:{emailDto.Password}<br><br>";
                    html +=
                        $"Please use the above login credentials to access your account.<br><br>";
                    html += $"Best regards,<br> {emailDto.CompanyName}<br>{emailDto.CompanyNumber}";
                    var model = new EmailRequestDto
                    {
                        To = emailDto.Email,
                        Message = html,
                        Subject = "Account Created"
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

                    var vidoePath = await _fileServerService
                        .GetFileLocalPathAsync(lesson.VideoUrl)
                        .ConfigureAwait(true);
                    var duration = await _videoService
                        .GetVideoDuration(vidoePath)
                        .ConfigureAwait(true);
                    lesson.Duration = duration;
                    _unitOfWork.GetRepository<Lesson>().Update(lesson);
                    _videoService.DeleteTempFile(vidoePath);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                }
            });
        }

        /// <summary>
        /// Handle to send mail to new group member
        /// </summary>
        /// <param name="gropName"> the group name </param>
        /// <param name="groupSlug"> the group slug </param>
        /// <param name="userIds"> the list of <see cref="Guid" /> .</param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
        /// <returns> the task complete </returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SendMailNewGroupMember(
            string gropName,
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

                var settings = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync();

                foreach (var user in users)
                {
                    var fullName = string.IsNullOrEmpty(user.MiddleName)
                        ? $"{user.FirstName} {user.LastName}"
                        : $"{user.FirstName} {user.MiddleName} {user.LastName}";
                    var html = $"Dear {fullName},<br><br>";
                    html +=
                        $"You have been added to the {gropName}. Now you can find the Training Materials which has been created for this {gropName}. <br><br>";
                    html += $"Link to the group :";
                    html +=
                        $"<a href = '{_appUrl}/groups/{groupSlug}' ><u  style='color:blue;'> Click here </u> </a>";
                    html += $"<br>Thank You, <br> {settings.CompanyName}";

                    var model = new EmailRequestDto
                    {
                        To = user.Email,
                        Subject = "New group member",
                        Message = html,
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
                    .GetRepository<Group>()
                    .GetFirstOrDefaultAsync(
                        predicate: x => x.Id == groupId,
                        include: source =>
                            source.Include(x => x.GroupMembers).ThenInclude(x => x.User)
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
                            + @$"<a href ='{_appUrl}/trainings/{courseSlug}'><u  style='color:blue;'>Click Here </u></a> to find the training there. <br>";
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
        ///<param name="coursename"> the course name</param>
        ///<param name="UserId">the list of <see cref="Guid" /></param>
        ////// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
        ///<returns>the tasl complete </returns>
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
                var settings = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync();

                if (course.CourseTeachers == null)
                {
                    throw new ArgumentException(_localizer.GetString("TeacherNotFound"));
                }

                foreach (var teacher in course.CourseTeachers)
                {
                    var html = $"Dear {teacher.User.FirstName},<br><br>";
                    html +=
                        $"A new user has enrolled in your {courseName} course. Here are the details:";
                    html +=
                        $"<ul><li>Training: {courseName}</li><li>Enrolled User: {userName}</li> <li>User Email:{userEmail}</li></ul>";
                    html +=
                        $"Thank you for your attention to this enrollment. We appreciate your dedication to providing an exceptional learning experience.<br>";
                    html += $"<br><br>Best regards, <br> {settings.CompanyName}";
                    var model = new EmailRequestDto
                    {
                        To = teacher.User?.Email,
                        Subject = "New Enrollment",
                        Message = html,
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

                var settings = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync();

                foreach (var user in certificateUserIssuedDtos)
                {
                    var fullName = user.UserName;
                    var html = $"Dear {fullName},<br><br>";
                    html +=
                        $"We are happy to inform you that your Certificate of Achievement for {courseName} has been issued and is now available in your profile on the application. "
                        + "Please log in to your account and navigate to your profile to view and download your certificate.<br><br>";
                    html += $"we hope you find the training helpful.<br><br>";
                    html += $"Best regards, <br> {settings.CompanyName}";

                    var model = new EmailRequestDto
                    {
                        To = user?.Email,
                        Subject = "Certificate Issued",
                        Message = html,
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

                var settings = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync();
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

                foreach (var users in course.CourseEnrollments.AsList())
                {
                    var firstName = users.User.FirstName;
                    var html = $"Dear {firstName},<br><br>";
                    html +=
                        @$"Your enrolled training entitled  <a href='{_appUrl}/trainings/{courseSlug}' ><u  style='color:blue;'>'{courseName}'</u></a>  has been updated with new content. We encourage you to visit the training page and "
                        + $"explore the new materials to enhance your learning experience.<br><br>";

                    html += $"<br><br>Thank You, <br> {settings.CompanyName}";
                    var model = new EmailRequestDto
                    {
                        To = users.User?.Email,
                        Subject = "New content",
                        Message = html,
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

        /// <summary>
        /// handel to send email update mail
        /// </summary>
        /// <param name="fullName">Users full name</param>
        /// <param name="Newemail">Users new email</param>
        /// <param name="oldEmail">Users old email</param>
        /// <param name="context"></param>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task AccountUpdatedMailAsync(
            string fullName,
            string Newemail,
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

                var settings = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync()
                    .ConfigureAwait(false);
                var html = $"Dear {fullName}<br><br>";
                html +=
                    @$"A recent change has been made to the email address associated with your account to {Newemail}<br>.Please check your email for the login credentials. If you encounter any difficulties, please contact your administrator immediately.";
                html += $"<br><br>Thank You, <br> {settings.CompanyName}";
                var model = new EmailRequestDto
                {
                    To = oldEmail,
                    Subject = "Notification: Email Address Change",
                    Message = html,
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

                var settings = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync()
                    .ConfigureAwait(false);
                var html = $"Dear {fullName} <br><br>";
                html +=
                    $"A recent change has been made to the email address associated with your account to {newEmail}.Please check your email to verify the email address.If you did not initiate this change, please contact your administrator immediately to address the issue.";
                html += $"<br><br>Best regards, <br> {settings.CompanyName}";
                var model = new EmailRequestDto
                {
                    To = oldEmail,
                    Subject = "Notification: Email Address Change",
                    Message = html,
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
                                    x.User.Role == UserRole.SuperAdmin
                                    || x.User.Role == UserRole.Admin
                                )
                                .Include(x => x.User)
                    )
                    .ConfigureAwait(false);
                if (assessment.Id != assessmentId)
                {
                    return;
                }

                // all admins and super admin
                var users = user.Where(x =>
                    x.Role == UserRole.SuperAdmin || x.Role == UserRole.Admin
                );

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
}
