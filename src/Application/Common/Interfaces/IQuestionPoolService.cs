namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;
    using Org.BouncyCastle.Crypto;

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
        /// reorder questions in questionpool
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="identity">id or slug of questionpool</param>
        /// <param name="ids">list of question id in questionpool</param>
        /// <returns>task completed</returns>
        Task Reorder(Guid currentUserId,string identity,IList<Guid> ids);
    }
}
