namespace Lingtren.Infrastructure.Services
{
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
    using System.Linq.Expressions;

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
                    throw new ForbiddenException("Question set contains answer submission. So, not allowed to add question in question set.");
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

        #region Start Exam and Submission

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
                    predicate: x => x.Id.ToString() == identity || x.Slug == identity,
                    include: src => src.Include(x => x.Lesson)).ConfigureAwait(false);
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

                await InsertWatchHistory(currentUserId, currentTimeStamp, questionSet).ConfigureAwait(false);
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

        #endregion Start Exam and Submission

        #region Exam Result Reports

        /// <summary>
        /// Handles to fetch result of a particular question set
        /// </summary>
        /// <param name="identity">the question set id or slug </param>
        /// <param name="currentUserId"></param>
        /// <returns>the instance of <see cref="QuestionSetResultResponseModel"</returns>
        public async Task<SearchResult<QuestionSetResultResponseModel>> GetResults(BaseSearchCriteria searchCriteria, string identity, Guid currentUserId)
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                var questionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id.ToString() == identity || p.Slug == identity,
                    include: src => src.Include(x => x.Lesson)).ConfigureAwait(false);

                if (questionSet == null)
                {
                    _logger.LogWarning("Question set not found with identity: {identity}", identity);
                    throw new EntityNotFoundException("Question set not found");
                }

                var course = await ValidateAndGetCourse(currentUserId, questionSet.Lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);

                var isTeacher = course.CourseTeachers.Any(x => x.UserId == currentUserId);

                var predicate = PredicateBuilder.New<QuestionSetResult>(true);
                predicate = predicate.And(p => p.QuestionSetId == questionSet.Id);

                if (questionSet.CreatedBy != currentUserId && !isSuperAdminOrAdmin && !isTeacher)
                {
                    predicate = predicate.And(p => p.UserId == currentUserId);
                }
                if (!string.IsNullOrWhiteSpace(searchCriteria.Search))
                {
                    var search = searchCriteria.Search.ToLower().Trim();
                    predicate = predicate.And(x => x.User.LastName.ToLower().Trim().Contains(search)
                     || x.User.FirstName.ToLower().Trim().Contains(search));
                }

                var query = await _unitOfWork.GetRepository<QuestionSetResult>().GetAllAsync(predicate: predicate, include: src => src.Include(x => x.User)).ConfigureAwait(false);

                var result = query.GroupBy(x => x.UserId).Select(x => new QuestionSetResult
                {
                    Id = x.FirstOrDefault(a => a.CreatedOn == x.Max(b => b.CreatedOn)).Id,
                    QuestionSetId = questionSet.Id,
                    UserId = x.FirstOrDefault(a => a.CreatedOn == x.Max(b => b.CreatedOn)).UserId,
                    TotalMark = x.FirstOrDefault(a => a.CreatedOn == x.Max(b => b.CreatedOn)).TotalMark,
                    NegativeMark = x.FirstOrDefault(a => a.CreatedOn == x.Max(b => b.CreatedOn)).NegativeMark,
                    User = x.FirstOrDefault(a => a.CreatedOn == x.Max(b => b.CreatedOn)).User,
                    CreatedOn = x.Max(a => a.CreatedOn),
                }).ToList();

                var paginatedResult = result.ToIPagedList(searchCriteria.Page, searchCriteria.Size);
                var response = new SearchResult<QuestionSetResultResponseModel>
                {
                    Items = new List<QuestionSetResultResponseModel>(),
                    CurrentPage = paginatedResult.CurrentPage,
                    PageSize = paginatedResult.PageSize,
                    TotalCount = paginatedResult.TotalCount,
                    TotalPage = paginatedResult.TotalPage,
                };
                paginatedResult.Items.ForEach(res => response.Items.Add(new QuestionSetResultResponseModel
                {
                    Id = res.Id,
                    QuestionSetId = res.QuestionSetId,
                    ObtainedMarks = (res.TotalMark - res.NegativeMark) > 0 ? (res.TotalMark - res.NegativeMark) : 0,
                    User = new UserModel
                    {
                        Id = (Guid)res.User?.Id,
                        FullName = res.User?.FullName,
                        Email = res.User?.Email,
                        ImageUrl = res.User.ImageUrl,
                    }
                }));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to retrieving the results.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to retrieving the results.");
            }
        }

        /// <summary>
        /// Handles to fetch result of a particular student result
        /// </summary>
        /// <param name="identity">the question set id or slug </param>
        /// <param name="userId">the student user id </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the instance of <see cref="StudentResultResponseModel"</returns>
        public async Task<StudentResultResponseModel> GetStudentResult(string identity, Guid userId, Guid currentUserId)
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                var questionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                   predicate: p => p.Id.ToString() == identity || p.Slug == identity,
                   include: src => src.Include(x => x.Lesson)).ConfigureAwait(false);

                if (questionSet == null)
                {
                    _logger.LogWarning("Question set not found with identity: {identity}", identity);
                    throw new EntityNotFoundException("Question set not found");
                }

                var course = await ValidateAndGetCourse(currentUserId, questionSet.Lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);

                var isTeacher = course.CourseTeachers.Any(x => x.UserId == currentUserId);

                var predicate = PredicateBuilder.New<QuestionSetSubmission>(true);
                predicate = predicate.And(p => p.QuestionSetId == questionSet.Id);
                predicate = predicate.And(p => p.UserId == userId);
                predicate = predicate.And(p => p.EndTime != default);
                predicate = predicate.And(p => p.QuestionSetSubmissionAnswers.Any(x => x.QuestionSetSubmissionId == p.Id));
                predicate = predicate.And(p => p.QuestionSetResults.Any(x => x.QuestionSetSubmissionId == p.Id));

                var questionSetSubmissions = await _unitOfWork.GetRepository<QuestionSetSubmission>().GetAllAsync(
                    predicate: predicate,
                    include: src => src.Include(x => x.User).Include(x => x.QuestionSetResults)).ConfigureAwait(false);

                if (questionSetSubmissions.Count == 0)
                {
                    return new StudentResultResponseModel();
                }
                var user = questionSetSubmissions.FirstOrDefault()?.User;
                var response = new StudentResultResponseModel
                {
                    AttemptCount = questionSetSubmissions.Count,
                    User = new UserModel
                    {
                        Id = (Guid)user?.Id,
                        FullName = user?.FullName,
                        Email = user?.Email,
                        ImageUrl = user?.ImageUrl,
                    },
                    QuestionSetSubmissions = new List<QuestionSetResultDetailModel>()
                };

                questionSetSubmissions.ForEach(res => response.QuestionSetSubmissions.Add(new QuestionSetResultDetailModel
                {
                    QuestionSetSubmissionId = res.Id,
                    SubmissionDate = res.EndTime != default ? res.EndTime : res.StartTime != default ? res.StartTime : res.CreatedOn,
                    TotalMarks = res.QuestionSetResults.Count > 0 ? res.QuestionSetResults.FirstOrDefault().TotalMark.ToString() : "",
                    NegativeMarks = res.QuestionSetResults.Count > 0 ? res.QuestionSetResults.FirstOrDefault().NegativeMark.ToString() : "",
                    ObtainedMarks = res.QuestionSetResults.Count > 0 ? ((res.QuestionSetResults.FirstOrDefault().TotalMark - res.QuestionSetResults.FirstOrDefault().NegativeMark)
                       > 0 ? (res.QuestionSetResults.FirstOrDefault().TotalMark - res.QuestionSetResults.FirstOrDefault().NegativeMark) : 0).ToString() : "",
                    Duration = questionSet.Duration != 0 ? TimeSpan.FromSeconds(questionSet.Duration).ToString(@"hh\:mm\:ss") : string.Empty,
                    CompleteDuration = questionSet.Duration == 0 || res.EndTime == default ? string.Empty :
                                               TimeSpan.FromSeconds((Convert.ToDateTime(res.StartTime)
                                                                   - Convert.ToDateTime(res.EndTime)).TotalSeconds).ToString(@"hh\:mm\:ss"),
                }));
                response.QuestionSetSubmissions = response.QuestionSetSubmissions.OrderByDescending(x => x.SubmissionDate).ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to retrieving the student result.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to retrieving the student result.");
            }
        }

        /// <summary>
        /// Get result detail of question set submission
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="questionSetSubmissionId">the question set submission id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the instance of <see cref="QuestionSetUserResultResponseModel"</returns>
        public async Task<QuestionSetUserResultResponseModel> GetResultDetail(string identity, Guid questionSetSubmissionId, Guid currentUserId)
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                var questionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                   predicate: p => p.Id.ToString() == identity || p.Slug == identity,
                   include: src => src.Include(x => x.Lesson)).ConfigureAwait(false);

                if (questionSet == null)
                {
                    _logger.LogWarning("Question set not found with identity: {identity}", identity);
                    throw new EntityNotFoundException("Question set not found");
                }

                var course = await ValidateAndGetCourse(currentUserId, questionSet.Lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);

                var isTeacher = course.CourseTeachers.Any(x => x.UserId == currentUserId);

                var predicate = PredicateBuilder.New<QuestionSetResult>(true);
                predicate = predicate.And(p => p.QuestionSetId == questionSet.Id && p.QuestionSetSubmissionId == questionSetSubmissionId);
                var questionSetResult = await _unitOfWork.GetRepository<QuestionSetResult>().GetFirstOrDefaultAsync(predicate: predicate,
                                                                            include: src => src.Include(x => x.User)).ConfigureAwait(false);
                if (questionSetResult == null)
                {
                    _logger.LogWarning("Question set result not found for user with id: {currentUserId} and question-set-id: {questionSetId}", currentUserId, questionSet.Id);
                    throw new EntityNotFoundException("Exam result not found");
                }
                var questionSetSubmission = await _unitOfWork.GetRepository<QuestionSetSubmission>().GetFirstOrDefaultAsync(predicate: p => p.Id == questionSetSubmissionId
                                                                            && p.QuestionSetId == questionSet.Id).ConfigureAwait(false);
                if (questionSetSubmission == null)
                {
                    _logger.LogWarning("Question set submission not found with id: {questionSetSubmissionId} for user id: {currentUserId}", questionSetSubmission, currentUserId);
                    throw new EntityNotFoundException("Exam submission not found");
                }
                var questionSetSubmissionAnswers = await _unitOfWork.GetRepository<QuestionSetSubmissionAnswer>().GetAllAsync(predicate: p => p.QuestionSetSubmissionId == questionSetSubmissionId,
                                                            include: src => src.Include(x => x.QuestionSetQuestion.QuestionPoolQuestion.Question.QuestionOptions)).ConfigureAwait(false);

                var ObtainedMarks = questionSetResult.TotalMark - questionSetResult.NegativeMark;
                var resultMark = ObtainedMarks > 0 ? ObtainedMarks : 0;
                var responseModel = new QuestionSetUserResultResponseModel()
                {
                    QuestionSetSubmissionId = questionSetSubmissionId,
                    Name = questionSet.Name,
                    Description = questionSet.Description,
                    ThumbnailUrl = questionSet.ThumbnailUrl,
                    TotalMarks = Math.Round(questionSet.QuestionMarking * questionSetSubmissionAnswers.Count, 4),
                    NegativeMarks = questionSetResult.NegativeMark,
                    ObtainedMarks = resultMark,
                    SubmissionDate = questionSetSubmission?.EndTime ?? questionSetSubmission?.StartTime ?? questionSetSubmission.CreatedOn,
                    Duration = questionSet.Duration != 0 ? TimeSpan.FromSeconds(questionSet.Duration).ToString(@"hh\:mm\:ss") : string.Empty,
                    CompleteDuration = questionSet.Duration == 0 || questionSetSubmission.EndTime == default ? string.Empty :
                                                                TimeSpan.FromSeconds((Convert.ToDateTime(questionSetSubmission?.EndTime)
                                                                            - Convert.ToDateTime(questionSetSubmission?.StartTime)).TotalSeconds).ToString(@"hh\:mm\:ss"),
                    User = new UserModel
                    {
                        Id = (Guid)(questionSetResult?.User?.Id),
                        Email = questionSetResult?.User?.Email,
                        FullName = questionSetResult?.User?.FullName,
                        ImageUrl = questionSetResult?.User?.ImageUrl,
                    },
                    Results = new List<QuestionSetAnswerResultModel>()
                };

                foreach (var item in questionSetSubmissionAnswers)
                {
                    var result = new QuestionSetAnswerResultModel
                    {
                        Id = item.QuestionSetQuestion.QuestionPoolQuestion.Question.Id,
                        Name = item.QuestionSetQuestion.QuestionPoolQuestion.Question?.Name,
                        Hints = item.QuestionSetQuestion.QuestionPoolQuestion.Question?.Hints,
                        Description = item.QuestionSetQuestion.QuestionPoolQuestion.Question?.Description,
                        Type = item.QuestionSetQuestion.QuestionPoolQuestion.Question.Type,
                        IsCorrect = item.IsCorrect,
                        QuestionOptions = new List<QuestionResultOption>(),
                        OrderNumber = item.QuestionSetQuestion.Order
                    };

                    var selectedAnsIds = !string.IsNullOrWhiteSpace(item.SelectedAnswers) ? item.SelectedAnswers.Split(",").Select(Guid.Parse).ToList() : new List<Guid>();
                    item.QuestionSetQuestion.QuestionPoolQuestion.Question?.QuestionOptions.OrderBy(o => o.Order).ForEach(opt => result.QuestionOptions.Add(new QuestionResultOption
                    {
                        Id = opt.Id,
                        Value = opt.Option,
                        IsCorrect = opt.IsCorrect,
                        IsSelected = selectedAnsIds.Contains(opt.Id)
                    }));

                    responseModel.Results.Add(result);
                }
                responseModel.Results = responseModel.Results.OrderBy(x => x.OrderNumber).ToList();
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting result details.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while getting result details.");
            }
        }

        #endregion Exam Result Reports

        #region Private Methods

        /// <summary>
        /// Handle to insert into watch history
        /// </summary>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="currentTimeStamp">the current date time</param>
        /// <param name="questionSet">the instance of <see cref="QuestionSet"/></param>
        /// <returns></returns>
        private async Task InsertWatchHistory(Guid currentUserId, DateTime currentTimeStamp, QuestionSet questionSet)
        {
            var history = await _unitOfWork.GetRepository<WatchHistory>().GetFirstOrDefaultAsync(
                                predicate: p => p.UserId == currentUserId && p.LessonId == questionSet.Lesson.Id && p.CourseId == questionSet.Lesson.CourseId
                                ).ConfigureAwait(false);

            if (history != null && !history.IsPassed)
            {
                history.IsCompleted = true;
                history.IsPassed = true;
                history.UpdatedOn = currentTimeStamp;
                history.UpdatedBy = currentUserId;
                _unitOfWork.GetRepository<WatchHistory>().Update(history);
            }
            else
            {
                var watchHistory = new WatchHistory
                {
                    Id = Guid.NewGuid(),
                    CourseId = questionSet.Lesson.CourseId,
                    LessonId = questionSet.Lesson.Id,
                    UserId = currentUserId,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    IsCompleted = true,
                    IsPassed = true,
                };
                await _unitOfWork.GetRepository<WatchHistory>().InsertAsync(watchHistory).ConfigureAwait(false);
            }

            var totalLessonCount = await _unitOfWork.GetRepository<Lesson>().CountAsync(
                predicate: p => p.CourseId == questionSet.Lesson.CourseId && !p.IsDeleted && p.Status == CourseStatus.Published).ConfigureAwait(false);
            var completedLessonCount = await _unitOfWork.GetRepository<WatchHistory>().CountAsync(
                predicate: p => p.CourseId == questionSet.Lesson.CourseId && p.UserId == currentUserId && p.IsCompleted).ConfigureAwait(false);
            var percentage = (Convert.ToDouble(completedLessonCount) / Convert.ToDouble(totalLessonCount)) * 100;

            var courseEnrollment = await _unitOfWork.GetRepository<CourseEnrollment>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == questionSet.Lesson.CourseId && p.UserId == currentUserId
                                && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)
                ).ConfigureAwait(false);

            if (courseEnrollment != null)
            {
                courseEnrollment.Percentage = Convert.ToInt32(percentage);
                courseEnrollment.CurrentLessonId = questionSet.Lesson.Id;
                courseEnrollment.UpdatedBy = currentUserId;
                courseEnrollment.UpdatedOn = currentTimeStamp;
                if (percentage == 100)
                {
                    courseEnrollment.EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Completed;
                }
                _unitOfWork.GetRepository<CourseEnrollment>().Update(courseEnrollment);
            }
        }

        #endregion Private Methods
    }
}
