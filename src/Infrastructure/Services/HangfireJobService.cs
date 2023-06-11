namespace Lingtren.Infrastructure.Services
{
    using Amazon.S3.Model;
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
    using Org.BouncyCastle.Crypto.Parameters;
    using System;
    using System.Diagnostics.Metrics;
    using System.Text;
    using static Dapper.SqlMapper;
    using static System.Net.WebRequestMethods;

    public class HangfireJobService : BaseService, IHangfireJobService
    {
        private readonly string _appUrl;
        private readonly IEmailService _emailService;
        public HangfireJobService(IConfiguration configuration, IUnitOfWork unitOfWork, ILogger<HangfireJobService> logger,
        IEmailService emailService,IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger,localizer)
        {
            _emailService = emailService;
            _appUrl = configuration.GetSection("AppUrls:App").Value;
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

                var users = await _unitOfWork.GetRepository<User>().GetAllAsync(predicate: p => p.Role == UserRole.Admin || p.Role == UserRole.SuperAdmin).ConfigureAwait(false);
                if (users.Count == default)
                {
                    return;
                }
                var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();
                foreach (var user in users)
                {
                    var fullName = string.IsNullOrEmpty(user.MiddleName) ? $"{user.FirstName} {user.LastName}" : $"{user.FirstName} {user.MiddleName} {user.LastName}";
                    var html = $"Dear {fullName},<br><br>";
                    html += $"You have new {courseName} training available for the review process <br>" +
                            @$"<a href = '{this._appUrl}/settings/courses'> <u  style='color:blue;'> Click Here </u></a> to redirect to the course<br><br>";
                    html += $"Thank You, <br> {settings.CompanyName}";
                    var model = new EmailRequestDto
                    {
                        To = user.Email,
                        Subject = "Review Courses",
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
        public async Task CourseRejectedMailAsync(Guid courseId,string message, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException(_localizer.GetString("ContextNotFound"));
                }

                var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(predicate: p => p.Id == courseId,
                include: source => source.Include(x => x.CourseTeachers).ThenInclude(x => x.User)).ConfigureAwait(false);

                if (course == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("CourseNotFound"));
                }

                 var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();
                foreach (var teacher in course.CourseTeachers)
                {
                    if (!string.IsNullOrEmpty(teacher.User?.Email))
                    {
                        var html = $"Dear {teacher?.User.FullName},<br><br>";
                        html += $"Your {course.Name} course has been rejected. <br>{message}</b><br><br> ";
                        html += $"Thank You,<br> {settings.CompanyName}";
                        
                        var model = new EmailRequestDto
                        {
                            To = teacher.User.Email,
                            Subject = "Course Rejected",
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
        public async Task SendUserCreatedPasswordEmail(string emailAddress, string firstName, string password, string companyName, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException("Context not found.");
                }

                var html = $"Dear {firstName},<br><br>";
                html += $"Your account has been created in Vurilo Team. <br> Your Login Password is <b><u>{password}</u></b><br><br>";
                html += $"Thank You,<br> {companyName}";
                html += @$"<a href = '{this._appUrl}' > <u  style='color:blue;'> Click Here </u></a>";

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
                _logger.LogError(ex, "An error occurred while attempting to send change email address mail.");
            }
        }

        /// <summary>
        /// Handle to send email to imported user async
        /// </summary>
        /// <param name="dtos"> the list of <see cref="UserEmailDto" /> .</param>
        /// <returns> the task complete </returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SendEmailImportedUserAsync(IList<UserEmailDto> dtos, PerformContext context = null)
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
                    html += $"Your account has been created in {emailDto.CompanyName}." +
                        @$"<a href ='{this._appUrl}' ><u  style='color:blue;'> Click Here </u></a> to go to application" +
                        $"<br> Your Login Password is <b><u>{emailDto.Password}</u></b><br><br>";
                    html += $"<br><br>Thank You,<br> {emailDto.CompanyName}";
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
        /// Handle to send mail to new group member
        /// </summary>
        /// <param name="gropName"> the group name </param>
        /// <param name="groupSlug"> the group slug </param>
        /// <param name="userIds"> the list of <see cref="Guid" /> .</param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
        /// <returns> the task complete </returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SendMailNewGroupMember(string gropName,string groupSlug ,IList<Guid> userIds, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException(_localizer.GetString("ContextNotFound"));
                }

                var users = await _unitOfWork.GetRepository<User>().GetAllAsync(
                    predicate: p => userIds.Contains(p.Id)).ConfigureAwait(false);

                if (users.Count == default)
                {
                    return;
                }
                var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();

                foreach (var user in users)
                {
                   
                    var fullName = string.IsNullOrEmpty(user.MiddleName) ? $"{user.FirstName} {user.LastName}" : $"{user.FirstName} {user.MiddleName} {user.LastName}";
                    var html = $"Dear {fullName},<br><br>";
                    html += $"You have been added to the {gropName}. Now you can find the Training Materials which has been created for this {gropName}. <br><br>";
                    html += $"Link to the group :";
                    html += $"<a href = '{this._appUrl}/groups/{groupSlug}' ><u  style='color:blue;'> Click here </u> </a>";
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
        public async Task GroupCoursePublishedMailAsync(Guid groupId, string courseName,string courseSlug ,PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException(_localizer.GetString("ContextNotFound"));
                }

                var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(predicate: x => x.Id == groupId,
                include: source => source.Include(x => x.GroupMembers).ThenInclude(x => x.User)).ConfigureAwait(false);
                if (group.GroupMembers.Count != default)
                {
                    foreach (var member in group.GroupMembers)
                    {
                       
                        
                        var fullName = string.IsNullOrEmpty(member.User?.MiddleName) ? $"{member.User?.FirstName} {member.User?.LastName}" : $"{member.User?.FirstName} {member.User?.MiddleName} {member.User?.LastName}";
                        var html = $"Dear {fullName},<br><br>";
                        html += $"You have new {courseName} training available for the {group.Name} group. Please, go to {group.Name} group or " +
                                @$"<a href ='{this._appUrl}/trainings/{courseSlug}'><u  style='color:blue;'>Click Here </u></a> to find the training there. <br>";
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
        public async Task SendCourseEnrollmentMailAsync(Guid courseId, string courseName, PerformContext context = null)
        {
            try
            {
                if(context ==null)
                {
                    throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
                }

                var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(predicate: p => p.Id == courseId,
                include: source => source.Include(x => x.CourseTeachers).ThenInclude(x => x.User)).ConfigureAwait(false);
                var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();

                if (course.CourseTeachers ==null)
                {
                    throw new ArgumentException(_localizer.GetString("TeacherNotFound"));
                }
              
                 foreach (var teacher in course.CourseTeachers)
                {
                        var fullName = string.IsNullOrEmpty(teacher.User?.MiddleName) ? $"{teacher.User?.FirstName} {teacher.User?.LastName}" : $"{teacher.User?.FirstName} {teacher.User?.MiddleName} {teacher.User?.LastName}";
                        var html = $"Dear {fullName},<br><br>";
                        html += $"Your lecture video named '{course.Name}' have been enrolled " +
                                @$"<a href ={this._appUrl}><u  style='color:blue;'>Click Here </u></a>to add more courses ";
                        html += $"<br><br>Thank You, <br> {settings.CompanyName}";

                        var model = new EmailRequestDto
                        {
                            To = teacher.User?.Email,
                            Subject = "Course Enrolled",
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
        public async Task SendCertificateIssueMailAsync(IList<CertificateUserIssuedDto> certificateUserIssuedDtos, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
                }
                var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();

                foreach (var user in certificateUserIssuedDtos)
                {
                    var fullName = user.UserName;
                    var html = $"Dear {fullName},<br><br>";
                    html += $"Your certificate of couse named '{user.CourseName}' have been issued.";
                    html += $"<br><br>Thank You, <br> {settings.CompanyName}";

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
        public async Task SendLessonAddedMailAsync(string courseName,string courseSlug, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentNullException(_localizer.GetString("ContextNotFound"));
                }

                var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();
                var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(predicate: p => p.Name == courseName,
                include: source => source.Include(x => x.CourseEnrollments).ThenInclude(x => x.User)).ConfigureAwait(false);
                if (course.CourseEnrollments == null)
                {
                    throw new ArgumentException("No enrollments");
                }
                foreach(var users in course.CourseEnrollments.AsList()) 
                {
                    
                    var fullName = users.User.FullName;
                    var html = $"Dear {fullName},<br><br>";
                    html += $"Your enrolled course titled '{courseName}' have uploaded new content<br><br>" +
                        @$"<a href='{this._appUrl}/trainings/{courseSlug}' ><u  style='color:blue;'>Click Here </u></a> to watch the course";
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
    }
}