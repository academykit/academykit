﻿namespace AcademyKit.Infrastructure.Services
{
    using System;
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

        private void ValidateContext(PerformContext context)
        {
            if (context == null) throw new ArgumentException(_localizer.GetString("ContextNotFound"));
        }

        private async Task<IList<User>> GetUsersByRole(params UserRole[] roles)
        {
            return await _unitOfWork.GetRepository<User>()
                .GetAllAsync(predicate: p => roles.Contains(p.Role))
                .ConfigureAwait(false);
        }

        private async Task<MailNotification> GetMailNotification(MailType mailType)
        {
            return await _unitOfWork.GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(predicate: p => p.MailType == mailType)
                .ConfigureAwait(false);
        }

        private static string ReplacePlaceholders(string template, string userName, string courseName, string companyName)
        {
            return template.Replace("{UserName}", userName)
                           .Replace("{CourseName}", courseName)
                           .Replace("{EmailSignature}", $"Best regards,<br>{companyName}");
        }

        private static string BuildUserCreatedPasswordEmailHtml(string firstName, string emailAddress, string password, string companyName, string companyNumber)
        {
            return $"Dear {firstName},<br><br>" +
                   $"Your account has been created in the LMS.<br>" +
                   $"Here are the login details:<br>Email: {emailAddress}<br>Password: {password}<br><br>" +
                   $"Best regards,<br>{companyName}<br>{companyNumber}";
        }

        private async Task<GeneralSetting> GetGeneralSettings()
        {
            return await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();
        }

        private async Task<Course> GetCourseById(Guid courseId)
        {
            return await _unitOfWork.GetRepository<Course>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == courseId, include: source => source.Include(x => x.CourseTeachers).ThenInclude(x => x.User))
                .ConfigureAwait(false);
        }

        private static string BuildUserCreatedPasswordEmailHtml(string firstName, string emailAddress, string password, string companyName, string companyNumber)
        {
            return $"Dear {firstName},<br><br>" +
              $"Your account has been created in the <a href='{_appUrl}'><u style='color:blue;'>LMS</u></a>.<br><br>" +
              "Here are the login details for your LMS account:<br><br>" +
              $"Email: {emailAddress}<br>" +
              $"Password: {password}<br><br>" +
              $"Please use the above login credentials to access your account.<br><br>" +
              $"Best regards,<br>{companyName}<br>{companyNumber}";

        }

        private static string BuildCourseRejectedEmailHtml(string firstName, string courseName, string reason, string companyName)
        {
            return $"Dear {firstName},<br><br>" +
                   $"We regret to inform you that your training, {courseName} has been rejected for the following reason:<br>{reason}<br><br>" +
                   "Please make the necessary corrections and resubmit.<br><br>" +
                   $"Best regards,<br>{companyName}";
        }

        private string BuildCourseReviewEmailHtml(string firstName, string courseName, string companyName)
        {
            return $"Dear {firstName},<br><br>" +
                   $"Training <a href = '{_appUrl}/settings/courses'>{courseName}</a> is under review. Kindly provide feedback and assessment. " +
                   "Your input is vital for quality assurance. Thank you.<br><br>" +
                   $"Best regards,<br>{companyName}";
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
                ValidateContext(context);

                var users = await GetUsersByRole(UserRole.Admin, UserRole.SuperAdmin);
                if (users.Count == default) return;

                var mailNotification = await GetMailNotification(MailType.TrainingReview);
                var settings = await GetGeneralSettings();

                foreach (var user in users)
                {
                    var html = mailNotification is null
                    ? BuildCourseReviewEmailHtml(user.FirstName, courseName, settings.CompanyName)
                    : ReplacePlaceholders(mailNotification.Message, user.FirstName, courseName, settings.CompanyName);

                    var model = new EmailRequestDto
                    {
                        To = user.Email,
                        Subject = mailNotification?.Subject ?? $"Training Review Status - {courseName}",
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
                ValidateContext(context);

                var course = await GetCourseById(courseId);
                if (course == default) throw new EntityNotFoundException(_localizer.GetString("CourseNotFound"));

                var settings = await GetGeneralSettings();
                var mailNotification = await GetMailNotification(MailType.TrainingRejected);
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

                        var emailHtml = mailNotification is null
                            ? html
                            : ReplacePlaceholders(mailNotification.Message, teacher.User.FirstName, course.Name, settings.CompanyName);

                        var model = new EmailRequestDto
                        {
                            To = teacher.User.Email,
                            Subject = mailNotification?.Subject ?? "Training Rejection",
                            Message = emailHtml,
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
                ValidateContext(context);

                var mailNotification = await GetMailNotification(MailType.UserCreate);
                var html = mailNotification is null
                    ? BuildUserCreatedPasswordEmailHtml(firstName, emailAddress, password, companyName, companyNumber)
                    : ReplacePlaceholders(mailNotification.Message, firstName, emailAddress, password, companyName, companyNumber);

                var model = new EmailRequestDto
                {
                    To = emailAddress,
                    Subject = mailNotification?.Subject ?? "Account Created",
                    Message = html
                };
                await _emailService.SendMailWithHtmlBodyAsync(model).ConfigureAwait(true);
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
                ValidateContext(context);

                var mailNotification = await GetMailNotification(MailType.UserCreate);

                foreach (var emailDto in dtos)
                {
                    var html = mailNotification is null
                    ? BuildUserCreatedPasswordEmailHtml(emailDto.FullName, emailDto.Email, emailDto.Password, emailDto.CompanyName, emailDto.CompanyNumber)
                    : ReplacePlaceholders(mailNotification.Message, emailDto.FullName, emailDto.Email, emailDto.Password, emailDto.CompanyName, emailDto.CompanyNumber);

                    var model = new EmailRequestDto
                    {
                        To = emailDto.Email,
                        Subject = mailNotification?.Subject ?? "Account Created",
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
                ValidateContext(context);

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
                    var duration = await _videoService
                        .GetVideoDuration(videoPath)
                        .ConfigureAwait(true);
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
                ValidateContext(context);

                var users = await _unitOfWork
                    .GetRepository<User>()
                    .GetAllAsync(predicate: p => userIds.Contains(p.Id))
                    .ConfigureAwait(false);

                var mailNotification = await GetMailNotification(MailType.NewGroupMember);

                if (users.Count == default) return;

                var settings = await GetGeneralSettings();

                foreach (var user in users)
                {
                    var html = $"Dear {user.FirstName},<br><br>";
                    html +=
                        $"You have been added to the {groupName}. Now you can find the Training Materials which has been created for this {groupName}. <br><br>";
                    html += $"Link to the group :";
                    html +=
                        $"<a href = '{_appUrl}/groups/{groupSlug}' ><u  style='color:blue;'> Click here </u> </a>";
                    html += $"<br>Thank You, <br> {settings.CompanyName}";
                    var emailHtml = mailNotification is null
                    ? html
                    : ReplacePlaceholders(mailNotification.Message, user.FirstName, groupName, groupSlug, settings.CompanyName);

                    var model = new EmailRequestDto
                    {
                        To = user.Email,
                        Subject = mailNotification?.Subject ?? "New group member",
                        Message = emailHtml,
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
                ValidateContext(context);

                var settings = await GetGeneralSettings()
                var group = await _unitOfWork
                    .GetRepository<Group>()
                    .GetFirstOrDefaultAsync(
                        predicate: x => x.Id == groupId,
                        include: source =>
                            source.Include(x => x.GroupMembers).ThenInclude(x => x.User)
                    )
                    .ConfigureAwait(false);

                var mailNotification = await GetMailNotification(MailType.NewCoursePublished);

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

                        var emailHtml = mailNotification is null
                        ? html
                        : ReplacePlaceholders(mailNotification.Message, fullName, group.Name, settings.CompanyName);

                        var model = new EmailRequestDto
                        {
                            To = member.User?.Email,
                            Subject = mailNotification?.Subject ?? "New training published",
                            Message = emailHtml,
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
                ValidateContext(context);

                var course = await GetCourseById(courseId);
                if (course.CourseTeachers == null) throw new ArgumentException(_localizer.GetString("TeacherNotFound"));

                var mailNotification = await GetMailNotification(MailType.TrainingEnrollment);

                var settings = await GetGeneralSettings();

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


                    var emailHtml = mailNotification is null
                    ? html
                    : ReplacePlaceholders(mailNotification.Message, userName, settings.CompanyName);

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
                ValidateContext(context);

                var settings = await GetGeneralSettings();

                var mailNotification = await GetMailNotification(MailType.CertificateIssued);
                foreach (var user in certificateUserIssuedDtos)
                {

                    var fullName = user.UserName;
                    var html = $"Dear {fullName},<br><br>";
                    html +=
                        $"We are happy to inform you that your Certificate of Achievement for {courseName} has been issued and is now available in your profile on the application. "
                        + "Please log in to your account and navigate to your profile to view and download your certificate.<br><br>";
                    html += $"we hope you find the training helpful.<br><br>";
                    html += $"Best regards, <br> {settings.CompanyName}";

                    var emailHtml = mailNotification is null
                    ? html
                    : ReplacePlaceholders(mailNotification.Message, user.UserName, settings.CompanyName);

                    var model = new EmailRequestDto
                    {
                        To = user?.Email,
                        Subject = mailNotification?.Subject ?? "Certificate Issued",
                        Message = emailHtml,
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
                ValidateContext(context);

                var settings = await GetGeneralSettings();
                var course = await _unitOfWork
                    .GetRepository<Course>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Name == courseName,
                        include: source =>
                            source.Include(x => x.CourseEnrollments).ThenInclude(x => x.User)
                    )
                    .ConfigureAwait(false);

                var mailNotification = await GetMailNotification(MailType.LessonAdded);

                if (course.CourseEnrollments == null) throw new ArgumentException("No enrollments");

                foreach (var users in course.CourseEnrollments.AsList())
                {

                    var firstName = users.User.FirstName;
                    var html = $"Dear {firstName},<br><br>";
                    html +=
                    @$"Your enrolled training entitled  <a href='{_appUrl}/trainings/{courseSlug}' ><u  style='color:blue;'>'{courseName}'</u></a>  has been updated with new content. We encourage you to visit the training page and "
                    + $"explore the new materials to enhance your learning experience.<br><br>";

                    html += $"<br><br>Thank You, <br> {settings.CompanyName}";

                    var emailHtml = mailNotification is null
                    ? html
                    : ReplacePlaceholders(mailNotification.Message, firstName, settings.CompanyName);

                    var model = new EmailRequestDto
                    {
                        To = users.User?.Email,
                        Subject = mailNotification?.Subject ?? "Lesson Added",
                        Message = emailHtml,
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
                ValidateContext(context);

                var settings = await GetGeneralSettings();

                var mailNotification = await GetMailNotification(MailType.AccountUpdate);
                var html = $"Dear {fullName}<br><br>";
                html +=
                    @$"A recent change has been made to the email address associated with your account to {newEmail}<br>.Please check your email for the login credentials. If you encounter any difficulties, please contact your administrator immediately.";
                html += $"<br><br>Thank You, <br> {settings.CompanyName}";

                var emailHtml = mailNotification is null
                ? html
                : ReplacePlaceholders(mailNotification.Message, fullName, settings.CompanyName);
                var model = new EmailRequestDto
                {
                    To = oldEmail,
                    Subject = mailNotification?.Subject ?? "Notification: Email Address Change",
                    Message = emailHtml,
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
                ValidateContext(context);

                var settings = await GetGeneralSettings();

                var mailNotification = await GetMailNotification(MailType.EmailChange);

                var html = $"Dear {fullName} <br><br>";
                html +=
                    $"A recent change has been made to the email address associated with your account to {newEmail}.Please check your email to verify the email address.If you did not initiate this change, please contact your administrator immediately to address the issue.";
                html += $"<br><br>Best regards, <br> {settings.CompanyName}";

                var emailHtml = mailNotification is null
                ? html
                : ReplacePlaceholders(mailNotification.Message, fullName, settings.CompanyName);

                var model = new EmailRequestDto
                {
                    To = oldEmail,
                    Subject = mailNotification?.Subject ?? "Notification: Email Address Change",
                    Message = emailHtml,
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
                ValidateContext(context);

                var assessment = await _unitOfWork
                    .GetRepository<Assessment>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id == assessmentId,
                        include: source => source.Include(x => x.User)
                    )
                    .ConfigureAwait(false);
                if (assessment.Id != assessmentId) return;

                var settings = await GetGeneralSettings();

                var mailNotification = await GetMailNotification(MailType.AssessmentAccept);


                var html = $"Dear {assessment.User.FirstName},<br><br>";
                html +=
                    $"Assessment "
                    + @$"<a href = '{_appUrl}/settings/courses'>""{assessment.Title}""</a> published successfully. Thank you.<br><br>";
                html += $"Best regards, <br> {settings.CompanyName}";

                var emailHtml = mailNotification is null
                ? html
                : ReplacePlaceholders(mailNotification.Message, assessment.User.FirstName, settings.CompanyName);

                var model = new EmailRequestDto
                {
                    To = assessment.User.Email,
                    Subject = mailNotification?.Subject ?? $"Assessment Review Status - {assessment.AssessmentStatus}",
                    Message = emailHtml
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
                ValidateContext(context);

                var assessment = await _unitOfWork
                    .GetRepository<Assessment>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id == assessmentId,
                        include: source => source.Include(x => x.User)
                    )
                    .ConfigureAwait(false);
                if (assessment.Id != assessmentId) return;

                var settings = await GetGeneralSettings();

                var mailNotification = await GetMailNotification(MailType.AssessmentReject);

                var html = $"Dear {assessment?.User.FirstName},<br><br>";
                html +=
                    $"We regret to inform you that your Assessment, {assessment.Title} has been rejected for the following reason:<br><br>";
                html += $"{assessment.Message}<br><br>";
                html +=
                    $"However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. Once you have addressed the identified issues, please resubmit the training program for further review.<br><br>";
                html += $"Thank you for your understanding and cooperation.<br><br>";
                html += $"Best regards,<br> {settings.CompanyName}";

                var emailHtml = mailNotification is null
                ? html
                : ReplacePlaceholders(mailNotification.Message, assessment.User.FirstName, settings.CompanyName);
                var model = new EmailRequestDto
                {
                    To = assessment.User.Email,
                    Subject = mailNotification?.Subject ?? $"Assessment Review Status - {assessment.AssessmentStatus}",
                    Message = emailHtml
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
                ValidateContext(context);

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
                if (assessment.Id != assessmentId) return;

                // all admins and super admin
                var users = user.Where(x =>
                    x.Role == UserRole.SuperAdmin || x.Role == UserRole.Admin
                );

                var settings = await GetGeneralSettings();

                var mailNotification = await GetMailNotification(MailType.AssessmentReview);

                foreach (var assessments in users)
                {
                    var firstName = assessments.FirstName;
                    var emails = assessments.Email;

                    var html = $"Dear {firstName},<br><br>";
                    html +=
                        $"Assessment "
                        + @$"<a href = '{_appUrl}/settings/courses'>""{assessment.Title}""</a> is requested for review. Thank you.<br><br>";
                    html += $"Best regards, <br> {settings.CompanyName}";

                    var emailHtml = mailNotification is null
                    ? html
                    : ReplacePlaceholders(mailNotification.Message, firstName, settings.CompanyName);

                    var model = new EmailRequestDto
                    {
                        To = emails,
                        Subject = mailNotification?.Subject ?? $"Request for Assessment Review - {assessment.AssessmentStatus}",
                        Message = emailHtml
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
