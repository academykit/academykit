namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
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
    }
}
