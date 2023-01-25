namespace Lingtren.Infrastructure.Services
{
    using Hangfire;
    using Hangfire.Server;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;

    public class HangfireJobService : BaseService, IHangfireJobService
    {
        private readonly IEmailService _emailService;
        public HangfireJobService(IUnitOfWork unitOfWork, ILogger<HangfireJobService> logger,
        IEmailService emailService) : base(unitOfWork, logger)
        {
            _emailService = emailService;
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
                    throw new ArgumentException("Context not found.");
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
                    html += $"You have new {courseName} training available for the review process. <br><br>";
                    html += $"Thank You, <br> {settings.CompanyName}";
                    var model = new EmailRequestDto
                    {
                        To = user.Email,
                        Subject = "",
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
                    throw new ArgumentException("Context not found.");
                }

                var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(predicate: p => p.Id == courseId,
                include: source => source.Include(x => x.CourseTeachers).ThenInclude(x => x.User)).ConfigureAwait(false);

                if (course == default)
                {
                    throw new EntityNotFoundException("Course not found");
                }

                 var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();
                foreach (var teacher in course.CourseTeachers)
                {
                    if (!string.IsNullOrEmpty(teacher.User?.Email))
                    {
                        var html = $"Dear {teacher?.User.FullName},<br><br>";
                        html += $"Your {course.Name} course has been rejected. <br>{message}</b><br><br>";
                        html += $"Thank You,<br> {settings.CompanyName}";
                        var model = new EmailRequestDto
                        {
                            To = teacher.User.Email,
                            Message = html,
                            Subject = "Account Created"
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
                    throw new ArgumentException("Context not found.");
                }

                foreach (var emailDto in dtos)
                {
                    var html = $"Dear {emailDto.FullName},<br><br>";
                    html += $"Your account has been created in {emailDto.CompanyName}. <br> Your Login Password is <b><u>{emailDto.Password}</u></b><br><br>";
                    html += $"Thank You,<br> {emailDto.CompanyName}";
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
        /// <param name="userIds"> the list of <see cref="Guid" /> .</param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
        /// <returns> the task complete </returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SendMailNewGroupMember(string gropName, IList<Guid> userIds, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException("Context not found.");
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
                    html += $"Thank You, <br> {settings.CompanyName}";
                    var model = new EmailRequestDto
                    {
                        To = user.Email,
                        Message = html,
                        Subject = "New group member"
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
        /// <param name="context"> the instance of <see cref="PerformContext"/></param>
        /// <returns> the task complete </returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task GroupCoursePublishedMailAsync(Guid groupId, string courseName, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException("Context not found.");
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
                        html += $"You have new {courseName} training available for the {group.Name} group. Please, go to {group.Name} group to find the training there. <br><br>";
                        html += $"Thank You, <br> {settings.CompanyName}";

                        var model = new EmailRequestDto
                        {
                            Subject = "New training published",
                            To = member.User?.Email,
                            Message = html
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
    }
}