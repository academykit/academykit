﻿namespace AcademyKit.Infrastructure.Services
{
    using System.Linq;
    using System.Linq.Expressions;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class AssignmentService
        : BaseGenericService<Assignment, AssignmentBaseSearchCriteria>,
            IAssignmentService
    {
        private readonly ICourseService _courseService;

        public AssignmentService(
            IUnitOfWork unitOfWork,
            ILogger<AssignmentService> logger,
            ICourseService courseService,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer)
        {
            _courseService = courseService;
        }

        #region Protected Region

        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="entity">the instance of <see cref="Assignment"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(
            Assignment entity,
            Guid currentUserId
        )
        {
            await ValidateAndGetLessonForAssignment(entity).ConfigureAwait(false);

            var assignmentSubmissions = await _unitOfWork
                .GetRepository<AssignmentSubmission>()
                .ExistsAsync(predicate: p => p.AssignmentId == entity.Id)
                .ConfigureAwait(false);
            if (assignmentSubmissions)
            {
                _logger.LogWarning(
                    "Assignment with id : {id} having type : {type} contains assignment submissions.",
                    entity.Id,
                    entity.Type
                );
                throw new ForbiddenException(_localizer.GetString("AssignmentSubmission"));
            }

            _unitOfWork.GetRepository<AssignmentAttachment>().Delete(entity.AssignmentAttachments);
            _unitOfWork
                .GetRepository<AssignmentQuestionOption>()
                .Delete(entity.AssignmentQuestionOptions);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Assignment, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Assignment, object> IncludeNavigationProperties(
            IQueryable<Assignment> query
        )
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Assignment entity)
        {
            await ValidateAndGetLessonForAssignment(entity).ConfigureAwait(false);

            var order = await _unitOfWork
                .GetRepository<Assignment>()
                .MaxAsync(
                    predicate: p => p.LessonId == entity.LessonId && p.IsActive,
                    selector: x => (int?)x.Order
                )
                .ConfigureAwait(false);
            entity.Order = order == null ? 1 : order.Value + 1;
            if (entity.AssignmentQuestionOptions.Count > 0)
            {
                await _unitOfWork
                    .GetRepository<AssignmentQuestionOption>()
                    .InsertAsync(entity.AssignmentQuestionOptions)
                    .ConfigureAwait(false);
            }

            if (entity.AssignmentAttachments.Count > 0)
            {
                await _unitOfWork
                    .GetRepository<AssignmentAttachment>()
                    .InsertAsync(entity.AssignmentAttachments)
                    .ConfigureAwait(false);
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Handel to populate live session retrieved entity
        /// </summary>
        /// <param name="entity">the instance of <see cref="LiveSession"/></param>
        /// <returns></returns>
        protected override async Task PopulateRetrievedEntity(Assignment entity)
        {
            entity.AssignmentAttachments = await _unitOfWork
                .GetRepository<AssignmentAttachment>()
                .GetAllAsync(predicate: p => p.AssignmentId == entity.Id)
                .ConfigureAwait(false);
            entity.AssignmentQuestionOptions = await _unitOfWork
                .GetRepository<AssignmentQuestionOption>()
                .GetAllAsync(predicate: p => p.AssignmentId == entity.Id)
                .ConfigureAwait(false);
        }

        #endregion Protected Region

        #region Private Region

        /// <summary>
        /// Handle to validate and get lesson for assignment
        /// </summary>
        /// <param name="entity">the instance of <see cref="Assignment"/></param>
        /// <returns>the instance of <see cref="Lesson"/></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private async Task<Lesson> ValidateAndGetLessonForAssignment(Assignment entity)
        {
            var lesson = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == entity.LessonId && !p.IsDeleted)
                .ConfigureAwait(false);
            if (lesson == null)
            {
                _logger.LogWarning(
                    "Lesson with id : {lessonId} not found for assignment with id : {id}.",
                    entity.LessonId,
                    entity.Id
                );
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }

            if (lesson.Type != LessonType.Assignment)
            {
                _logger.LogWarning(
                    "Lesson with id : {lessonId} is of invalid lesson type to create,edit or delete assignment for user with id :{userId}.",
                    lesson.Id,
                    entity.CreatedBy
                );
                throw new ForbiddenException(_localizer.GetString("InvalidLessonTypeAssignment"));
            }

            await ValidateAndGetCourse(
                    entity.CreatedBy,
                    lesson.CourseId.ToString(),
                    validateForModify: true
                )
                .ConfigureAwait(false);
            return lesson;
        }

        #endregion Private Region

        #region Public Methods

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the assignment id or slug</param>
        /// <param name="model">the instance of <see cref="AssignmentRequestModel"/> </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<Assignment> UpdateAsync(
            string identity,
            AssignmentRequestModel model,
            Guid currentUserId
        )
        {
            try
            {
                var existing = await GetByIdOrSlugAsync(identity, currentUserId)
                    .ConfigureAwait(false);
                var currentTimeStamp = DateTime.UtcNow;

                existing.Id = existing.Id;
                existing.Name = model.Name;
                existing.Hints = model.Hints;
                existing.LessonId = model.LessonId;
                existing.Type = model.Type;
                existing.Description = model.Description;
                existing.UpdatedBy = currentUserId;
                existing.UpdatedOn = currentTimeStamp;

                var assignmentQuestionOptions = new List<AssignmentQuestionOption>();
                var assignmentAttachments = new List<AssignmentAttachment>();

                if (
                    model.Type == QuestionTypeEnum.SingleChoice
                    || model.Type == QuestionTypeEnum.MultipleChoice
                )
                {
                    foreach (var item in model.Answers.Select((answer, i) => new { i, answer }))
                    {
                        assignmentQuestionOptions.Add(
                            new AssignmentQuestionOption
                            {
                                Id = Guid.NewGuid(),
                                AssignmentId = existing.Id,
                                Order = item.i + 1,
                                Option = item.answer.Option,
                                IsCorrect = item.answer.IsCorrect,
                                CreatedBy = currentUserId,
                                CreatedOn = currentTimeStamp,
                                UpdatedBy = currentUserId,
                                UpdatedOn = currentTimeStamp,
                            }
                        );
                    }
                }

                if (model.Type == QuestionTypeEnum.Subjective && model.FileUrls?.Count > 0)
                {
                    foreach (var item in model.FileUrls.Select((fileUrl, i) => new { i, fileUrl }))
                    {
                        assignmentAttachments.Add(
                            new AssignmentAttachment
                            {
                                Id = Guid.NewGuid(),
                                AssignmentId = existing.Id,
                                FileUrl = item.fileUrl,
                                Order = item.i + 1,
                                CreatedBy = currentUserId,
                                CreatedOn = currentTimeStamp,
                                UpdatedBy = currentUserId,
                                UpdatedOn = currentTimeStamp,
                            }
                        );
                    }
                }

                if (existing.AssignmentAttachments.Count > 0)
                {
                    _unitOfWork
                        .GetRepository<AssignmentAttachment>()
                        .Delete(existing.AssignmentAttachments);
                }

                if (existing.AssignmentQuestionOptions.Count > 0)
                {
                    _unitOfWork
                        .GetRepository<AssignmentQuestionOption>()
                        .Delete(existing.AssignmentQuestionOptions);
                }

                if (assignmentAttachments.Count > 0)
                {
                    await _unitOfWork
                        .GetRepository<AssignmentAttachment>()
                        .InsertAsync(assignmentAttachments)
                        .ConfigureAwait(false);
                }

                if (assignmentQuestionOptions.Count > 0)
                {
                    await _unitOfWork
                        .GetRepository<AssignmentQuestionOption>()
                        .InsertAsync(assignmentQuestionOptions)
                        .ConfigureAwait(false);
                }

                _unitOfWork.GetRepository<Assignment>().Update(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update assignment.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorOccurredUpdateAssignment"));
            }
        }

        /// <summary>
        /// Handle to submit assignments by the user
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="models">the list of <see cref="AssignmentSubmissionRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        public async Task AssignmentSubmissionAsync(
            string lessonIdentity,
            IList<AssignmentSubmissionRequestModel> models,
            Guid currentUserId
        )
        {
            try
            {
                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity
                    )
                    .ConfigureAwait(false);
                if (lesson == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (lesson.Type != LessonType.Assignment)
                {
                    throw new ForbiddenException(
                        _localizer.GetString("InvalidLessonAssignmentType")
                    );
                }

                if (lesson.Status != CourseStatus.Published)
                {
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotpublished"));
                }

                var course = await ValidateAndGetCourse(
                        currentUserId,
                        lesson.CourseId.ToString(),
                        validateForModify: false
                    )
                    .ConfigureAwait(false);
                if (course.Status == CourseStatus.Completed)
                {
                    _logger.LogWarning(
                        "Training with id : {courseId} is in {status} status to give assignment for the user with id: {userId}.",
                        course.Id,
                        course.Status,
                        currentUserId
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("CannotSubmitAssignmentStatusCompleted")
                    );
                }

                if (course.CourseTeachers.Any(x => x.UserId == currentUserId))
                {
                    _logger.LogWarning(
                        "User with id: {userId} is a teacher of the training with id: {courseId} and lesson with id: {lessonId} to submit the assignment.",
                        currentUserId,
                        course.Id,
                        lesson.Id
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("TrainingTeacherCannotSubmitAssignment")
                    );
                }

                var assignmentReviewExist = await _unitOfWork
                    .GetRepository<AssignmentReview>()
                    .ExistsAsync(predicate: p =>
                        p.LessonId == lesson.Id && p.UserId == currentUserId && !p.IsDeleted
                    )
                    .ConfigureAwait(false);
                if (assignmentReviewExist)
                {
                    _logger.LogWarning(
                        "Assignment review exist for lesson with id: {lessonId} and user with id : {userId}.",
                        lesson.Id,
                        currentUserId
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("ReviewAlreadyGivenAssignment")
                    );
                }

                var assignments = await _unitOfWork
                    .GetRepository<Assignment>()
                    .GetAllAsync(
                        predicate: p => p.LessonId == lesson.Id && p.IsActive,
                        include: src => src.Include(x => x.AssignmentQuestionOptions)
                    )
                    .ConfigureAwait(false);

                var watchHistory = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.LessonId == lesson.Id && p.UserId == currentUserId
                    )
                    .ConfigureAwait(false);

                var currentTimeStamp = DateTime.UtcNow;
                if (
                    !await IsSuperAdminOrAdminOrTrainerOfTraining(
                            currentUserId,
                            lesson.CourseId.ToString(),
                            TrainingTypeEnum.Course
                        )
                        .ConfigureAwait(false)
                )
                {
                    foreach (var item in models)
                    {
                        var assignment = assignments.FirstOrDefault(x => x.Id == item.AssignmentId);
                        if (assignment != null)
                        {
                            if (item.Id != default)
                            {
                                await UpdateSubmissionAsync(
                                        currentUserId,
                                        currentTimeStamp,
                                        item,
                                        assignment
                                    )
                                    .ConfigureAwait(false);
                            }
                            else
                            {
                                await InsertSubmissionAsync(
                                        currentUserId,
                                        lesson.Id,
                                        currentTimeStamp,
                                        item,
                                        assignment
                                    )
                                    .ConfigureAwait(false);
                            }
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
                            IsPassed = false,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp,
                        };
                        await ManageStudentCourseComplete(
                                course.Id,
                                lesson.Id,
                                currentUserId,
                                currentTimeStamp
                            )
                            .ConfigureAwait(false);

                        await _unitOfWork
                            .GetRepository<WatchHistory>()
                            .InsertAsync(watchHistory)
                            .ConfigureAwait(false);
                    }

                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to submit the assignment.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorOccuredSubmitAssignment"));
            }
        }

        /// <summary>
        /// Handle to fetch student submitted assignment
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="userId">the user id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the instance of <see cref="AssignmentSubmissionStudentResponseModel"/></returns>
        public async Task<AssignmentSubmissionStudentResponseModel> GetStudentSubmittedAssignment(
            string lessonIdentity,
            Guid userId,
            Guid currentUserId
        )
        {
            try
            {
                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity
                    )
                    .ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity not found for user");
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (lesson.Type != LessonType.Assignment)
                {
                    _logger.LogWarning(
                        "Lesson type not matched for assignment submission for lesson with and user."
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("InvalidLessonAssignmentType")
                    );
                }

                var course = await ValidateAndGetCourse(
                        currentUserId,
                        lesson.CourseId.ToString(),
                        validateForModify: false
                    )
                    .ConfigureAwait(false);

                var predicate = PredicateBuilder.New<Assignment>(true);
                predicate = predicate.And(x => x.LessonId == lesson.Id);

                var assignments = await _unitOfWork
                    .GetRepository<Assignment>()
                    .GetAllAsync(
                        predicate: p => p.LessonId == lesson.Id,
                        include: src =>
                            src.Include(x => x.AssignmentAttachments)
                                .Include(x => x.AssignmentQuestionOptions),
                        orderBy: x => x.OrderBy(o => o.Order)
                    )
                    .ConfigureAwait(false);

                var userAssignments = await _unitOfWork
                    .GetRepository<AssignmentSubmission>()
                    .GetAllAsync(
                        predicate: p => p.LessonId == lesson.Id && p.UserId == userId,
                        include: src =>
                            src.Include(x => x.AssignmentSubmissionAttachments).Include(x => x.User)
                    )
                    .ConfigureAwait(false);

                var assignmentReview = await _unitOfWork
                    .GetRepository<AssignmentReview>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.LessonId == lesson.Id && p.UserId == userId,
                        include: src => src.Include(x => x.User)
                    )
                    .ConfigureAwait(false);

                var watchHistory = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.LessonId == lesson.Id && p.UserId == userId
                    )
                    .ConfigureAwait(false);

                var user = await _unitOfWork
                    .GetRepository<User>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Id == currentUserId)
                    .ConfigureAwait(false);

                var response = new AssignmentSubmissionStudentResponseModel
                {
                    LessonId = lesson.Id,
                    LessonSlug = lesson.Slug,
                    User = user != null ? new UserModel(user) : null,
                    Assignments = new List<AssignmentResponseModel>()
                };
                if (assignmentReview != null)
                {
                    var teacher = await _unitOfWork
                        .GetRepository<User>()
                        .GetFirstOrDefaultAsync(predicate: p => p.Id == assignmentReview.CreatedBy)
                        .ConfigureAwait(false);
                    response.AssignmentReview = new AssignmentReviewResponseModel
                    {
                        Id = assignmentReview.Id,
                        LessonId = assignmentReview.LessonId,
                        Mark = assignmentReview.Mark,
                        Review = assignmentReview.Review,
                        IsCompleted = watchHistory?.IsCompleted,
                        IsPassed = watchHistory?.IsPassed,
                        UserId = assignmentReview.UserId,
                        User = new UserModel(assignmentReview.User),
                        Teacher = teacher != null ? new UserModel(teacher) : null
                    };
                }

                response.UserStatus = _courseService.GetUserCourseEnrollmentStatus(
                    course,
                    currentUserId
                );

                foreach (var item in assignments)
                {
                    MapAssignment(true, userAssignments, item, response.Assignments);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while trying to fetch the student submitted assignment."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(
                        _localizer.GetString("ErroprOccurredFetchStudentSubmittedAssignment")
                    );
            }
        }

        /// <summary>
        /// Handle to search assignment
        /// </summary>
        /// <param name="searchCriteria">the instance of <see cref="AssignmentBaseSearchCriteria"/></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        public async Task<IList<AssignmentResponseModel>> SearchAsync(
            AssignmentBaseSearchCriteria searchCriteria
        )
        {
            var lesson = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.Id.ToString() == searchCriteria.LessonIdentity
                    || p.Slug == searchCriteria.LessonIdentity
                )
                .ConfigureAwait(false);

            if (lesson == null)
            {
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }

            if (lesson.Type != LessonType.Assignment)
            {
                throw new ForbiddenException(_localizer.GetString("InvalidLessonAssignmentType"));
            }

            var currentDateTime = DateTime.UtcNow;
            var nepalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            var startDateLocal = TimeZoneInfo.ConvertTime(
                lesson.StartDate.Value.ToLocalTime(),
                nepalTimeZone
            );
            var currentLocalDate = TimeZoneInfo.ConvertTime(currentDateTime, nepalTimeZone);
            var hasAuthority = await IsSuperAdminOrAdminOrTrainerOfTraining(
                    searchCriteria.CurrentUserId,
                    lesson.CourseId.ToString(),
                    TrainingTypeEnum.Course
                )
                .ConfigureAwait(false);
            if (startDateLocal > currentLocalDate && !hasAuthority)
            {
                throw new ForbiddenException(_localizer.GetString("AssignmentStartTimeException"));
            }

            var course = await ValidateAndGetCourse(
                    searchCriteria.CurrentUserId,
                    lesson.CourseId.ToString(),
                    validateForModify: false
                )
                .ConfigureAwait(false);

            var predicate = PredicateBuilder.New<Assignment>(true);
            predicate = predicate.And(x => x.LessonId == lesson.Id);
            var IsValidUser = await IsSuperAdminOrAdminOrTrainerOfTraining(
                searchCriteria.CurrentUserId,
                lesson.CourseId.ToString(),
                TrainingTypeEnum.Course
            );
            var assignments = await _unitOfWork
                .GetRepository<Assignment>()
                .GetAllAsync(
                    predicate: predicate,
                    include: src =>
                        src.Include(x => x.AssignmentAttachments)
                            .Include(x => x.AssignmentQuestionOptions)
                            .Include(x => x.User),
                    orderBy: x => x.OrderBy(a => a.Order)
                )
                .ConfigureAwait(false);
            if (searchCriteria.UserId == default)
            {
                searchCriteria.UserId = searchCriteria.CurrentUserId;
            }

            var userAssignments = await _unitOfWork
                .GetRepository<AssignmentSubmission>()
                .GetAllAsync(
                    predicate: p =>
                        p.LessonId == lesson.Id
                        && searchCriteria.UserId.HasValue
                        && p.UserId == searchCriteria.UserId.Value,
                    include: src =>
                        src.Include(x => x.AssignmentSubmissionAttachments).Include(x => x.User)
                )
                .ConfigureAwait(false);

            var assignmentReview = await _unitOfWork
                .GetRepository<AssignmentReview>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.LessonId == lesson.Id
                    && searchCriteria.UserId.HasValue
                    && p.UserId == searchCriteria.UserId.Value
                )
                .ConfigureAwait(false);
            var response = new List<AssignmentResponseModel>();
            foreach (var item in assignments)
            {
                MapAssignment(IsValidUser, userAssignments, item, response);
            }

            return response;
        }

        #endregion Public Methods

        #region Assignment Review

        /// <summary>
        /// Handle to review user assignment
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="model">the instance of <see cref="AssignmentReviewRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task AssignmentReviewAsync(
            string lessonIdentity,
            AssignmentReviewRequestModel model,
            Guid currentUserId
        )
        {
            try
            {
                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity
                    )
                    .ConfigureAwait(false);
                if (lesson == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (lesson.Type != LessonType.Assignment)
                {
                    throw new ForbiddenException(
                        _localizer.GetString("InvalidLessonAssignmentType")
                    );
                }

                await ValidateAndGetCourse(
                        currentUserId,
                        lesson.CourseId.ToString(),
                        validateForModify: true
                    )
                    .ConfigureAwait(false);

                var currentTimeStamp = DateTime.UtcNow;
                var assignmentReview = new AssignmentReview
                {
                    Id = Guid.NewGuid(),
                    LessonId = lesson.Id,
                    Mark = model.Marks,
                    Review = model.Review,
                    UserId = model.UserId,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                };
                var watchHistory = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.LessonId == lesson.Id && p.UserId == model.UserId
                    )
                    .ConfigureAwait(false);
                if (watchHistory != null)
                {
                    watchHistory.IsPassed = model.IsPassed;
                    _unitOfWork.GetRepository<WatchHistory>().Update(watchHistory);
                }
                else
                {
                    watchHistory = new WatchHistory
                    {
                        Id = Guid.NewGuid(),
                        LessonId = lesson.Id,
                        CourseId = lesson.CourseId,
                        UserId = model.UserId,
                        IsCompleted = true,
                        IsPassed = model.IsPassed,
                        CreatedBy = currentUserId,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = currentUserId,
                        UpdatedOn = currentTimeStamp,
                    };
                    await _unitOfWork
                        .GetRepository<WatchHistory>()
                        .InsertAsync(watchHistory)
                        .ConfigureAwait(false);
                }

                await _unitOfWork
                    .GetRepository<AssignmentReview>()
                    .InsertAsync(assignmentReview)
                    .ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to submit assignment review.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(
                        _localizer.GetString("ErrorOccurredSubmitAssignmentReview")
                    );
            }
        }

        /// <summary>
        /// Handle to update user assignment review
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="id">assignment review id</param>
        /// <param name="model">the instance of <see cref="AssignmentReviewRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task UpdateAssignmentReviewAsync(
            string lessonIdentity,
            Guid id,
            AssignmentReviewRequestModel model,
            Guid currentUserId
        )
        {
            try
            {
                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity
                    )
                    .ConfigureAwait(false);
                if (lesson == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (lesson.Type != LessonType.Assignment)
                {
                    throw new ForbiddenException(
                        _localizer.GetString("InvalidLessonAssignmentType")
                    );
                }

                await ValidateAndGetCourse(
                        currentUserId,
                        lesson.CourseId.ToString(),
                        validateForModify: true
                    )
                    .ConfigureAwait(false);

                var assignmentReview = await _unitOfWork
                    .GetRepository<AssignmentReview>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id == id
                        && p.LessonId == lesson.Id
                        && p.UserId == model.UserId
                        && !p.IsDeleted
                    )
                    .ConfigureAwait(false);
                if (assignmentReview == null)
                {
                    _logger.LogWarning(
                        "Assignment review with id: {id} not found for user with id: {userId} and lesson with id: {lessonId}.",
                        id,
                        currentUserId,
                        lesson.Id
                    );
                    throw new EntityNotFoundException(
                        _localizer.GetString("AssignmentReviewNotFound")
                    );
                }

                var currentTimeStamp = DateTime.UtcNow;

                assignmentReview.Mark = model.Marks;
                assignmentReview.Review = model.Review;
                assignmentReview.UpdatedBy = currentUserId;
                assignmentReview.UpdatedOn = currentTimeStamp;

                var watchHistory = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.LessonId == lesson.Id && p.UserId == model.UserId
                    )
                    .ConfigureAwait(false);
                if (watchHistory != null)
                {
                    watchHistory.IsPassed = model.IsPassed;
                    _unitOfWork.GetRepository<WatchHistory>().Update(watchHistory);
                }
                else
                {
                    watchHistory = new WatchHistory
                    {
                        Id = Guid.NewGuid(),
                        LessonId = lesson.Id,
                        CourseId = lesson.CourseId,
                        UserId = model.UserId,
                        IsCompleted = true,
                        IsPassed = model.IsPassed,
                        CreatedBy = currentUserId,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = currentUserId,
                        UpdatedOn = currentTimeStamp,
                    };
                    await _unitOfWork
                        .GetRepository<WatchHistory>()
                        .InsertAsync(watchHistory)
                        .ConfigureAwait(false);
                }

                _unitOfWork.GetRepository<AssignmentReview>().Update(assignmentReview);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update assignment review.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorOccurredUpdateAssignment"));
            }
        }

        /// <summary>
        /// Handle to delete assignment review
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="id">the assignment review id</param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task DeleteReviewAsync(string lessonIdentity, Guid id, Guid currentUserId)
        {
            try
            {
                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity
                    )
                    .ConfigureAwait(false);
                if (lesson == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (lesson.Type != LessonType.Assignment)
                {
                    throw new ForbiddenException(
                        _localizer.GetString("InvalidLessonAssignmentType")
                    );
                }

                await ValidateAndGetCourse(
                        currentUserId,
                        lesson.CourseId.ToString(),
                        validateForModify: true
                    )
                    .ConfigureAwait(false);

                var assignmentReview = await _unitOfWork
                    .GetRepository<AssignmentReview>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id == id
                        && p.LessonId == lesson.Id
                        && p.UserId == currentUserId
                        && !p.IsDeleted
                    )
                    .ConfigureAwait(false);
                if (assignmentReview == null)
                {
                    throw new EntityNotFoundException(
                        _localizer.GetString("AssignmentReviewNotFound")
                    );
                }

                var watchHistory = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.LessonId == lesson.Id && p.UserId == currentUserId
                    )
                    .ConfigureAwait(false);
                if (watchHistory != null)
                {
                    _unitOfWork.GetRepository<WatchHistory>().Delete(watchHistory);
                }

                assignmentReview.IsDeleted = true;
                assignmentReview.UpdatedBy = currentUserId;
                assignmentReview.UpdatedOn = DateTime.Now;
                _unitOfWork.GetRepository<AssignmentReview>().Update(assignmentReview);

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to delete assignment review.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(
                        _localizer.GetString("ErrorOccurredDeleteAssignmentReview")
                    );
            }
        }

        /// <summary>
        /// reorder the assignemnt questions
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="lessonIdentity">lesson identity</param>
        /// <param name="ids">list of assignment question id</param>
        /// <returns>Task completed</returns>
        public async Task ReorderAssignmentQuestionAsync(
            Guid currentUserId,
            string lessonIdentity,
            IList<Guid> ids
        )
        {
            await ExecuteAsync(async () =>
            {
                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(
                        predicate: p =>
                            p.Id.ToString() == lessonIdentity
                            || p.Slug.ToLower().Trim() == lessonIdentity.ToString().Trim(),
                        include: src => src.Include(x => x.Assignments)
                    )
                    .ConfigureAwait(false);
                if (lesson == default)
                {
                    throw new EntityNotFoundException("LessonNotFound");
                }

                var hasAuthority = await IsSuperAdminOrAdminOrTrainerOfTraining(
                        currentUserId,
                        lesson.CourseId.ToString(),
                        TrainingTypeEnum.Course
                    )
                    .ConfigureAwait(false);
                if (!hasAuthority)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var assignmentQuestions = lesson.Assignments.ToList();
                if (assignmentQuestions.Count == default)
                {
                    throw new EntityNotFoundException(
                        _localizer.GetString("InvalidLessonAssignmentType")
                    );
                }

                var reorderedAssignment = new List<Assignment>();
                var order = 0;
                foreach (var id in ids)
                {
                    var assignmentQuestion = assignmentQuestions.FirstOrDefault(x => x.Id == id);
                    if (assignmentQuestion != default)
                    {
                        assignmentQuestion.Order = order;
                        assignmentQuestion.UpdatedBy = currentUserId;
                        assignmentQuestion.UpdatedOn = DateTime.UtcNow;
                        reorderedAssignment.Add(assignmentQuestion);
                        order++;
                    }
                }

                if (reorderedAssignment.Count != default)
                {
                    _unitOfWork.GetRepository<Assignment>().Update(reorderedAssignment);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                }
            });
        }

        #endregion Assignment Review

        #region Private Methods

        /// <summary>
        /// Handle to insert assignment submission
        /// </summary>
        /// <param name="currentUserId">the current logged in user</param>
        /// <param name="lessonId">the lesson id</param>
        /// <param name="currentTimeStamp">the current time stamp</param>
        /// <param name="item">the instance of <see cref="AssignmentSubmissionRequestModel"</param>
        /// <param name="assignment">the instance of <see cref="Assignment"/></param>
        /// <returns></returns>
        private async Task InsertSubmissionAsync(
            Guid currentUserId,
            Guid lessonId,
            DateTime currentTimeStamp,
            AssignmentSubmissionRequestModel item,
            Assignment assignment
        )
        {
            var assignmentSubmission = await _unitOfWork
                .GetRepository<AssignmentSubmission>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.Id == item.Id && p.UserId == currentUserId
                )
                .ConfigureAwait(false);
            if (assignmentSubmission == null)
            {
                assignmentSubmission = new AssignmentSubmission
                {
                    Id = Guid.NewGuid(),
                    LessonId = lessonId,
                    AssignmentId = assignment.Id,
                    UserId = currentUserId,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                };
            }
            else
            {
                assignmentSubmission.UpdatedOn = currentTimeStamp;
                assignmentSubmission.UpdatedBy = currentUserId;
            }

            if (
                assignment.Type == QuestionTypeEnum.SingleChoice
                || assignment.Type == QuestionTypeEnum.MultipleChoice
            )
            {
                var answerIds = assignment
                    .AssignmentQuestionOptions?.Where(x => x.IsCorrect)
                    .Select(x => x.Id);
                var isCorrect = answerIds
                    ?.OrderBy(x => x)
                    .ToList()
                    .SequenceEqual(item.SelectedOption.OrderBy(x => x).ToList());

                assignmentSubmission.IsCorrect = isCorrect ?? false;
                assignmentSubmission.SelectedOption = string.Join(",", item.SelectedOption);
            }

            if (assignment.Type == QuestionTypeEnum.Subjective)
            {
                assignmentSubmission.Answer = item.Answer;
                assignmentSubmission.AssignmentSubmissionAttachments =
                    new List<AssignmentSubmissionAttachment>();
                if (item.Id != default)
                {
                    var existingAssignmentSubmissionAttachments = await _unitOfWork
                        .GetRepository<AssignmentSubmissionAttachment>()
                        .GetAllAsync(predicate: p => p.AssignmentSubmissionId == item.Id)
                        .ConfigureAwait(false);

                    _unitOfWork
                        .GetRepository<AssignmentSubmissionAttachment>()
                        .Delete(existingAssignmentSubmissionAttachments);
                }

                item.AttachmentUrls?.ForEach(attachment =>
                    assignmentSubmission.AssignmentSubmissionAttachments.Add(
                        new AssignmentSubmissionAttachment
                        {
                            Id = Guid.NewGuid(),
                            AssignmentSubmissionId = assignmentSubmission.Id,
                            FileUrl = attachment,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp,
                        }
                    )
                );

                await _unitOfWork
                    .GetRepository<AssignmentSubmissionAttachment>()
                    .InsertAsync(assignmentSubmission.AssignmentSubmissionAttachments)
                    .ConfigureAwait(false);
            }

            await _unitOfWork
                .GetRepository<AssignmentSubmission>()
                .InsertAsync(assignmentSubmission)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to update assignment submission
        /// </summary>
        /// <param name="currentUserId">the current logged in user</param>
        /// <param name="currentTimeStamp">the current time stamp</param>
        /// <param name="item">the instance of <see cref="AssignmentSubmissionRequestModel"/></param>
        /// <param name="assignment">the instance of <see cref="Assignment"/></param>
        /// <returns></returns>
        private async Task UpdateSubmissionAsync(
            Guid currentUserId,
            DateTime currentTimeStamp,
            AssignmentSubmissionRequestModel item,
            Assignment assignment
        )
        {
            var assignmentSubmission = await _unitOfWork
                .GetRepository<AssignmentSubmission>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.Id == item.Id && p.UserId == currentUserId
                )
                .ConfigureAwait(false);

            assignmentSubmission.UpdatedOn = currentTimeStamp;
            assignmentSubmission.UpdatedBy = currentUserId;
            if (
                assignment.Type == QuestionTypeEnum.SingleChoice
                || assignment.Type == QuestionTypeEnum.MultipleChoice
            )
            {
                var answerIds = assignment
                    .AssignmentQuestionOptions?.Where(x => x.IsCorrect)
                    .Select(x => x.Id);
                var isCorrect = answerIds
                    ?.OrderBy(x => x)
                    .ToList()
                    .SequenceEqual(item.SelectedOption.OrderBy(x => x).ToList());

                assignmentSubmission.IsCorrect = isCorrect ?? false;
                assignmentSubmission.SelectedOption = string.Join(",", item.SelectedOption);
            }

            if (assignment.Type == QuestionTypeEnum.Subjective)
            {
                assignmentSubmission.Answer = item.Answer;
                assignmentSubmission.AssignmentSubmissionAttachments =
                    new List<AssignmentSubmissionAttachment>();

                if (item.Id != default)
                {
                    var existingAssignmentSubmissionAttachments = await _unitOfWork
                        .GetRepository<AssignmentSubmissionAttachment>()
                        .GetAllAsync(predicate: p => p.AssignmentSubmissionId == item.Id)
                        .ConfigureAwait(false);

                    _unitOfWork
                        .GetRepository<AssignmentSubmissionAttachment>()
                        .Delete(existingAssignmentSubmissionAttachments);
                }

                item.AttachmentUrls?.ForEach(attachment =>
                    assignmentSubmission.AssignmentSubmissionAttachments.Add(
                        new AssignmentSubmissionAttachment
                        {
                            Id = Guid.NewGuid(),
                            AssignmentSubmissionId = assignmentSubmission.Id,
                            FileUrl = attachment,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp,
                        }
                    )
                );

                await _unitOfWork
                    .GetRepository<AssignmentSubmissionAttachment>()
                    .InsertAsync(assignmentSubmission.AssignmentSubmissionAttachments)
                    .ConfigureAwait(false);
            }

            _unitOfWork.GetRepository<AssignmentSubmission>().Update(assignmentSubmission);
        }

        /// <summary>
        /// Handle to map assignment
        /// </summary>
        /// <param name="showCorrectAndHints">the boolean value</param>
        /// <param name="userAssignments">the list of <see cref="AssignmentSubmission"/></param>
        /// <param name="item">the instance of <see cref="Assignment"/></param>
        /// <param name="response">the list of <see cref="AssignmentResponseModel"/></param>
        private static void MapAssignment(
            bool showCorrectAndHints,
            IList<AssignmentSubmission> userAssignments,
            Assignment item,
            IList<AssignmentResponseModel> response
        )
        {
            var userAssignment = userAssignments.FirstOrDefault(x => x.AssignmentId == item.Id);
            var data = new AssignmentResponseModel
            {
                Id = item.Id,
                LessonId = item.LessonId,
                Name = item.Name,
                Description = item.Description,
                Order = item.Order,
                Hints = showCorrectAndHints ? item.Hints : null,
                Type = item.Type,
                IsActive = item.IsActive,
                User = item.User == null ? new UserModel() : new UserModel(item.User),
                Student = userAssignment == null ? null : new UserModel(userAssignment.User),
                AssignmentSubmissionId = userAssignment?.Id,
                Answer = userAssignment?.Answer,
                IsTrainee = !showCorrectAndHints,
                AssignmentAttachments = new List<AssignmentAttachmentResponseModel>(),
                AssignmentQuestionOptions = new List<AssignmentQuestionOptionResponseModel>(),
                AssignmentSubmissionAttachments =
                    new List<AssignmentSubmissionAttachmentResponseModel>(),
            };
            if (item.Type == QuestionTypeEnum.Subjective)
            {
                item.AssignmentAttachments?.ToList()
                    .ForEach(x =>
                        data.AssignmentAttachments.Add(new AssignmentAttachmentResponseModel(x))
                    );
            }

            if (
                item.Type == QuestionTypeEnum.SingleChoice
                || item.Type == QuestionTypeEnum.MultipleChoice
            )
            {
                var selectedAnsIds = !string.IsNullOrWhiteSpace(userAssignment?.SelectedOption)
                    ? userAssignment?.SelectedOption.Split(",").Select(Guid.Parse).ToList()
                    : new List<Guid>();
                item.AssignmentQuestionOptions?.OrderBy(x => x.Order)
                    .ToList()
                    .ForEach(x =>
                        data.AssignmentQuestionOptions.Add(
                            new AssignmentQuestionOptionResponseModel()
                            {
                                Id = x.Id,
                                AssignmentId = x.AssignmentId,
                                AssignmentName = x.Assignment?.Name,
                                Option = x.Option,
                                IsCorrect = showCorrectAndHints ? x.IsCorrect : null,
                                IsSelected =
                                    userAssignment != null ? selectedAnsIds?.Contains(x.Id) : null,
                                Order = x.Order,
                            }
                        )
                    );
            }

            if (userAssignment?.AssignmentSubmissionAttachments.Count > 0)
            {
                userAssignment.AssignmentSubmissionAttachments.ForEach(x =>
                {
                    _ = new AssignmentSubmissionAttachmentResponseModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        MimeType = x.MimeType,
                        AssignmentSubmissionId = x.AssignmentSubmissionId,
                        FileUrl = x.FileUrl,
                    };
                });
            }

            response.Add(data);
        }

        public async Task<IList<AssignmentResultExportModel>> GetResultsExportAsync(
            string lessonIdentity,
            Guid currentUserId
        )
        {
            await IsSuperAdminOrAdminOrTrainer(currentUserId);

            var lesson = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(predicate: p => p.Slug == lessonIdentity)
                .ConfigureAwait(false);

            var Students = await _unitOfWork
                .GetRepository<AssignmentReview>()
                .GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id,
                    include: u => u.Include(x => x.User)
                )
                .ConfigureAwait(false);

            var submissionDate = await _unitOfWork
                .GetRepository<AssignmentSubmission>()
                .GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id,
                    include: u => u.Include(x => x.User)
                )
                .ConfigureAwait(false);

            var uniqueDate = submissionDate
                .GroupBy(item => item.UserId)
                .Select(group => group.First());

            var studentDetail =
                from std in Students
                join date in uniqueDate
                    on std.UserId equals date.UserId
                    into studentSubmissionDetail
                from m in studentSubmissionDetail
                select new AssignmentResultExportModel
                {
                    StudentName = std.User.FullName,
                    TotalMarks = std.Mark,
                    SubmissionDate = m.UpdatedOn
                };

            return studentDetail.ToList();
        }

        public async Task<IList<AssignmentIndividualExportModel>> GetIndividualResultsExportAsync(
            string lessonIdentity,
            Guid currentUserId
        )
        {
            await IsSuperAdminOrAdminOrTrainer(currentUserId);

            var lesson = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(predicate: p => p.Slug == lessonIdentity)
                .ConfigureAwait(false);

            var IndividualStudents = await _unitOfWork
                .GetRepository<WatchHistory>()
                .GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id,
                    include: u => u.Include(u => u.User)
                )
                .ConfigureAwait(false);

            var studentList = new List<AssignmentIndividualExportModel>();
            foreach (var std in IndividualStudents)
            {
                studentList.Add(
                    new AssignmentIndividualExportModel
                    {
                        StudentName = std.User.FullName,
                        Status = std.IsPassed ? "Pass" : "Fail"
                    }
                );
            }

            return studentList;
        }

        #endregion Private Methods
    }
}
