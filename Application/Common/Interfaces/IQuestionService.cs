﻿namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Domain.Entities;

    public interface IQuestionService : IGenericService<Question, QuestionBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to add question
        /// </summary>
        /// <param name="identity"> the question pool id or slug </param>
        /// <param name="question"> the instance of <see cref="QuestionRequestModel"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task<Question> AddAsync(string identity, QuestionRequestModel question, Guid currentUserId);

        /// <summary>
        /// Handle to update question
        /// </summary>
        /// <param name="poolIdentity">the question pool id or slug</param>
        /// <param name="questionId">the question id </param>
        /// <param name="question">the instance of <see cref="QuestionRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<Question> UpdateAsync(
            string poolIdentity,
            Guid questionId,
            QuestionRequestModel question,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to delete question
        /// </summary>
        /// <param name="poolIdentity">the question pool id or slug</param>
        /// <param name="questionId">the question id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task DeleteQuestionAsync(string poolIdentity, Guid questionId, Guid currentUserId);

        /// <summary>
        /// Handle to add question
        /// /// </summary>
        /// <param name="identity"> the question pool id or slug </param>
        /// <param name="question"> the instance of <see cref="QuestionRequestModel"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task<QuestionSetAddQuestionRequestModel> BulkAddQuestions(
            List<QuestionRequestModel> questions,
            Guid currentUserId,
            string identity
        );
    }
}
