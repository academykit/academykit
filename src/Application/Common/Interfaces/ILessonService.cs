namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using System.Threading.Tasks;

    public interface ILessonService : IGenericService<Lesson, LessonBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to create lesson
        /// </summary>
        /// <param name="courseIdentity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></see></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<Lesson> AddAsync(string courseIdentity, LessonRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to get lesson detail
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns>the instance of <see cref="Lesson"/></returns>
        Task<Lesson> GetLessonAsync(string identity, string lessonIdentity, Guid currentUserId);

        /// <summary>
        /// Handle to delete lesson
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task DeleteLessonAsync(string identity, string lessonIdentity, Guid currentUserId);

        /// <summary>
        /// Handle to reorder lesson
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="LessonReorderRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task ReorderAsync(string identity, LessonReorderRequestModel model, Guid currentUserId);
    }
}