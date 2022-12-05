namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;

    public interface IQuestionPoolService : IGenericService<QuestionPool, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to question pool question
        /// </summary>
        /// <param name="poolIdentity">the question pool id or slug</param>
        /// <param name="questionId">the question id</param>
        /// <returns>the instance of <see cref="QuestionPoolQuestion"/></returns>
        Task<QuestionPoolQuestion> GetQuestionPoolQuestion(string poolIdentity, Guid questionId);
    }
}
