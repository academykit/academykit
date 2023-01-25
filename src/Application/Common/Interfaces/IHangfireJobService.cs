namespace Lingtren.Application.Common.Interfaces
{
    using Hangfire;
    using Hangfire.Server;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;

    public interface IHangfireJobService
    {
        /// <summary>
        /// Handle to send mail to new group member
        /// </summary>
        /// <param name="gropName"> the group name </param>
        /// <param name="userIds"> the list of <see cref="Guid" /> .</param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
        /// <returns> the task complete </returns>
        Task SendMailNewGroupMember(string gropName, IList<Guid> userIds, PerformContext context = null);

        /// <summary>
        /// Handle to send group course published mail
        /// </summary>
        /// <param name="groupId"> the group id</param>
        /// <param name="courseName"> the course name </param>
        /// <param name="context"> the instance of <see cref="PerformContext"/></param>
        /// <returns> the task complete </returns>
        Task GroupCoursePublishedMailAsync(Guid groupId, string courseName, PerformContext context = null);

        /// <summary>
        /// Handle to send course rejected mail
        /// </summary>
        /// <param name="courseId"> the course id </param>
        /// <param name="message"> the message </param>
        /// <param name="context"> the instance of <see cref="PerformContext" /> .</param>
        /// <returns> the task complete </returns>
        Task CourseRejectedMailAsync(Guid courseId, string message, PerformContext context = null);

        /// <summary>
        /// Handle to send course review mail
        /// </summary>
        /// <param name="courseName"> the course name </param>
        /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
        /// <returns> the task complete </returns>
        Task SendCourseReviewMailAsync(string courseName, PerformContext context = null);

        /// <summary>
        /// Handle to send email to imported user async
        /// </summary>
        /// <param name="dtos"> the list of <see cref="UserEmailDto" /> .</param>
        /// <returns> the task complete </returns>
        Task SendEmailImportedUserAsync(IList<UserEmailDto> dtos, PerformContext context = null);
    }
}