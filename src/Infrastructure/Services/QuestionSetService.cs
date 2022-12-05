namespace Lingtren.Infrastructure.Services
{
    using System.Linq.Expressions;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;

    public class QuestionSetService : BaseGenericService<QuestionSet, BaseSearchCriteria>, IQuestionSetService
    {
        public QuestionSetService(
            IUnitOfWork unitOfWork,
            ILogger<QuestionSetService> logger) : base(unitOfWork, logger)
        {
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<QuestionSet, object> IncludeNavigationProperties(IQueryable<QuestionSet> query)
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="identity">The id or slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<QuestionSet, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
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

        /// <summary>
        /// Handle to set exam start time
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="currentUserId">the current user id</param>
        public async Task<QuestionSetSubmissionResponseModel> StartExam(string identity, Guid currentUserId)
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                var questionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                    predicate: x => (x.Id.ToString() == identity || x.Slug == identity)).ConfigureAwait(false);
                if (questionSet == null)
                {
                    _logger.LogWarning("Question set not found with identity: {identity} for user with id : {currentUserId}", identity, currentUserId);
                    throw new EntityNotFoundException("Question set not found");
                }

                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                    predicate: p => p.QuestionSetId == questionSet.Id,
                    include: src => src.Include(x => x.Course)).ConfigureAwait(false);

                var isEnrolled = await _unitOfWork.GetRepository<CourseEnrollment>().ExistsAsync(
                    predicate: p => p.CourseId == lesson.CourseId && p.UserId == currentUserId && !p.IsDeleted
                            && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Unenrolled)
                            ).ConfigureAwait(false);
                if (!isEnrolled)
                {
                    _logger.LogWarning("User with id:{currentUserId} has not enrolled in course with id: {courseId} and question set id with id: {questionSetId}."
                                                , currentUserId, lesson.CourseId, questionSet.Id);
                    throw new ForbiddenException("User not enrolled in the course");
                }

                var questionSetSubmissionCount = await _unitOfWork.GetRepository<QuestionSetSubmission>().CountAsync(
                    predicate: p => p.QuestionSetId == questionSet.Id && p.UserId == currentUserId).ConfigureAwait(false);

                if (questionSetSubmissionCount >= questionSet.AllowedRetake)
                {
                    _logger.LogWarning("User with Id {currentUserId} has already taken exam of Question Set with Id {questionSetId}.", currentUserId, questionSet.Id);
                    throw new ForbiddenException("Exam already taken");
                }
                if (questionSet.EndTime.HasValue && questionSet.EndTime != default && questionSet.EndTime < currentTimeStamp)
                {
                    _logger.LogWarning("Question set with id: {questionSetId} has been finished.", questionSet.Id);
                    throw new ForbiddenException("Exam already finished");
                }

                var questionSetQuestions = await _unitOfWork.GetRepository<QuestionSetQuestion>().GetAllAsync(
                    predicate: p => p.QuestionSetId == questionSet.Id,
                    orderBy: o => o.OrderBy(a => a.Order),
                    include: src => src.Include(x => x.QuestionPoolQuestion.Question.QuestionOptions.OrderBy(o => o.Order))).ConfigureAwait(false);

                var questionSetSubmission = new QuestionSetSubmission
                {
                    Id = Guid.NewGuid(),
                    StartTime = currentTimeStamp,
                    QuestionSetId = questionSet.Id,
                    UserId = currentUserId,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                };

                var duration = questionSet.StartTime == default
                                || currentTimeStamp.AddSeconds(questionSet.Duration) < Convert.ToDateTime(questionSet.EndTime) ?
                                questionSet.Duration :
                                Convert.ToInt32((Convert.ToDateTime(questionSet.EndTime) - currentTimeStamp).TotalSeconds);

                var response = new QuestionSetSubmissionResponseModel
                {
                    Id = questionSetSubmission.Id,
                    StartDateTime = Convert.ToDateTime(questionSetSubmission.StartTime),
                    Duration = duration,
                    Name = questionSet.Name,
                    Description = questionSet.Description,
                    Questions = new List<QuestionResponseModel>()
                };
                questionSetQuestions.ForEach(x => response.Questions.Add(new QuestionResponseModel(x.QuestionPoolQuestion.Question, questionSetQuestionId: x.Id, showHints: false)));
                await _unitOfWork.GetRepository<QuestionSetSubmission>().InsertAsync(questionSetSubmission).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return response;
            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();
                _logger.LogError(ex, "An error occurred while attempting to start exam.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to start exam.");
            }
        }

        /// <summary>
        /// Handle to update answer submission
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="questionSetSubmissionId">the question set submission</param>
        /// <param name="answers">the list of <see cref="AnswerSubmissionRequestModel" /></param>
        /// <param name="currentUserId">the current user id</param>
        public async Task AnswerSubmission(string identity, Guid questionSetSubmissionId, IList<AnswerSubmissionRequestModel> answers, Guid currentUserId)
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                bool isSubmissionError = false;

                var questionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                    predicate: x => (x.Id.ToString() == identity || x.Slug == identity)).ConfigureAwait(false);
                if (questionSet == null)
                {
                    _logger.LogWarning("Question set not found with identity: {identity} for user with id : {currentUserId}", identity, currentUserId);
                    throw new EntityNotFoundException("Question set not found");
                }

                var questionSetQuestions = await _unitOfWork.GetRepository<QuestionSetQuestion>().GetAllAsync(
                    predicate: p => p.QuestionSetId == questionSet.Id,
                    include: src => src.Include(x => x.QuestionPoolQuestion.Question.QuestionOptions)).ConfigureAwait(false);

                var questionSetSubmission = await _unitOfWork.GetRepository<QuestionSetSubmission>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == questionSetSubmissionId && p.QuestionSetId == questionSet.Id && p.UserId == currentUserId).ConfigureAwait(false);
                if (questionSetSubmission == null)
                {
                    _logger.LogWarning("Question set submission not found with id: {questionSetSubmissionId} for user with id : {currentUserId}", questionSetSubmissionId, currentUserId);
                    throw new EntityNotFoundException("Question set submission not found");
                }

                var questionSetSubmissionAnswerCount = await _unitOfWork.GetRepository<QuestionSetSubmissionAnswer>().CountAsync(
                    predicate: p => p.QuestionSetSubmissionId == questionSetSubmissionId).ConfigureAwait(false);
                if (questionSetSubmissionAnswerCount > 0)
                {
                    _logger.LogWarning("Question set submission with id: {questionSetSubmissionId} already contains answers for user with id: {currentUserId}", questionSetSubmissionId, currentUserId);
                    throw new ForbiddenException("Exam already submitted.");
                }

                var answerSubmissionCount = await _unitOfWork.GetRepository<QuestionSetSubmission>().CountAsync(
                    predicate: p => p.UserId == currentUserId && p.QuestionSetId == questionSet.Id && p.EndTime != default
                    ).ConfigureAwait(false);
                if (questionSet.AllowedRetake != 0 && answerSubmissionCount >= questionSet.AllowedRetake)
                {
                    _logger.LogWarning("Maximum attempt / submission count reached for user with id : {userId} for question-set with id : {questionSetId}", currentUserId, questionSet.Id);
                    throw new ForbiddenException("Exam already submitted.");
                }

                var questionSetQuestionIds = answers.ToList().ConvertAll(x => x.QuestionSetQuestionId);
                var intersectCount = questionSetQuestions.Select(x => x.Id).IntersectBy(questionSetQuestionIds, y => y).Count();
                if (intersectCount != questionSetQuestions.Count)
                {
                    _logger.LogWarning("User with id: {currentUserId} doesn't submit all question for question set submission with id: {questionSetSubmissionId}", currentUserId, questionSetSubmissionId);
                    isSubmissionError = true;
                    questionSetSubmission.SubmissionErrorMessage += " Exam submission question doesn't match with question set question.";
                }

                if (questionSet.Duration != 0 && currentTimeStamp.AddSeconds(-30) > questionSet.EndTime)
                {
                    _logger.LogWarning("Exam duration expires for question set submission with id: {questionSetSubmissionId} and user with id: {currentUserId}", questionSetSubmissionId, currentUserId);
                    isSubmissionError = true;
                    questionSetSubmission.SubmissionErrorMessage += " Late Submission";
                }

                IList<QuestionSetSubmissionAnswer> answerSubmissionAnswers = new List<QuestionSetSubmissionAnswer>();

                questionSetSubmission.IsSubmissionError = isSubmissionError;
                questionSetSubmission.EndTime = currentTimeStamp;
                questionSetSubmission.UpdatedBy = currentUserId;
                questionSetSubmission.UpdatedOn = currentTimeStamp;
                foreach (var item in answers)
                {
                    var questionSetQuestion = questionSetQuestions.FirstOrDefault(x => x.Id == item.QuestionSetQuestionId);
                    if (questionSetQuestion != null)
                    {
                        var answerIds = questionSetQuestion.QuestionPoolQuestion.Question.QuestionOptions.Where(x => x.IsCorrect).Select(x => x.Id);
                        bool isCorrect = answerIds.OrderBy(x => x).ToList().SequenceEqual(item.Answers.OrderBy(x => x).ToList());
                        answerSubmissionAnswers.Add(new QuestionSetSubmissionAnswer
                        {
                            Id = Guid.NewGuid(),
                            QuestionSetSubmissionId = questionSetSubmissionId,
                            QuestionSetQuestionId = item.QuestionSetQuestionId,
                            SelectedAnswers = string.Join(",", item.Answers),
                            IsCorrect = isCorrect,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp
                        });
                    }
                }
                var questionSetResult = new QuestionSetResult();
                if (!isSubmissionError)
                {
                    questionSetResult = new QuestionSetResult()
                    {
                        Id = Guid.NewGuid(),
                        UserId = currentUserId,
                        QuestionSetSubmissionId = questionSetSubmissionId,
                        QuestionSetId = questionSet.Id,
                        TotalMark = 0,
                        NegativeMark = 0,
                        CreatedBy = currentUserId,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = currentUserId,
                        UpdatedOn = currentTimeStamp,
                    };
                    var correctAnswersCount = answerSubmissionAnswers.Count(x => x.IsCorrect);
                    questionSetResult.TotalMark = questionSet.QuestionMarking * correctAnswersCount;
                    if (questionSet.NegativeMarking > 0)
                    {
                        var incorrectAnswersCount = answerSubmissionAnswers.Count(x => !x.IsCorrect && !string.IsNullOrEmpty(x.SelectedAnswers));
                        questionSetResult.NegativeMark = incorrectAnswersCount * questionSet.NegativeMarking;
                    }
                    await _unitOfWork.GetRepository<QuestionSetResult>().InsertAsync(questionSetResult).ConfigureAwait(false);
                }

                _unitOfWork.GetRepository<QuestionSetSubmission>().Update(questionSetSubmission);
                await _unitOfWork.GetRepository<QuestionSetSubmissionAnswer>().InsertAsync(answerSubmissionAnswers).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();
                _logger.LogError(ex, "An error occurred while attempting to submit the exam.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to submit the exam.");
            }
        }
    }
}
