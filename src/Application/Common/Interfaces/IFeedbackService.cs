namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;

    public interface IFeedbackService : IGenericService<Feedback, FeedbackBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to get list of student who has submitted feedback
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<IList<FeedbackSubmissionStudentResponseModel>> GetFeedbackSubmittedStudent(string lessonIdentity, Guid currentUserId);

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the Feedback id or slug</param>
        /// <param name="model">the instance of <see cref="FeedbackRequestModel"/> </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<Feedback> UpdateAsync(string identity, FeedbackRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to submit Feedbacks by the user
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="models">the list of <see cref="FeedbackSubmissionRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        Task FeedbackSubmissionAsync(string lessonIdentity, IList<FeedbackSubmissionRequestModel> models, Guid currentUserId);

        /// <summary>
        /// Handle to search feedback
        /// </summary>
        /// <param name="searchCriteria">the instance of <see cref="FeedbackBaseSearchCriteria"/></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ForbiddenException"></exception>

        Task<IList<FeedbackResponseModel>> SearchAsync(FeedbackBaseSearchCriteria searchCriteria);
    }
}
