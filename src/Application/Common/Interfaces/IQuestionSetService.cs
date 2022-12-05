namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;

    public interface IQuestionSetService : IGenericService<QuestionSet, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to add question in question set
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="model">the instance of <see cref="QuestionSetAddQuestionRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task AddQuestionsAsync(string identity, QuestionSetAddQuestionRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to set exam start time
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="currentUserId">the current user id</param>
        Task<QuestionSetSubmissionResponseModel> StartExam(string identity, Guid currentUserId);

        /// <summary>
        /// Handle to update answer submission
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="questionSetSubmissionId">the question set submission</param>
        /// <param name="answers">the list of <see cref="AnswerSubmissionRequestModel" /></param>
        /// <param name="currentUserId">the current user id</param>
        Task AnswerSubmission(string identity, Guid questionSetSubmissionId, IList<AnswerSubmissionRequestModel> answers, Guid currentUserId);
    }
}
