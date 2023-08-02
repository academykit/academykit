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
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;

    public class FeedbackService : BaseGenericService<Feedback, FeedbackBaseSearchCriteria>, IFeedbackService
    {
        public FeedbackService(
            IUnitOfWork unitOfWork,
            ILogger<FeedbackService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {
        }
        #region Protected Region

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Feedback, bool>> ConstructQueryConditions(Expression<Func<Feedback, bool>> predicate, FeedbackBaseSearchCriteria criteria)
        {
            var lesson = _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: p => p.Id.ToString() == criteria.LessonIdentity || p.Slug == criteria.LessonIdentity).Result;

            if (lesson == null)
            {
                _logger.LogWarning("Lesson with identity : {identity} not found for user with id : {id}.", criteria.LessonIdentity, criteria.CurrentUserId);
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
            }

            return predicate.And(x => x.LessonId == lesson.Id);
        }
        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="entity">the instance of <see cref="Feedback"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(Feedback entity, Guid currentUserId)
        {
            await ValidateAndGetLessonForFeedback(entity).ConfigureAwait(false);

            var feedbackSubmissions = await _unitOfWork.GetRepository<FeedbackSubmission>().ExistsAsync(
                    predicate: p => p.FeedbackId == entity.Id).ConfigureAwait(false);
            if (feedbackSubmissions)
            {
                _logger.LogWarning("Feedback with id : {id} having type : {type} contains Feedback submissions.", entity.Id, entity.Type);
                throw new ForbiddenException(_localizer.GetString("FeedbackContainsFeedbackSubmissions"));
            }
            _unitOfWork.GetRepository<FeedbackQuestionOption>().Delete(entity.FeedbackQuestionOptions);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Feedback, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Feedback, object> IncludeNavigationProperties(IQueryable<Feedback> query)
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Feedback entity)
        {
            await ValidateAndGetLessonForFeedback(entity).ConfigureAwait(false);

            var order = await _unitOfWork.GetRepository<Feedback>().MaxAsync(
              predicate: p => p.LessonId == entity.LessonId && p.IsActive,
              selector: x => (int?)x.Order
              ).ConfigureAwait(false);

            entity.Order = order == null ? 1 : order.Value + 1;

            if (entity.FeedbackQuestionOptions.Count > 0)
            {
                await _unitOfWork.GetRepository<FeedbackQuestionOption>().InsertAsync(entity.FeedbackQuestionOptions).ConfigureAwait(false);
            }
            await Task.FromResult(0);
        }

        /// <summary>
        /// Handel to populate live session retrieved entity
        /// </summary>
        /// <param name="entity">the instance of <see cref="LiveSession"/></param>
        /// <returns></returns>
        protected override async Task PopulateRetrievedEntity(Feedback entity)
        {
            entity.FeedbackQuestionOptions = await _unitOfWork.GetRepository<FeedbackQuestionOption>().GetAllAsync(predicate: p => p.FeedbackId == entity.Id).ConfigureAwait(false);
        }

        #endregion Protected Region

        #region Private Region

        /// <summary>
        /// Handle to validate and get lesson for Feedback
        /// </summary>
        /// <param name="entity">the instance of <see cref="Feedback"/></param>
        /// <returns>the instance of <see cref="Lesson"/></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private async Task<Lesson> ValidateAndGetLessonForFeedback(Feedback entity)
        {
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                            predicate: p => p.Id == entity.LessonId && !p.IsDeleted).ConfigureAwait(false);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson with id : {lessonId} not found for Feedback with id : {id}.", entity.LessonId, entity.Id);
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }
            if (lesson.Type != LessonType.Feedback)
            {
                _logger.LogWarning("Lesson with id : {lessonId} is of invalid lesson type to create,edit or delete Feedback for user with id :{userId}.", lesson.Id, entity.CreatedBy);
                throw new ForbiddenException(_localizer.GetString("InvalidLessonFeedbackType"));
            }
            await ValidateAndGetCourse(entity.CreatedBy, lesson.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);
            return lesson;
        }

        #endregion Private Region

        /// <summary>
        /// Handle to get list of student who has submitted feedback
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<IList<FeedbackSubmissionStudentResponseModel>> GetFeedbackSubmittedStudent(string lessonIdentity, Guid currentUserId)
        {
            try
            {
                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                   predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}.", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }
                if (lesson.Type != LessonType.Feedback)
                {
                    _logger.LogWarning("Lesson type not matched for feedback submission for lesson with id: {id} and user with id: {userId}.",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException(_localizer.GetString("InvalidLessonFeedbackType"));
                }
                await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);

                var userIds = await _unitOfWork.GetRepository<FeedbackSubmission>().GetAllAsync(
                    selector: s => s.UserId,
                    predicate: p => p.LessonId == lesson.Id).ConfigureAwait(false);

                userIds = userIds.Distinct().ToList();

                var users = await _unitOfWork.GetRepository<User>().GetAllAsync(
                    predicate: p => userIds.Contains(p.Id)
                    ).ConfigureAwait(false);

                var response = new List<FeedbackSubmissionStudentResponseModel>();

                users.ForEach(x => response.Add(new FeedbackSubmissionStudentResponseModel
                {
                    User = new UserModel(x),
                    LessonId = lesson.Id,
                    LessonSlug = lesson.Slug
                }));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch the student list who has submitted feedback.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("StudentListFetchError"));
            }
        }

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the Feedback id or slug</param>
        /// <param name="model">the instance of <see cref="FeedbackRequestModel"/> </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<Feedback> UpdateAsync(string identity, FeedbackRequestModel model, Guid currentUserId)
        {
            try
            {
                var existing = await GetByIdOrSlugAsync(identity, currentUserId).ConfigureAwait(false);
                var currentTimeStamp = DateTime.UtcNow;

                existing.Id = existing.Id;
                existing.Name = model.Name;
                existing.LessonId = model.LessonId;
                existing.Type = model.Type;
                existing.UpdatedBy = currentUserId;
                existing.UpdatedOn = currentTimeStamp;

                var feedbackQuestionOptions = new List<FeedbackQuestionOption>();

                if (model.Type == FeedbackTypeEnum.SingleChoice || model.Type == FeedbackTypeEnum.MultipleChoice)
                {
                    foreach (var item in model.Answers.Select((answer, i) => new { i, answer }))
                    {
                        feedbackQuestionOptions.Add(new FeedbackQuestionOption
                        {
                            Id = Guid.NewGuid(),
                            FeedbackId = existing.Id,
                            Order = item.i + 1,
                            Option = item.answer.Option,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp,
                        });
                    }
                }
                if (existing.FeedbackQuestionOptions.Count > 0)
                {
                    _unitOfWork.GetRepository<FeedbackQuestionOption>().Delete(existing.FeedbackQuestionOptions);
                }
                if (feedbackQuestionOptions.Count > 0)
                {
                    await _unitOfWork.GetRepository<FeedbackQuestionOption>().InsertAsync(feedbackQuestionOptions).ConfigureAwait(false);
                }
                _unitOfWork.GetRepository<Feedback>().Update(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update feedback.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("UpdateFeedBackError"));
            }
        }

        /// <summary>
        /// Handle to submit Feedbacks by the user
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="models">the list of <see cref="FeedbackSubmissionRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        public async Task FeedbackSubmissionAsync(string lessonIdentity, IList<FeedbackSubmissionRequestModel> models, Guid currentUserId)
        {
            try
            {
                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}.", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }
                if (lesson.Type != LessonType.Feedback)
                {
                    _logger.LogWarning("Lesson type not matched for feedback submission for lesson with id: {id} and user with id: {userId}.",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException(_localizer.GetString("InvalidLessonFeedbackType"));
                }
                if (lesson.Status != CourseStatus.Published)
                {
                    _logger.LogWarning("Lesson with id: {id} not published for user with id: {userId}.", lesson.Id, currentUserId);
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotpublished"));
                }

                var course = await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);
                var isAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (course.Status == CourseStatus.Completed)
                {
                    _logger.LogWarning("training with id : {courseId} is in {status} status to give Feedback for the user with id: {userId}.",
                        course.Id, course.Status, currentUserId);
                    throw new ForbiddenException(_localizer.GetString("FeedbackCompletedStatus"));
                }
                if (course.CourseTeachers.Any(x => x.UserId == currentUserId))
                {
                    _logger.LogWarning("User with id: {userId} is a teacher of the training with id: {courseId} and lesson with id: {lessonId} to submit the feedback.",
                        currentUserId, course.Id, lesson.Id);
                    throw new ForbiddenException(_localizer.GetString("TrainingTrainerCannotSubmitFeedback"));
                }
                if (isAdmin)
                {
                    _logger.LogWarning("User with id: {userId} is a teacher of the training with id: {courseId} and lesson with id: {lessonId} to submit the feedback.",
                       currentUserId, course.Id, lesson.Id);
                    throw new ForbiddenException(_localizer.GetString("AdminCannotSubmitFeedBack"));
                }

                var feedbacks = await _unitOfWork.GetRepository<Feedback>().GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id && p.IsActive,
                    include: src => src.Include(x => x.FeedbackQuestionOptions)
                    ).ConfigureAwait(false);

                var feedbackIds = feedbacks.Select(x => x.Id).ToList();

                var feebackSubmissionExists = await _unitOfWork.GetRepository<FeedbackSubmission>().ExistsAsync(
                    predicate: p => feedbackIds.Contains(p.FeedbackId) && p.UserId == currentUserId
                    ).ConfigureAwait(false);


                if (feebackSubmissionExists)
                {
                    _logger.LogWarning("User with id: {userId} cannot resubmit the feedback having id: {feedbackId}.", currentUserId, lesson.Id);
                    throw new ForbiddenException(_localizer.GetString("FeedBackCannotReSubmit"));
                }

                var watchHistory = await _unitOfWork.GetRepository<WatchHistory>().GetFirstOrDefaultAsync(
                    predicate: p => p.LessonId == lesson.Id && p.UserId == currentUserId
                    ).ConfigureAwait(false);

                var currentTimeStamp = DateTime.UtcNow;

                foreach (var item in models)
                {
                    var feedback = feedbacks.FirstOrDefault(x => x.Id == item.FeedbackId);
                    if (feedback != null)
                    {
                        await InsertSubmissionAsync(currentUserId, lesson.Id, currentTimeStamp, item, feedback).ConfigureAwait(false);
                    }
                }

                if (watchHistory == null)
                {
                    watchHistory = new WatchHistory
                    {
                        Id = Guid.NewGuid(),
                        LessonId = lesson.Id,
                        CourseId = lesson.CourseId,
                        UserId = currentUserId,
                        IsCompleted = true,
                        IsPassed = true,
                        CreatedBy = currentUserId,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = currentUserId,
                        UpdatedOn = currentTimeStamp,
                    };
                    await ManageStudentCourseComplete(course.Id, lesson.Id, currentUserId, currentTimeStamp).ConfigureAwait(false);

                    await _unitOfWork.GetRepository<WatchHistory>().InsertAsync(watchHistory).ConfigureAwait(false);
                }

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to submit the feedback.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("SubmitFeedBackError"));
            }
        }

        /// <summary>
        /// Handle to get feedback report
        /// </summary>
        /// <param name="lessonIdentity"> the lesson id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the byte</returns>
        public async Task<byte[]> GetFeedBackReportAsync(string lessonIdentity, Guid currentUserId)
        {
            return await ExecuteWithResultAsync(async () =>
            {

                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);

                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}.", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }
                if (lesson.Type != LessonType.Feedback)
                {
                    _logger.LogWarning("Lesson type not matched for Feedback fetch for lesson with id: {id} and user with id: {userId}.",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException(_localizer.GetString("InvalidLessonFeedbackType"));
                }

                await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);
                var feedback = await _unitOfWork.GetRepository<Feedback>().GetAllAsync(predicate: p => p.LessonId == lesson.Id, include: source => source.Include(x => x.FeedbackQuestionOptions)).ConfigureAwait(false);
                var feedbackSubmissions = await _unitOfWork.GetRepository<FeedbackSubmission>().GetAllAsync(predicate: p => p.LessonId == lesson.Id,
                include: source => source.Include(x => x.User)).ConfigureAwait(false);

                if (feedbackSubmissions.Count == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("FeedBackReportNotFound"));
                }

                var users = feedbackSubmissions.DistinctBy(x => x.UserId).Select(x => x.User).ToList();
                var feedbackQuestions = string.Join(",", feedback.OrderBy(x => x.Order).Select(x => x.Name));
                StringBuilder builder = new StringBuilder();
                var i = 1;
                builder.AppendLine("S.N,Name,Email," + feedbackQuestions);
                foreach (var user in users)
                {
                    builder.Append(i);
                    builder.Append(",");
                    builder.Append($"{user.FullName}");
                    builder.Append(",");
                    builder.Append($"{user.Email}");
                    builder.Append(",");
                    var questionList = feedbackQuestions.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var qustion in questionList)
                    {
                        var feedBack = feedback.FirstOrDefault(x => x.Name == qustion);
                        if (feedback != null)
                        {
                            var feedbackanswer = feedbackSubmissions.FirstOrDefault(x => x.FeedbackId == feedBack.Id && x.UserId == user.Id);
                            if (feedbackanswer != default)
                            {
                                if (feedBack.Type == FeedbackTypeEnum.Subjective)
                                {
                                    var answer = Regex.Replace(feedbackanswer.Answer, "<[a-zA-Z/].*?>", string.Empty);
                                    builder.Append(answer);
                                }

                                if (feedBack.Type == FeedbackTypeEnum.Rating)
                                {
                                    builder.Append(feedbackanswer.Rating);
                                }

                                if (feedBack.Type == FeedbackTypeEnum.SingleChoice)
                                {
                                    var singleAnswer = feedBack.FeedbackQuestionOptions.FirstOrDefault(x => x.Id.ToString() == feedbackanswer.SelectedOption.ToString());
                                    if (singleAnswer != null)
                                    {
                                        singleAnswer.Option = Regex.Replace(singleAnswer.Option, "<[a-zA-Z/].*?>", string.Empty);
                                        builder.Append(singleAnswer.Option);
                                    }
                                }

                                if (feedBack.Type == FeedbackTypeEnum.MultipleChoice)
                                {
                                    var options = feedbackanswer.SelectedOption.Split(",");
                                    var choices = new List<string>();
                                    foreach (var opt in options)
                                    {
                                        var optAnswer = feedBack.FeedbackQuestionOptions.FirstOrDefault(x => x.Id.ToString() == opt)?.Option;
                                        var removeHtml = Regex.Replace(optAnswer, "<[a-zA-Z/].*?>", string.Empty);
                                        choices.Add(removeHtml);
                                    }
                                    var choiceAnswer = string.Join(" | ", choices);
                                    builder.Append(choiceAnswer);
                                }
                            }
                        }
                        builder.Append(",");
                    }
                    builder.AppendLine();
                    i++;
                }
                var fileContents = Encoding.UTF8.GetBytes(builder.ToString());
                return fileContents;
            });
        }

        #region Private Methods

        /// <summary>
        /// Handle to insert Feedback submission
        /// </summary>
        /// <param name="currentUserId">the current logged in user</param>
        /// <param name="lessonId">the lesson id</param>
        /// <param name="currentTimeStamp">the current time stamp</param>
        /// <param name="item">the instance of <see cref="FeedbackSubmissionRequestModel"</param>
        /// <param name="Feedback">the instance of <see cref="Feedback"/></param>
        /// <returns></returns>
        private async Task InsertSubmissionAsync(Guid currentUserId, Guid lessonId, DateTime currentTimeStamp, FeedbackSubmissionRequestModel item, Feedback feedback)
        {
            var feedbackSubmission = new FeedbackSubmission
            {
                Id = Guid.NewGuid(),
                LessonId = lessonId,
                FeedbackId = feedback.Id,
                UserId = currentUserId,
                CreatedBy = currentUserId,
                CreatedOn = currentTimeStamp,
                UpdatedBy = currentUserId,
                UpdatedOn = currentTimeStamp,
            };

            if (feedback.Type == FeedbackTypeEnum.SingleChoice || feedback.Type == FeedbackTypeEnum.MultipleChoice)
            {
                feedbackSubmission.SelectedOption = string.Join(",", item.SelectedOption);
            }
            if (feedback.Type == FeedbackTypeEnum.Subjective)
            {
                feedbackSubmission.Answer = item.Answer;
            }
            if (feedback.Type == FeedbackTypeEnum.Rating)
            {
                feedbackSubmission.Rating = item.Rating;
            }
            await _unitOfWork.GetRepository<FeedbackSubmission>().InsertAsync(feedbackSubmission).ConfigureAwait(false);
        }

        #endregion Private Methods

        /// <summary>
        /// Handle to search feedback
        /// </summary>
        /// <param name="searchCriteria">the instance of <see cref="FeedbackBaseSearchCriteria"/></param>
        /// <returns>the list of <see cref="FeedbackResponseModel"/></returns>
        public async Task<IList<FeedbackResponseModel>> SearchAsync(FeedbackBaseSearchCriteria searchCriteria)
        {
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
               predicate: p => p.Id.ToString() == searchCriteria.LessonIdentity || p.Slug == searchCriteria.LessonIdentity
               ).ConfigureAwait(false);

            if (lesson == null)
            {
                _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}.", searchCriteria.LessonIdentity, searchCriteria.CurrentUserId);
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }
            if (lesson.Type != LessonType.Feedback)
            {
                _logger.LogWarning("Lesson type not matched for Feedback fetch for lesson with id: {id} and user with id: {userId}.",
                                    lesson.Id, searchCriteria.CurrentUserId);
                throw new ForbiddenException(_localizer.GetString("InvalidLessonFeedbackType"));
            }

            var course = await ValidateAndGetCourse(searchCriteria.CurrentUserId, lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);

            var isTeacher = course.CourseTeachers.Any(x => x.UserId == searchCriteria.CurrentUserId);
            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(searchCriteria.CurrentUserId).ConfigureAwait(false);

            if (!isTeacher && !isSuperAdminOrAdmin && searchCriteria.UserId == null)
            {
                searchCriteria.UserId = searchCriteria.CurrentUserId;
            }

            var predicate = PredicateBuilder.New<Feedback>(true);
            predicate = predicate.And(x => x.LessonId == lesson.Id);

            var feedbacks = await _unitOfWork.GetRepository<Feedback>().GetAllAsync(
                predicate: p => p.LessonId == lesson.Id,
                include: src => src.Include(x => x.FeedbackQuestionOptions),
                orderBy: o => o.OrderBy(x => x.Order)
                ).ConfigureAwait(false);

            var userFeedbacks = await _unitOfWork.GetRepository<FeedbackSubmission>().GetAllAsync(
                predicate: p => p.LessonId == lesson.Id && searchCriteria.UserId.HasValue && p.UserId == searchCriteria.UserId.Value,
                include: src => src.Include(x => x.User)
                ).ConfigureAwait(false);

            var response = new List<FeedbackResponseModel>();

            foreach (var item in feedbacks)
            {
                MapFeedback(userFeedbacks, item, response);
            }
            return response;
        }

        private static void MapFeedback(IList<FeedbackSubmission> userFeedbacks, Feedback item, IList<FeedbackResponseModel> response)
        {
            var userFeedback = userFeedbacks.FirstOrDefault(x => x.FeedbackId == item.Id);
            var data = new FeedbackResponseModel
            {
                Id = item.Id,
                FeedbackSubmissionId = userFeedback?.Id,
                LessonId = item.LessonId,
                Name = item.Name,
                Order = item.Order,
                Type = item.Type,
                IsActive = item.IsActive,
                User = item.User == null ? new UserModel() : new UserModel(item.User),
                Student = userFeedback == null ? null : new UserModel(userFeedback.User),
                Answer = userFeedback?.Answer,
                Rating = userFeedback?.Rating,
                FeedbackQuestionOptions = new List<FeedbackQuestionOptionResponseModel>(),
            };

            if (item.Type == FeedbackTypeEnum.SingleChoice || item.Type == FeedbackTypeEnum.MultipleChoice)
            {
                var selectedAnsIds = !string.IsNullOrWhiteSpace(userFeedback?.SelectedOption) ?
                                        userFeedback?.SelectedOption.Split(",").Select(Guid.Parse).ToList() : new List<Guid>();
                item.FeedbackQuestionOptions?.OrderBy(x => x.Order).ToList().ForEach(x =>
                                data.FeedbackQuestionOptions.Add(new FeedbackQuestionOptionResponseModel()
                                {
                                    Id = x.Id,
                                    FeedbackId = x.FeedbackId,
                                    FeedbackName = x.Feedback?.Name,
                                    Option = x.Option,
                                    IsSelected = userFeedback != null ? selectedAnsIds?.Contains(x.Id) : null,
                                    Order = x.Order,
                                }));
            }
            response.Add(data);
        }
    }
}