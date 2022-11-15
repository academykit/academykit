namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using System;
    using System.Threading.Tasks;

    public interface ICourseService : IGenericService<Course, CourseBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to change course status
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="status">the course status</param>
        /// <param name="currentUserId">the current id</param>
        /// <returns></returns>
        Task ChangeStatusAsync(string identity, CourseStatus status, Guid currentUserId);
        /// <summary>
        /// Course Enrollment
        /// </summary>
        /// <param name="identity"> course id or slug</param>
        /// <param name="userId"> the user id</param>

        Task EnrollmentAsync(string identity, Guid userId);
    }
}