namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;

    public interface IFeedbackService : IGenericService<Feedback, FeedbackBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to get list of student who has submitted feedback
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<IList<FeedbackSubmissionStudentResponseModel>> GetFeedbackSubmittedStudent(
            string lessonIdentity,
            Guid currentUserId
        );

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
        Task FeedbackSubmissionAsync(
            string lessonIdentity,
            IList<FeedbackSubmissionRequestModel> models,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to search feedback
        /// </summary>
        /// <param name="searchCriteria">the instance of <see cref="FeedbackBaseSearchCriteria"/></param>
        /// <returns>the list of <see cref="FeedbackResponseModel"/></returns>
        Task<IList<FeedbackResponseModel>> SearchAsync(FeedbackBaseSearchCriteria searchCriteria);

        /// <summary>
        ///
        /// </summary>
        /// <param name="lessonIdentity"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns> <summary>
        Task<IList<FeedBackChartResponseModel>> GetFeedbackChartData(
            string lessonIdentity,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to get feedback report
        /// </summary>
        /// <param name="lessonIdentity"> the lesson id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of byte </returns>
        Task<byte[]> GetFeedBackReportAsync(string lessonIdentity, Guid currentUserId);

        /// <summary>
        /// reorder feedback questions
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="lessonIdentiy">lesson id or slug</param>
        /// <param name="ids">list of feedback id</param>
        /// <returns>Task completed</returns>
        Task ReorderFeedbackQuestionsAsync(
            Guid currentUserId,
            string lessonIdentiy,
            List<Guid> ids
        );
    }
}
