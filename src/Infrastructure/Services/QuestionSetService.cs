namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class QuestionSetService : BaseGenericService<QuestionSet, BaseSearchCriteria>, IQuestionSetService
    {
        public QuestionSetService(
            IUnitOfWork unitOfWork,
            ILogger<QuestionSetService> logger) : base(unitOfWork, logger)
        {

        }

        /// <summary>
        /// Handle to add question in question set
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="model">the instance of <see cref="QuestionSetAddQuestionRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task AddQuestionsAsync(string identity, QuestionSetAddQuestionRequestModel model, Guid currentUserId)
        {
            try
            {
                var questionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id.ToString() == identity || p.Slug == identity,
                    include: src => src.Include(x => x.Lesson.Course.CourseTeachers)).ConfigureAwait(false);

                if (questionSet == null)
                {
                    _logger.LogWarning("Question set not found with identity: {identity}.", identity);
                    throw new EntityNotFoundException("Question set not found");
                }
                var isCourseTeacher = questionSet.Lesson.Course.CourseTeachers.Any(x => x.UserId == currentUserId);
                if (questionSet.CreatedBy != currentUserId && !isCourseTeacher)
                {
                    _logger.LogWarning("User with userId: {userId} is unauthorized user to add questions in question set with id : {id}", currentUserId, questionSet.Id);
                    throw new EntityNotFoundException("Unauthorized user to add questions in question set");
                }
                var checkQuestionSetSubmission = await _unitOfWork.GetRepository<QuestionSetSubmission>().ExistsAsync(
                    predicate: p => p.QuestionSetId == questionSet.Id).ConfigureAwait(false);

                if (checkQuestionSetSubmission)
                {
                    _logger.LogWarning("Question set with id: {questionSetId} contains question set submission.", questionSet.Id);
                    throw new ArgumentException("Question set contains answer submission. So, not allowed to add question in question set.");
                }
                var existingQuestionSetQuestions = await _unitOfWork.GetRepository<QuestionSetQuestion>().GetAllAsync(
                    predicate: p => p.QuestionSetId == questionSet.Id).ConfigureAwait(false);

                int order = 0;
                if (existingQuestionSetQuestions.Count != default)
                {
                    order = existingQuestionSetQuestions.OrderByDescending(x => x.Order).LastOrDefault().Order;
                }

                var oldQuestionIds = existingQuestionSetQuestions.Select(x => x.Id).ToList();
                var removeQuestionIds = oldQuestionIds.Except(model.QuestionPoolQuestionIds);
                var newQuestionIds = model.QuestionPoolQuestionIds.Except(oldQuestionIds).ToList();
                var currentTimeStamp = DateTime.UtcNow;
                var removeData = existingQuestionSetQuestions.Where(x => removeQuestionIds.Any(y => y == x.Id)).ToList();

                var questionSetQuestions = new List<QuestionSetQuestion>();
                if (removeData.Count != default)
                {
                    _unitOfWork.GetRepository<QuestionSetQuestion>().Delete(removeData);
                }
                if (newQuestionIds.Count != default)
                {
                    foreach (var questionId in newQuestionIds)
                    {
                        questionSetQuestions.Add(new QuestionSetQuestion()
                        {
                            Id = Guid.NewGuid(),
                            QuestionSetId = questionSet.Id,
                            QuestionPoolQuestionId = questionId,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp,
                            Order = ++order
                        });
                    }
                    await _unitOfWork.GetRepository<QuestionSetQuestion>().InsertAsync(questionSetQuestions).ConfigureAwait(false);
                }
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to add questions in question set.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to add questions in question set.");
            }
        }
    }
}
