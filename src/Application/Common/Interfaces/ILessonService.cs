namespace Lingtren.Application.Common.Interfaces
{
    using System.Threading.Tasks;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;

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
        /// <returns>the instance of <see cref="LessonResponseModel"/></returns>
        Task<LessonResponseModel> GetLessonAsync(string identity, string lessonIdentity, Guid currentUserId);

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

        /// <summary>
        /// Handle to join meeting
        /// </summary>
        /// <param name="identity">the course identity</param>
        /// <param name="lessonIdentity">the lesson identity</param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        Task<MeetingJoinResponseModel> GetJoinMeetingAsync(string identity, string lessonIdentity, Guid currentUserId);

        /// <summary>
        /// Handle to get meeting report 
        /// </summary>
        /// <param name="identity"> the lesson identity </param>
        /// <param name="userId"> the user id </param>
        /// <param name="lessonIdentity">the lesson identity</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="MeetingReportResponseModel" /> .</returns>
        Task<IList<MeetingReportResponseModel>> GetMeetingReportAsync(string identity, string lessonIdentity, string userId, Guid currentUserId);

        /// <summary>
        /// Handle to update lesson
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<Lesson> UpdateAsync(string identity, string lessonIdentity, LessonRequestModel model, Guid currentUserId);
    }
}
