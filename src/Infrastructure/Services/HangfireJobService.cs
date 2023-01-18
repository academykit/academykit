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
                if(users.Count == default)
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
                        Subject ="",
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
                        Subject ="New group member"
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
        /// <param name="course"> the instance of <see cref="Course"/></param>
        /// <param name="context"> the instance of <see cref="PerformContext"/></param>
        /// <returns> the task complete </returns>
        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task GroupCoursePublishedMail(Course course, PerformContext context = null)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentException("Context not found.");
                }

                var settings = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync();
                var members = await _unitOfWork.GetRepository<GroupMember>().GetAllAsync(predicate:p => p.GroupId == course.GroupId, include: s => s.Include(x =>x.User).Include(x => x.Group)).ConfigureAwait(false);
                if(members.Count != default)
                {
                    foreach(var member in members)
                    {
                        var fullName = string.IsNullOrEmpty(member.User?.MiddleName) ? $"{member.User?.FirstName} {member.User?.LastName}" : $"{member.User?.FirstName} {member.User?.MiddleName} {member.User?.LastName}";
                        var html = $"Dear {fullName},<br><br>";
                        html += $"You have new {course.Name} training available for the {member.Group.Name}. Please, go to {member.Group.Name} to find the training there. <br><br>";
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
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }
    }
}