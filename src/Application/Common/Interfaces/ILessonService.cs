namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using System.Threading.Tasks;

    public interface ILessonService : IGenericService<Lesson, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to create lesson
        /// </summary>
        /// <param name="courseIdentity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></see></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<Lesson> AddAsync(string courseIdentity, LessonRequestModel model, Guid currentUserId);
    }
}