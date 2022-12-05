namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
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

        /// <summary>
        /// Handle to delete course
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the task complete</returns>
        Task DeleteCourseAsync(string identity, Guid currentUserId);

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CourseRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task<Course> UpdateAsync(string identity, CourseRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to get user enrollment status
        /// </summary>
        /// <param name="course">the course id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="fetchMembers">the bool value for fetch members</param>
        /// <returns></returns>
        Task<CourseEnrollmentStatus> GetUserCourseEnrollmentStatus(Course course, Guid currentUserId, bool fetchMembers = false);

        /// <summary>
        /// Handle to get course detail
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="CourseResponseModel"/></returns>
        Task<CourseResponseModel> GetDetailAsync(string identity, Guid currentUserId);

        /// <summary>
        /// Handle to search group courses
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated search result</returns>
        Task<SearchResult<Course>> GroupCourseSearchAsync(string identity, BaseSearchCriteria criteria);
    }
}