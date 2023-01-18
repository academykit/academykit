namespace Lingtren.Application.Common.Interfaces
{
    using Hangfire;
    using Hangfire.Server;
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
        /// <param name="course"> the instance of <see cref="Course"/></param>
        /// <param name="context"> the instance of <see cref="PerformContext"/></param>
        /// <returns> the task complete </returns>
        Task GroupCoursePublishedMail(Course course, PerformContext context = null);

        /// <summary>
        /// Handle to send course review mail
        /// </summary>
        /// <param name="courseName"> the course name </param>
        /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
        /// <returns> the task complete </returns>
        Task SendCourseReviewMailAsync(string courseName, PerformContext context = null);
    }
}