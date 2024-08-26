namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Domain.Entities;

    public interface IQuestionPoolService : IGenericService<QuestionPool, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to question pool question
        /// </summary>
        /// <param name="poolIdentity">the question pool id or slug</param>
        /// <param name="questionId">the question id</param>
        /// <returns>the instance of <see cref="QuestionPoolQuestion"/></returns>
        Task<QuestionPoolQuestion> GetQuestionPoolQuestion(string poolIdentity, Guid questionId);

        /// <summary>
        /// reorder questions in question pool
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="identity">id or slug of question pool</param>
        /// <param name="ids">list of question id in question pool</param>
        /// <returns>task completed</returns>
        Task QuestionPoolQuestionReorderAsync(Guid currentUserId, string identity, IList<Guid> ids);
    }
}
