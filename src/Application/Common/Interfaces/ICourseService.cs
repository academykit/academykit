namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Domain.Entities;
    using Lingtren.Application.Common.Dtos;
    using System.Threading.Tasks;
    using System;
    using Lingtren.Domain.Enums;

    public interface ICourseService : IGenericService<Course, CourseBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to change course status
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="status">the course status</param>
        /// <param name="currentUserId">the current id</param>
        /// <returns></returns>
        Task ChangeStatusAsync(string identity, Status status, Guid currentUserId);
    }
}