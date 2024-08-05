﻿namespace AcademyKit.Infrastructure.Services
{
    using System;
    using System.Linq.Expressions;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Helpers;
    using AcademyKit.Infrastructure.Localization;
    using AngleSharp.Common;
    using Hangfire;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class LessonService
        : BaseGenericService<Lesson, LessonBaseSearchCriteria>,
            ILessonService
    {
        private readonly IZoomLicenseService _zoomLicenseService;
        private readonly IZoomSettingService _zoomSettingService;
        private readonly IFileServerService _fileServerService;

        public LessonService(
            IUnitOfWork unitOfWork,
            ILogger<LessonService> logger,
            IZoomLicenseService zoomLicenseService,
            IFileServerService fileServerService,
            IZoomSettingService zoomSettingService,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer)
        {
            _zoomLicenseService = zoomLicenseService;
            _zoomSettingService = zoomSettingService;
            _fileServerService = fileServerService;
        }

        #region Protected Region

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Lesson, bool>> ConstructQueryConditions(
            Expression<Func<Lesson, bool>> predicate,
            LessonBaseSearchCriteria criteria
        )
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(
                criteria.CourseIdentity,
                nameof(criteria.CourseIdentity)
            );
            CommonHelper.ValidateArgumentNotNullOrEmpty(
                criteria.SectionIdentity,
                nameof(criteria.SectionIdentity)
            );
            var course = ValidateAndGetCourse(
                criteria.CurrentUserId,
                criteria.CourseIdentity,
                validateForModify: false
            ).Result;
            var section = _unitOfWork
                .GetRepository<Section>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.CourseId == course.Id
                    && (
                        p.Id.ToString() == criteria.SectionIdentity
                        || p.Slug == criteria.SectionIdentity
                    )
                )
                .Result;

            return predicate.And(p =>
                p.CourseId == course.Id && p.SectionId == section.Id && !p.IsDeleted
            );
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Lesson, object> IncludeNavigationProperties(
            IQueryable<Lesson> query
        )
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Lesson, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }

        #endregion Protected Region

        /// <summary>
        /// Handle to get lesson detail
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns>the instance of <see cref="LessonResponseModel"/></returns>
        public async Task<LessonResponseModel> GetLessonAsync(
            string identity,
            string lessonIdentity,
            Guid currentUserId
        )
        {
            var course = await ValidateAndGetCourse(
                    currentUserId,
                    identity,
                    validateForModify: false
                )
                .ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning(
                    "Training with identity: {identity} not found for user with :{id}.",
                    identity.SanitizeForLogger(),
                    currentUserId
                );
                throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            }

            var lesson = new Lesson();
            if (!string.IsNullOrWhiteSpace(lessonIdentity))
            {
                var requestedLesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(
                        predicate: p =>
                            p.CourseId == course.Id
                            && (p.Id.ToString() == lessonIdentity || p.Slug.Equals(lessonIdentity)),
                        include: src =>
                            src.Include(x => x.User).Include(x => x.Course).Include(x => x.Section)
                    )
                    .ConfigureAwait(false);

                if (requestedLesson == null)
                {
                    _logger.LogWarning(
                        "Lesson with identity : {identity} and training with id: {courseId} not found for user with :{id}.",
                        identity.SanitizeForLogger(),
                        course.Id,
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                lesson = requestedLesson;
            }
            else
            {
                lesson = await GetCurrentLesson(currentUserId, course).ConfigureAwait(false);
            }

            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId)
                .ConfigureAwait(false);
            var isTeacher = course.CourseTeachers.Any(x => x.UserId == currentUserId);

            var userCompletedWatchHistories = await _unitOfWork
                .GetRepository<WatchHistory>()
                .GetAllAsync(predicate: p =>
                    p.UserId == currentUserId && p.CourseId == course.Id && p.IsPassed
                )
                .ConfigureAwait(false);

            var sections = await _unitOfWork
                .GetRepository<Section>()
                .GetAllAsync(
                    predicate: p => p.CourseId == course.Id,
                    orderBy: o => o.OrderBy(x => x.Order),
                    include: src => src.Include(x => x.Lessons)
                )
                .ConfigureAwait(false);

            var lessons = new List<Lesson>();
            lessons = sections.SelectMany(x => x.Lessons.OrderBy(x => x.Order)).ToList();

            var currentIndex = lessons.FindIndex(x => x.Id == lesson.Id);
            var orderLessons = lessons.GetRange(0, currentIndex).Where(x => x.IsMandatory);

            var containMandatoryLesson = orderLessons
                .Select(x => x.Id)
                .Except(userCompletedWatchHistories.Select(x => x.LessonId));
            if (!isTeacher && !isSuperAdminOrAdmin && containMandatoryLesson.Any())
            {
                _logger.LogWarning(
                    "User with id: {userId} needs to view other mandatory lesson before viewing current lesson with id: {lessonId}",
                    currentUserId,
                    lesson.Id
                );
                throw new ForbiddenException(_localizer.GetString("CompleteMandatoryLesson"));
            }

            if (lesson.Type == LessonType.LiveClass)
            {
                lesson.Meeting = new Meeting();
                lesson.Meeting = await _unitOfWork
                    .GetRepository<Meeting>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Id == lesson.MeetingId)
                    .ConfigureAwait(false);
            }

            bool? hasResult = null;
            int? remainingAttempt = null;
            if (lesson.Type == LessonType.Exam)
            {
                lesson.QuestionSet = new QuestionSet();
                lesson.QuestionSet = await _unitOfWork
                    .GetRepository<QuestionSet>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id == lesson.QuestionSetId,
                        include: x => x.Include(p => p.QuestionSetQuestions)
                    )
                    .ConfigureAwait(false);
                var containResults = await _unitOfWork
                    .GetRepository<QuestionSetResult>()
                    .ExistsAsync(predicate: p =>
                        p.UserId == currentUserId && p.QuestionSetId == lesson.QuestionSetId
                    )
                    .ConfigureAwait(false);

                var submissionCount = await _unitOfWork
                    .GetRepository<QuestionSetSubmission>()
                    .CountAsync(predicate: p =>
                        p.QuestionSetId == lesson.QuestionSetId
                        && p.UserId == currentUserId
                        && p.StartTime != default
                        && p.EndTime != default
                    )
                    .ConfigureAwait(false);

                remainingAttempt =
                    lesson.QuestionSet.AllowedRetake > 0
                        ? lesson.QuestionSet.AllowedRetake - submissionCount
                        : submissionCount > 0
                            ? 0
                            : 1;

                hasResult = containResults;
            }

            bool? hasSubmitAssignment = null;
            bool? hasReviewedAssignment = null;
            AssignmentReviewResponseModel review = null;

            if (lesson.Type == LessonType.Assignment)
            {
                lesson.Assignments = new List<Assignment>();
                lesson.Assignments = await _unitOfWork
                    .GetRepository<Assignment>()
                    .GetAllAsync(predicate: p => p.LessonId == lesson.Id)
                    .ConfigureAwait(false);
                var assignmentSubmission = await _unitOfWork
                    .GetRepository<AssignmentSubmission>()
                    .ExistsAsync(predicate: p =>
                        p.UserId == currentUserId && p.LessonId == lesson.Id
                    )
                    .ConfigureAwait(false);
                if (assignmentSubmission)
                {
                    hasSubmitAssignment = true;
                }

                var assignmentReview = await _unitOfWork
                    .GetRepository<AssignmentReview>()
                    .GetFirstOrDefaultAsync(
                        predicate: p =>
                            p.LessonId == lesson.Id && p.UserId == currentUserId && !p.IsDeleted,
                        include: src => src.Include(x => x.User)
                    )
                    .ConfigureAwait(false);

                if (assignmentReview != null)
                {
                    hasReviewedAssignment = true;
                    var teacher = await _unitOfWork
                        .GetRepository<User>()
                        .GetFirstOrDefaultAsync(predicate: p => p.Id == assignmentReview.CreatedBy)
                        .ConfigureAwait(false);
                    review = new AssignmentReviewResponseModel()
                    {
                        Id = assignmentReview.Id,
                        UserId = assignmentReview.UserId,
                        LessonId = assignmentReview.LessonId,
                        Mark = assignmentReview.Mark,
                        Review = assignmentReview.Review,
                        User = new UserModel(assignmentReview.User),
                        Teacher = new UserModel(teacher)
                    };
                }
            }

            bool? hasFeedbackSubmitted = null;

            if (lesson.Type == LessonType.Feedback)
            {
                var feedbackIds = await _unitOfWork
                    .GetRepository<Feedback>()
                    .GetAllAsync(
                        selector: x => x.Id,
                        predicate: p => p.LessonId == lesson.Id && p.IsActive
                    )
                    .ConfigureAwait(false);

                var isFeedbackSubmissionExists = await _unitOfWork
                    .GetRepository<FeedbackSubmission>()
                    .ExistsAsync(predicate: p =>
                        feedbackIds.Contains(p.FeedbackId) && p.UserId == currentUserId
                    )
                    .ConfigureAwait(false);

                if (isFeedbackSubmissionExists)
                {
                    hasFeedbackSubmitted = true;
                }
            }

            var currentLessonWatchHistory = await _unitOfWork
                .GetRepository<WatchHistory>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.LessonId == lesson.Id && p.UserId == currentUserId
                )
                .ConfigureAwait(false);
            var responseModel = new LessonResponseModel(lesson);
            if (!string.IsNullOrEmpty(responseModel.VideoUrl))
            {
                responseModel.VideoUrl = await _fileServerService
                    .GetFilePresignedUrl(responseModel.VideoUrl)
                    .ConfigureAwait(false);
            }

            responseModel.IsCompleted =
                currentLessonWatchHistory != null ? currentLessonWatchHistory.IsCompleted : false;
            responseModel.IsPassed =
                currentLessonWatchHistory != null ? currentLessonWatchHistory.IsPassed : false;

            if (lesson.Type == LessonType.Document && !string.IsNullOrEmpty(lesson.DocumentUrl))
            {
                responseModel.DocumentUrl = await _fileServerService
                    .GetFilePresignedUrl(lesson.DocumentUrl)
                    .ConfigureAwait(false);
            }

            bool? HasAttended = null;
            if (lesson.Type == LessonType.Physical)
            {
                HasAttended = await _unitOfWork
                    .GetRepository<PhysicalLessonReview>()
                    .ExistsAsync(predicate: p =>
                        p.UserId == currentUserId && p.LessonId == lesson.Id
                    )
                    .ConfigureAwait(false);
            }

            var nextLessonIndex = currentIndex + 1;
            if ((nextLessonIndex + 1) <= lessons.Count)
            {
                responseModel.NextLessonSlug = lessons.GetItemByIndex(nextLessonIndex)?.Slug;
            }

            if (lesson.Type == LessonType.Assignment && lesson.EndDate.HasValue)
            {
                responseModel.AssignmentExpired = lesson.EndDate <= DateTime.UtcNow;
            }

            responseModel.IsTrainee = !await IsSuperAdminOrAdminOrTrainerOfTraining(
                    currentUserId,
                    lesson.CourseId.ToString(),
                    TrainingTypeEnum.Course
                )
                .ConfigureAwait(false);
            responseModel.HasAttended = HasAttended;
            responseModel.HasSubmittedAssignment = hasSubmitAssignment;
            responseModel.HasResult = hasResult;
            responseModel.HasReviewedAssignment = hasReviewedAssignment;
            responseModel.AssignmentReview = review;
            responseModel.RemainingAttempt = remainingAttempt;
            responseModel.HasFeedbackSubmitted = hasFeedbackSubmitted;
            return responseModel;
        }

        /// <summary>
        /// Handle to create lesson
        /// </summary>
        /// <param name="courseIdentity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></see></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<Lesson> AddAsync(
            string courseIdentity,
            LessonRequestModel model,
            Guid currentUserId
        )
        {
            try
            {
                var course = await ValidateAndGetCourse(
                        currentUserId,
                        courseIdentity,
                        validateForModify: true
                    )
                    .ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning(
                        "Training with identity: {identity} not found for user with :{id}.",
                        courseIdentity.SanitizeForLogger(),
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                var section = await _unitOfWork
                    .GetRepository<Section>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.CourseId == course.Id
                        && (
                            p.Id.ToString() == model.SectionIdentity
                            || p.Slug == model.SectionIdentity
                        )
                    )
                    .ConfigureAwait(false);
                if (section == null)
                {
                    _logger.LogWarning(
                        "Section with identity: {identity} not found for user with id:{id} and training with id: {courseId}.",
                        courseIdentity.SanitizeForLogger(),
                        currentUserId,
                        course.Id
                    );
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                var currentTimeStamp = DateTime.UtcNow;
                var lesson = new Lesson
                {
                    Id = Guid.NewGuid(),
                    CourseId = course.Id,
                    Name = model.Name,
                    Description = model.Description,
                    ThumbnailUrl = model.ThumbnailUrl,
                    Type = model.Type,
                    IsMandatory = model.IsMandatory,
                    SectionId = section.Id,
                    Status = CourseStatus.Draft,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                };

                if (lesson.Type == LessonType.Exam)
                {
                    lesson.Name = model.QuestionSet.Name;
                }

                lesson.Slug = CommonHelper.GetEntityTitleSlug<Lesson>(
                    _unitOfWork,
                    (slug) => q => q.Slug == slug,
                    lesson.Name
                );
                if (lesson.Type == LessonType.Document)
                {
                    lesson.DocumentUrl = model.DocumentUrl;
                }

                if (lesson.Type == LessonType.LiveClass)
                {
                    lesson.Duration = model.Meeting.MeetingDuration * 60; //convert duration from minutes to seconds;
                    await CreateMeetingAsync(model, lesson).ConfigureAwait(false);
                }

                if (lesson.Type == LessonType.Exam)
                {
                    lesson.Duration = model.QuestionSet.Duration * 60; //convert duration from minutes to seconds;
                    await CreateQuestionSetAsync(model, lesson).ConfigureAwait(false);
                }

                if (lesson.Type == LessonType.Video)
                {
                    lesson.VideoUrl = model.VideoUrl;
                    var videoQueue = new VideoQueue
                    {
                        Id = Guid.NewGuid(),
                        LessonId = lesson.Id,
                        VideoUrl = lesson.VideoUrl,
                        Status = VideoStatus.Queue,
                        CreatedOn = DateTime.UtcNow
                    };
                    await _unitOfWork
                        .GetRepository<VideoQueue>()
                        .InsertAsync(videoQueue)
                        .ConfigureAwait(false);
                    BackgroundJob.Enqueue<IHangfireJobService>(job =>
                        job.LessonVideoUploadedAsync(lesson.Id, null)
                    );
                }

                var courseEnrollments = await _unitOfWork
                    .GetRepository<CourseEnrollment>()
                    .GetAllAsync(predicate: p => p.CourseId == course.Id)
                    .ConfigureAwait(false);
                if (courseEnrollments != null)
                {
                    var lessonCount = await _unitOfWork
                        .GetRepository<Lesson>()
                        .CountAsync(predicate: p => p.CourseId == course.Id)
                        .ConfigureAwait(false);
                    var UpdateCourseEnrollments = new List<CourseEnrollment>();
                    foreach (var courseEnrollment in courseEnrollments)
                    {
                        courseEnrollment.Percentage =
                            (courseEnrollment.Percentage * lessonCount) / (lessonCount + 1);
                        UpdateCourseEnrollments.Add(courseEnrollment);
                    }

                    _unitOfWork.GetRepository<CourseEnrollment>().Update(UpdateCourseEnrollments);
                }

                var order = await LastLessonOrder(lesson).ConfigureAwait(false);
                lesson.Order = order;
                await _unitOfWork.GetRepository<Lesson>().InsertAsync(lesson).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                return lesson;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to create the lesson");
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to update lesson
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<Lesson> UpdateAsync(
            string identity,
            string lessonIdentity,
            LessonRequestModel model,
            Guid currentUserId
        )
        {
            try
            {
                var course = await ValidateAndGetCourse(
                        currentUserId,
                        identity,
                        validateForModify: true
                    )
                    .ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning(
                        "Training with identity: {identity} not found for user with :{id}.",
                        identity.SanitizeForLogger(),
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                if (course.Status == CourseStatus.Completed)
                {
                    throw new InvalidOperationException(
                        _localizer.GetString("CompletedCourseIssue")
                    );
                }

                var section = await _unitOfWork
                    .GetRepository<Section>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.CourseId == course.Id
                        && (
                            p.Id.ToString() == model.SectionIdentity
                            || p.Slug == model.SectionIdentity
                        )
                    )
                    .ConfigureAwait(false);
                if (section == null)
                {
                    _logger.LogWarning(
                        "Section with identity: {identity} not found for user with id:{id} and training with id: {courseId}.",
                        identity.SanitizeForLogger(),
                        currentUserId,
                        course.Id
                    );
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                var existing = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.CourseId == course.Id
                        && p.SectionId == section.Id
                        && (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity)
                    )
                    .ConfigureAwait(false);
                if (existing == null)
                {
                    _logger.LogWarning(
                        "Lesson with identity: {identity} not found for user with id: {id} and training with id: {courseId} and section with id: {sectionId}.",
                        lessonIdentity.SanitizeForLogger(),
                        currentUserId,
                        course.Id,
                        section.Id
                    );
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (model.Type != existing.Type)
                {
                    _logger.LogWarning(
                        "Lesson type not matched for lesson with id: {id}.",
                        existing.Id
                    );
                    throw new ForbiddenException(_localizer.GetString("LessonTypeNotMatched"));
                }

                var currentTimeStamp = DateTime.UtcNow;

                var existingThumbnailUrlKey = existing.ThumbnailUrl;
                var existingDocumentUrlKey = existing.DocumentUrl;
                var existingVideoUrl = existing.VideoUrl;

                existing.Name = model.Name;
                existing.Description = model.Description;
                existing.ThumbnailUrl = model.ThumbnailUrl;
                existing.IsMandatory = model.IsMandatory;
                existing.StartDate = model.StartDate;
                existing.EndDate = model.EndDate;
                existing.UpdatedBy = currentUserId;
                existing.UpdatedOn = currentTimeStamp;

                if (existing.Type == LessonType.Exam)
                {
                    existing.Name = model.QuestionSet.Name;
                }

                if (existing.Type == LessonType.Document)
                {
                    existing.DocumentUrl = model.DocumentUrl;
                }

                if (existing.Type == LessonType.Video)
                {
                    existing.VideoUrl = model.VideoUrl;
                }

                if (existing.Type == LessonType.LiveClass)
                {
                    existing.Meeting = new Meeting();
                    existing.Meeting = await _unitOfWork
                        .GetRepository<Meeting>()
                        .GetFirstOrDefaultAsync(predicate: p => p.Id == existing.MeetingId)
                        .ConfigureAwait(false);
                    existing.Duration = model.Meeting.MeetingDuration * 60; //convert duration from minutes to seconds;
                    await UpdateMeetingAsync(model, existing).ConfigureAwait(false);
                }

                if (existing.Type == LessonType.Exam)
                {
                    existing.QuestionSet = new QuestionSet();
                    existing.QuestionSet = await _unitOfWork
                        .GetRepository<QuestionSet>()
                        .GetFirstOrDefaultAsync(predicate: p => p.Id == existing.QuestionSetId)
                        .ConfigureAwait(false);
                    existing.Duration = model.QuestionSet.Duration * 60; //convert duration from minutes to seconds;
                    await UpdateQuestionSetAsync(model, existing).ConfigureAwait(false);
                }

                _unitOfWork.GetRepository<Lesson>().Update(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                // Delete file from server if file are changed

                if (
                    existingThumbnailUrlKey != model.ThumbnailUrl
                    && !string.IsNullOrWhiteSpace(existingThumbnailUrlKey)
                )
                {
                    if (
                        existingThumbnailUrlKey.ToLower().Trim().Contains("/public/")
                        && existingThumbnailUrlKey.IndexOf("/standalone/") != -1
                    )
                    {
                        existingThumbnailUrlKey = existingThumbnailUrlKey.Substring(
                            existingThumbnailUrlKey.IndexOf("/standalone/") + "/standalone/".Length
                        );
                    }

                    await _fileServerService
                        .RemoveFileAsync(existingThumbnailUrlKey)
                        .ConfigureAwait(false);
                }

                if (
                    existingDocumentUrlKey != model.DocumentUrl
                    && !string.IsNullOrWhiteSpace(existingDocumentUrlKey)
                )
                {
                    await _fileServerService
                        .RemoveFileAsync(existingDocumentUrlKey)
                        .ConfigureAwait(false);
                }

                if (
                    existingVideoUrl != model.VideoUrl
                    && !string.IsNullOrWhiteSpace(existingVideoUrl)
                )
                {
                    await _fileServerService
                        .RemoveFileAsync(existingVideoUrl)
                        .ConfigureAwait(false);
                }

                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while attempting to update the lesson information"
                );
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to delete lesson
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task DeleteLessonAsync(
            string identity,
            string lessonIdentity,
            Guid currentUserId
        )
        {
            await ExecuteAsync(async () =>
                {
                    var course = await ValidateAndGetCourse(
                            currentUserId,
                            identity,
                            validateForModify: true
                        )
                        .ConfigureAwait(false);
                    if (course == null)
                    {
                        _logger.LogWarning(
                            "DeleteLessonAsync(): Training with identity : {identity} not found for user with id :{currentUserId}.",
                            identity.SanitizeForLogger(),
                            currentUserId
                        );
                        throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                    }

                    var lesson = await _unitOfWork
                        .GetRepository<Lesson>()
                        .GetFirstOrDefaultAsync(predicate: p =>
                            (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity)
                            && p.CourseId == course.Id
                        )
                        .ConfigureAwait(false);
                    if (lesson == null)
                    {
                        _logger.LogWarning(
                            "DeleteLessonAsync(): Lesson with identity : {lessonIdentity} was not found for user with id : {userId} and having training with id : {courseId}.",
                            lessonIdentity.SanitizeForLogger(),
                            currentUserId,
                            course.Id
                        );
                        throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                    }

                    if (course.Status == CourseStatus.Completed)
                    {
                        throw new InvalidOperationException(
                            _localizer.GetString("CompletedCourseIssue")
                        );
                    }

                    if (lesson.Type == LessonType.RecordedVideo)
                    {
                        _logger.LogWarning(
                            "DeleteLessonAsync(): Lesson with id : {lessonId} has type : {type}.",
                            lesson.Id,
                            lesson.Type
                        );
                        throw new ForbiddenException(
                            _localizer.GetString("LessonTypeRecordedCannotDeleted")
                        );
                    }

                    if (course.CourseEnrollments.Any(x => x.CurrentLessonId == lesson.Id))
                    {
                        _logger.LogWarning(
                            "DeleteLessonAsync(): Course with courseID:{CourseId} contains enrolled user so it cannot be deleted.",
                            course.Id
                        );
                        throw new ForbiddenException(_localizer.GetString("ContainsUsers"));
                    }

                    if (lesson.Type == LessonType.Exam)
                    {
                        var questionSet = await _unitOfWork
                            .GetRepository<QuestionSet>()
                            .GetFirstOrDefaultAsync(
                                predicate: p => p.Id == lesson.QuestionSetId,
                                include: src => src.Include(x => x.QuestionSetQuestions)
                            )
                            .ConfigureAwait(false);
                        if (questionSet == null)
                        {
                            _logger.LogWarning(
                                "DeleteLessonAsync(): Lesson with id:{lessonId} and type: {lessonType} does not contain question set with id : {questionSetId}.",
                                lesson.Id,
                                lesson.Type,
                                lesson.QuestionSetId
                            );
                            throw new EntityNotFoundException(
                                _localizer.GetString("QuestionSetNotFound")
                            );
                        }

                        var hasAnyAttempt = await _unitOfWork
                            .GetRepository<QuestionSetSubmission>()
                            .ExistsAsync(predicate: p => p.QuestionSetId == lesson.QuestionSetId)
                            .ConfigureAwait(false);
                        if (hasAnyAttempt)
                        {
                            _logger.LogWarning(
                                "DeleteLessonAsync(): Lesson with id: {lessonId} and question set with id: {questionSetId} having type: {type} contains exam submission.",
                                lesson.Id,
                                lesson.QuestionSetId,
                                lesson.Type
                            );
                            throw new ForbiddenException(
                                _localizer.GetString("LessonWithType")
                                    + " "
                                    + lesson.Type
                                    + " "
                                    + _localizer.GetString("ExamCannotBeDeleted")
                            );
                        }

                        _unitOfWork
                            .GetRepository<QuestionSetQuestion>()
                            .Delete(questionSet.QuestionSetQuestions);
                        _unitOfWork.GetRepository<QuestionSet>().Delete(questionSet);
                    }

                    if (lesson.Type == LessonType.LiveClass)
                    {
                        var meeting = await _unitOfWork
                            .GetRepository<Meeting>()
                            .GetFirstOrDefaultAsync(predicate: p => p.Id == lesson.MeetingId)
                            .ConfigureAwait(false);
                        if (meeting == null)
                        {
                            _logger.LogWarning(
                                "DeleteLessonAsync(): Lesson with id:{lessonId} and type: {type} does not contain meeting with id : {meetingId}.",
                                lesson.Id,
                                lesson.Type,
                                lesson.MeetingId
                            );
                            throw new EntityNotFoundException(
                                _localizer.GetString("MeetingNotFound")
                            );
                        }
                        await _zoomLicenseService
                            .DeleteZoomMeeting(meeting.MeetingNumber.ToString())
                            .ConfigureAwait(false);

                        _unitOfWork.GetRepository<Meeting>().Delete(meeting);
                    }

                    if (lesson.Type == LessonType.Assignment)
                    {
                        var assignments = await _unitOfWork
                            .GetRepository<Assignment>()
                            .GetAllAsync(predicate: p => p.LessonId == lesson.Id)
                            .ConfigureAwait(false);
                        var assignmentIds = assignments.Select(x => x.Id).ToList();

                        var assignmentSubmissions = await _unitOfWork
                            .GetRepository<AssignmentSubmission>()
                            .GetAllAsync(predicate: p => assignmentIds.Contains(p.AssignmentId))
                            .ConfigureAwait(false);
                        if (assignmentSubmissions.Count > 0)
                        {
                            _logger.LogWarning(
                                "DeleteLessonAsync(): Lesson with id:{lessonId} and type: {type} contains assignmentSubmissions.",
                                lesson.Id,
                                lesson.Type
                            );
                            throw new EntityNotFoundException(
                                _localizer.GetString("AssignmentContainsSubmission")
                                    + " "
                                    + lesson.Type
                                    + "."
                            );
                        }

                        var assignmentAttachments = await _unitOfWork
                            .GetRepository<AssignmentAttachment>()
                            .GetAllAsync(predicate: p => assignmentIds.Contains(p.AssignmentId))
                            .ConfigureAwait(false);

                        var assignmentOptions = await _unitOfWork
                            .GetRepository<AssignmentQuestionOption>()
                            .GetAllAsync(predicate: p => assignmentIds.Contains(p.AssignmentId))
                            .ConfigureAwait(false);

                        _unitOfWork
                            .GetRepository<AssignmentQuestionOption>()
                            .Delete(assignmentOptions);
                        _unitOfWork
                            .GetRepository<AssignmentAttachment>()
                            .Delete(assignmentAttachments);
                        _unitOfWork.GetRepository<Assignment>().Delete(assignments);
                    }

                    if (lesson.Type == LessonType.Feedback)
                    {
                        var feedbacks = await _unitOfWork
                            .GetRepository<Feedback>()
                            .GetAllAsync(predicate: p => p.LessonId == lesson.Id)
                            .ConfigureAwait(false);
                        var feedbackIds = feedbacks.Select(x => x.Id).ToList();

                        var feedbackSubmissions = await _unitOfWork
                            .GetRepository<FeedbackSubmission>()
                            .GetAllAsync(predicate: p => feedbackIds.Contains(p.FeedbackId))
                            .ConfigureAwait(false);
                        if (feedbackSubmissions.Count > 0)
                        {
                            _logger.LogWarning(
                                "DeleteLessonAsync(): Lesson with id:{lessonId} and type: {type} contains feedbackSubmissions.",
                                lesson.Id,
                                lesson.Type
                            );
                            throw new EntityNotFoundException(
                                _localizer.GetString("FeedBackContainsSubmission")
                            );
                        }

                        var feedbackOptions = await _unitOfWork
                            .GetRepository<FeedbackQuestionOption>()
                            .GetAllAsync(predicate: p => feedbackIds.Contains(p.FeedbackId))
                            .ConfigureAwait(false);

                        _unitOfWork.GetRepository<FeedbackQuestionOption>().Delete(feedbackOptions);
                        _unitOfWork.GetRepository<Feedback>().Delete(feedbacks);
                    }

                    if (lesson.Type == LessonType.Physical)
                    {
                        var hasSubmission = await _unitOfWork
                            .GetRepository<PhysicalLessonReview>()
                            .ExistsAsync(predicate: p => p.LessonId == lesson.Id)
                            .ConfigureAwait(false);
                        if (hasSubmission)
                        {
                            throw new ForbiddenException(
                                _localizer.GetString("LessonContainsAttendance")
                            );
                        }
                    }

                    _unitOfWork.GetRepository<Lesson>().Delete(lesson);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(lesson.ThumbnailUrl))
                    {
                        if (
                            lesson.ThumbnailUrl.ToLower().Trim().Contains("/public/")
                            && lesson.ThumbnailUrl.IndexOf("/standalone/") != -1
                        )
                        {
                            lesson.ThumbnailUrl = lesson.ThumbnailUrl.Substring(
                                lesson.ThumbnailUrl.IndexOf("/standalone/") + "/standalone/".Length
                            );
                        }

                        await _fileServerService
                            .RemoveFileAsync(lesson.ThumbnailUrl)
                            .ConfigureAwait(false);
                    }

                    if (!string.IsNullOrWhiteSpace(lesson.DocumentUrl))
                    {
                        await _fileServerService
                            .RemoveFileAsync(lesson.DocumentUrl)
                            .ConfigureAwait(false);
                    }

                    if (!string.IsNullOrWhiteSpace(lesson.VideoUrl))
                    {
                        await _fileServerService
                            .RemoveFileAsync(lesson.VideoUrl)
                            .ConfigureAwait(false);
                    }
                })
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to join meeting
        /// </summary>
        /// <param name="identity">the course identity</param>
        /// <param name="lessonIdentity">the lesson identity</param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        public async Task<MeetingJoinResponseModel> GetJoinMeetingAsync(
            string identity,
            string lessonIdentity,
            Guid currentUserId
        )
        {
            try
            {
                var user = await _unitOfWork
                    .GetRepository<User>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.Id == currentUserId && p.Status == UserStatus.Active
                    )
                    .ConfigureAwait(false);
                if (user == null)
                {
                    _logger.LogWarning("User with id: {id} not found.", currentUserId);
                    throw new EntityNotFoundException(_localizer.GetString("UserNotFound"));
                }

                var course = await ValidateAndGetCourse(
                        currentUserId,
                        identity,
                        validateForModify: false
                    )
                    .ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning(
                        "Training with identity: {identity} not found for user with id :{id}.",
                        identity.SanitizeForLogger(),
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(
                        predicate: p =>
                            p.CourseId == course.Id
                            && (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity),
                        include: src => src.Include(x => x.User)
                    )
                    .ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning(
                        "Lesson with identity : {identity} and training with id: {courseId} not found for user with :{id}.",
                        identity.SanitizeForLogger(),
                        course.Id,
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }

                if (lesson.Type != LessonType.LiveClass)
                {
                    _logger.LogWarning(
                        "Lesson with id : {id} type not match for join meeting.",
                        lesson.Id
                    );
                    throw new ForbiddenException(_localizer.GetString("LessonTypeNotMatchMeeting"));
                }

                lesson.Meeting = await _unitOfWork
                    .GetRepository<Meeting>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id == lesson.MeetingId,
                        include: src => src.Include(x => x.ZoomLicense)
                    )
                    .ConfigureAwait(false);
                if (lesson.Meeting == null)
                {
                    _logger.LogWarning(
                        "Lesson with id : {id}  meeting not found for join meeting.",
                        lesson.Id
                    );
                    throw new EntityNotFoundException(_localizer.GetString("MeetingNotFound"));
                }

                if (lesson.Meeting.ZoomLicense == null)
                {
                    _logger.LogWarning(
                        "Zoom license with id : {id} not found.",
                        lesson.Meeting.ZoomLicenseId
                    );
                    throw new ServiceException(_localizer.GetString("ZoomLicenseNotFound"));
                }

                var isModerator = await IsLiveClassModerator(currentUserId, course)
                    .ConfigureAwait(false);
                if (!isModerator)
                {
                    var isMember = course.CourseEnrollments.Any(x =>
                        x.UserId == currentUserId
                        && !x.IsDeleted
                        && (
                            x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled
                            || x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled
                        )
                    );
                    if (!isMember)
                    {
                        _logger.LogWarning(
                            "User with id : {currentUserId} is invalid user to attend this meeting having lesson with id :{id}.",
                            currentUserId,
                            lesson.Id
                        );
                        throw new ForbiddenException(_localizer.GetString("MeetingNotAccessed"));
                    }
                }

                var zoomSetting = await _zoomSettingService
                    .GetFirstOrDefaultAsync()
                    .ConfigureAwait(false);
                if (zoomSetting == null)
                {
                    _logger.LogWarning(
                        "GetJoinMeetingAsync(): Zoom setting not found for user with id: {id}.",
                        currentUserId
                    );
                    throw new ServiceException(_localizer.GetString("ZoomSettingNotFound"));
                }

                if (lesson.Meeting.MeetingNumber == default)
                {
                    await _zoomLicenseService.CreateZoomMeetingAsync(lesson);
                }

                var signature = await _zoomLicenseService
                    .GenerateZoomSignatureAsync(
                        lesson.Meeting.MeetingNumber.ToString(),
                        isModerator
                    )
                    .ConfigureAwait(false);
                var zak = await _zoomLicenseService
                    .GetZAKAsync(lesson.Meeting.ZoomLicense.HostId)
                    .ConfigureAwait(false);
                var response = new MeetingJoinResponseModel
                {
                    RoomName = lesson.Name,
                    JwtToken = signature,
                    ZAKToken = zak,
                    SdkKey = zoomSetting.SdkKey,
                    MeetingId = lesson.Meeting?.MeetingNumber,
                    Passcode = lesson.Meeting?.Passcode,
                    User = new UserModel(user)
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to join live class.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("LiveClassJoinError"));
            }
        }

        /// <summary>
        /// Handle to get meeting report
        /// </summary>
        /// <param name="identity"> the lesson identity </param>
        /// <param name="userId"> the user id </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="MeetingReportResponseModel" /> .</returns>
        public async Task<IList<MeetingReportResponseModel>> GetMeetingReportAsync(
            string identity,
            string lessonIdentity,
            string userId,
            Guid currentUserId
        )
        {
            var response = new List<MeetingReportResponseModel>();
            var course = await ValidateAndGetCourse(
                    currentUserId,
                    identity,
                    validateForModify: true
                )
                .ConfigureAwait(false);
            var lesson = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    (p.Id.ToString() == lessonIdentity || p.Slug.Equals(lessonIdentity))
                    && (p.Type == LessonType.LiveClass || p.Type == LessonType.RecordedVideo)
                    && p.MeetingId != null
                )
                .ConfigureAwait(false);
            if (lesson == default)
            {
                _logger.LogError(
                    $"GetMeetingReportAsync: Lesson with identity : {identity.SanitizeForLogger()} not found."
                );
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }

            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: x => x.Id.ToString().Equals(userId))
                .ConfigureAwait(false);
            if (user == default)
            {
                throw new EntityNotFoundException(_localizer.GetString("UserNotFound"));
            }

            var reports = await _unitOfWork
                .GetRepository<MeetingReport>()
                .GetAllAsync(predicate: p =>
                    p.MeetingId == lesson.MeetingId && p.UserId.ToString() == userId
                )
                .ConfigureAwait(false);
            if (reports == default)
            {
                _logger.LogError(
                    $"GetMeetingReportAsync : Meeting report of user with id {userId.SanitizeForLogger()} not found."
                );
                throw new EntityNotFoundException(_localizer.GetString("MeetingReportNotFound"));
            }

            return reports
                .Select(x => new MeetingReportResponseModel
                {
                    UserId = x.UserId,
                    Name = user.FullName,
                    Email = user.Email,
                    MobileNumber = user.MobileNumber,
                    StartDate = x.StartTime,
                    JoinedTime = x.JoinTime.ToShortTimeString(),
                    LeftTime = x.LeftTime.HasValue
                        ? x.LeftTime.Value.ToShortTimeString()
                        : string.Empty,
                    LessonId = lesson.Id,
                    Duration = x.Duration.HasValue ? (int)x.Duration.Value.TotalSeconds : default,
                })
                .ToList();
        }

        /// <summary>
        /// Handle to reorder lesson
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="LessonReorderRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task ReorderAsync(
            string identity,
            LessonReorderRequestModel model,
            Guid currentUserId
        )
        {
            try
            {
                var course = await ValidateAndGetCourse(
                        currentUserId,
                        identity,
                        validateForModify: true
                    )
                    .ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning(
                        "ReorderAsync(): Training with identity : {identity} not found for user with id :{userId}.",
                        identity.SanitizeForLogger(),
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                var section = await _unitOfWork
                    .GetRepository<Section>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.CourseId == course.Id
                        && (
                            p.Id.ToString() == model.SectionIdentity
                            || p.Slug == model.SectionIdentity
                        )
                    )
                    .ConfigureAwait(false);
                if (section == null)
                {
                    _logger.LogWarning(
                        "ReorderAsync(): Section with identity : {identity} not found for training with id : {id} and user with id :{userId}.",
                        model.SectionIdentity.SanitizeForLogger(),
                        course.Id,
                        currentUserId
                    );
                    throw new EntityNotFoundException(_localizer.GetString("SectionNotFound"));
                }

                var lessons = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetAllAsync(predicate: p =>
                        p.CourseId == course.Id && model.Ids.Contains(p.Id)
                    )
                    .ConfigureAwait(false);

                var order = 1;
                var currentTimeStamp = DateTime.UtcNow;
                var updateEntities = new List<Lesson>();
                foreach (var id in model.Ids)
                {
                    var lesson = lessons.FirstOrDefault(x => x.Id == id);
                    if (lesson != null)
                    {
                        lesson.Order = order;
                        lesson.SectionId = section.Id;
                        lesson.UpdatedBy = currentUserId;
                        lesson.UpdatedOn = currentTimeStamp;
                        updateEntities.Add(lesson);
                        order++;
                    }
                }

                if (updateEntities.Count > 0)
                {
                    _unitOfWork.GetRepository<Lesson>().Update(updateEntities);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to reorder the lessons.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("LessonReorderError"));
            }
        }

        #region Private Methods

        /// <summary>
        /// Handle to get user current watched lesson
        /// </summary>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="course">the instance of <see cref="Course"/> </param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        private async Task<Lesson> GetCurrentLesson(Guid currentUserId, Course course)
        {
            var currentLessonWatched = course.CourseEnrollments.FirstOrDefault(x =>
                x.UserId == currentUserId
            );
            var currentLessonId = currentLessonWatched?.CurrentLessonId;
            if (currentLessonId == default)
            {
                var watchHistories = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .GetAllAsync(predicate: p =>
                        p.CourseId == course.Id && p.UserId == currentUserId
                    )
                    .ConfigureAwait(false);
                if (watchHistories.Count == 0)
                {
                    var section = await _unitOfWork
                        .GetRepository<Section>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.CourseId == course.Id,
                            orderBy: o => o.OrderBy(x => x.Order),
                            include: src => src.Include(x => x.Lessons)
                        )
                        .ConfigureAwait(false);
                    currentLessonId = section?.Lessons?.OrderBy(x => x.Order).FirstOrDefault()?.Id;
                }
                else
                {
                    currentLessonId = watchHistories
                        .OrderByDescending(x => x.UpdatedOn)
                        .FirstOrDefault()
                        ?.LessonId;
                }
            }

            var currentLesson = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Id == currentLessonId,
                    include: src =>
                        src.Include(x => x.User).Include(x => x.Course).Include(x => x.Section)
                )
                .ConfigureAwait(false);
            if (currentLesson == null)
            {
                _logger.LogWarning(
                    "Current watch lesson not found for training with id : {courseId} and user with id : {userId}.",
                    course.Id,
                    currentUserId
                );
                throw new EntityNotFoundException(
                    _localizer.GetString("CurrentWatchLessonNotFound")
                );
            }

            return currentLesson;
        }

        /// <summary>
        /// Handle to  create question set
        /// </summary>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="lesson">the instance of <see cref="Lesson"/></param>
        /// <returns></returns>
        private async Task CreateQuestionSetAsync(LessonRequestModel model, Lesson lesson)
        {
            lesson.QuestionSet = new QuestionSet();

            var startDate = DateTime.UtcNow;
            var startTimeUtc = (DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)).TimeOfDay;
            var timeDifference = (
                model.QuestionSet.StartTime.Value.TimeOfDay.TotalSeconds - startTimeUtc.TotalSeconds
            );

            if (startDate > model.QuestionSet.StartTime)
            {
                if (Math.Abs(timeDifference) > 60)
                {
                    throw new InvalidDataException(_localizer.GetString("InvalidTimeIssue"));
                }
            }

            lesson.QuestionSet = new QuestionSet
            {
                Id = Guid.NewGuid(),
                Name = lesson.Name,
                Slug = lesson.Slug,
                ThumbnailUrl = lesson.ThumbnailUrl,
                Description = model.QuestionSet.Description,
                NegativeMarking = model.QuestionSet.NegativeMarking,
                QuestionMarking = model.QuestionSet.QuestionMarking,
                PassingWeightage = model.QuestionSet.PassingWeightage,
                AllowedRetake = model.QuestionSet.AllowedRetake,
                StartTime = model.QuestionSet.StartTime,
                EndTime = model.QuestionSet.EndTime,
                Duration = model.QuestionSet.Duration * 60, //convert duration from minutes to seconds;
                CreatedBy = lesson.CreatedBy,
                CreatedOn = lesson.CreatedOn,
                UpdatedBy = lesson.UpdatedBy,
                UpdatedOn = lesson.UpdatedOn
            };

            lesson.QuestionSetId = lesson.QuestionSet.Id;
            await _unitOfWork
                .GetRepository<QuestionSet>()
                .InsertAsync(lesson.QuestionSet)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to update question set
        /// </summary>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="existingLesson">the instance of <see cref="Lesson"/></param>
        /// <returns></returns>
        private async Task UpdateQuestionSetAsync(LessonRequestModel model, Lesson existingLesson)
        {
            if (existingLesson.QuestionSet.StartTime != model.QuestionSet.StartTime)
            {
                var startDate = DateTime.UtcNow;
                var startTimeUtc = (
                    DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                ).TimeOfDay;
                var timeDifference = (
                    model.QuestionSet.StartTime.Value.TimeOfDay.TotalSeconds
                    - startTimeUtc.TotalSeconds
                );

                if (startDate > model.QuestionSet.StartTime)
                {
                    if (Math.Abs(timeDifference) > 60)
                    {
                        throw new InvalidDataException(_localizer.GetString("InvalidTimeIssue"));
                    }
                }
            }

            if (
                model.QuestionSet.IsShuffle == false
                && model.QuestionSet.NoOfQuestion == 0
                && model.QuestionSet.ShowAll == false
            )
            {
                throw new ForbiddenException("Field Is  Required For Shuffling");
            }

            existingLesson.QuestionSet.Name = existingLesson.Name;
            existingLesson.QuestionSet.ThumbnailUrl = existingLesson.ThumbnailUrl;
            existingLesson.QuestionSet.Description = model.QuestionSet.Description;
            existingLesson.QuestionSet.NegativeMarking = model.QuestionSet.NegativeMarking;
            existingLesson.QuestionSet.QuestionMarking = model.QuestionSet.QuestionMarking;
            existingLesson.QuestionSet.PassingWeightage = model.QuestionSet.PassingWeightage;
            existingLesson.QuestionSet.AllowedRetake = model.QuestionSet.AllowedRetake;
            existingLesson.QuestionSet.StartTime = model.QuestionSet.StartTime;
            existingLesson.QuestionSet.EndTime = model.QuestionSet.EndTime;
            existingLesson.QuestionSet.Duration = model.QuestionSet.Duration * 60; //convert duration from minutes to seconds;
            existingLesson.QuestionSet.UpdatedBy = existingLesson.UpdatedBy;
            existingLesson.QuestionSet.UpdatedOn = existingLesson.UpdatedOn;
            existingLesson.QuestionSet.IsShuffle = model.QuestionSet.IsShuffle;
            existingLesson.QuestionSet.ShowAll = model.QuestionSet.ShowAll;
            existingLesson.QuestionSet.NoOfQuestion = model.QuestionSet.NoOfQuestion;

            _unitOfWork.GetRepository<QuestionSet>().Update(existingLesson.QuestionSet);
            await Task.FromResult(0);
        }

        public async Task<QuestionSet> UpdateQuestionAsync(
            string identity,
            string lessonIdentity,
            QuestionSetRequestModel model,
            Guid currentUserId
        )
        {
            var course = await ValidateAndGetCourse(
                    currentUserId,
                    identity,
                    validateForModify: true
                )
                .ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning(
                    "Training with identity: {identity} not found for user with :{id}.",
                    identity.SanitizeForLogger(),
                    currentUserId
                );
                throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            }

            if (course.Status == CourseStatus.Completed)
            {
                throw new InvalidOperationException(_localizer.GetString("CompletedCourseIssue"));
            }

            var existing = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity)
                )
                .ConfigureAwait(false);
            if (existing == null)
            {
                _logger.LogWarning(
                    "Lesson with identity: {identity} not found for user with id: {id} and training with id: {courseId}",
                    lessonIdentity.SanitizeForLogger(),
                    currentUserId,
                    course.Id
                );
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }
            var existingQuestionSet = await _unitOfWork
                .GetRepository<QuestionSet>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == existing.QuestionSetId)
                .ConfigureAwait(false);
            if (model.IsShuffle == false && model.NoOfQuestion == 0 && model.ShowAll == false)
            {
                throw new ForbiddenException("Field Is  Required For Shuffling");
            }

            existingQuestionSet.IsShuffle = model.IsShuffle;
            existingQuestionSet.ShowAll = model.ShowAll;
            existingQuestionSet.NoOfQuestion = model.NoOfQuestion;

            _unitOfWork.GetRepository<QuestionSet>().Update(existingQuestionSet);
            await _unitOfWork.SaveChangesAsync();
            return existingQuestionSet;
        }

        public async Task<QuestionSet> GetQuestionAsync(
            string identity,
            string lessonIdentity,
            Guid currentUserId
        )
        {
            var course = await ValidateAndGetCourse(
                    currentUserId,
                    identity,
                    validateForModify: true
                )
                .ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning(
                    "Training with identity: {identity} not found for user with :{id}.",
                    identity.SanitizeForLogger(),
                    currentUserId
                );
                throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            }

            if (course.Status == CourseStatus.Completed)
            {
                throw new InvalidOperationException(_localizer.GetString("CompletedCourseIssue"));
            }

            var existing = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity)
                )
                .ConfigureAwait(false);
            if (existing == null)
            {
                _logger.LogWarning(
                    "Lesson with identity: {identity} not found for user with id: {id} and training with id: {courseId}",
                    lessonIdentity.SanitizeForLogger(),
                    currentUserId,
                    course.Id
                );
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }
            var existingQuestionSet = await _unitOfWork
                .GetRepository<QuestionSet>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == existing.QuestionSetId)
                .ConfigureAwait(false);
            return existingQuestionSet;
        }

        /// <summary>
        /// Handle to create meeting
        /// </summary>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="lesson">the instance of <see cref="Lesson"/></param>
        /// <returns></returns>
        private async Task CreateMeetingAsync(LessonRequestModel model, Lesson lesson)
        {
            lesson.Meeting = new Meeting();
            var startDate = DateTime.UtcNow;
            var startTimeUtc = (DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)).TimeOfDay;
            var timeDifference = (
                model.Meeting.MeetingStartDate.TimeOfDay.TotalSeconds - startTimeUtc.TotalSeconds
            );

            if (startDate > model.Meeting.MeetingStartDate)
            {
                if (Math.Abs(timeDifference) > 60)
                {
                    throw new InvalidDataException(_localizer.GetString("InvalidMeetingTimeIssue"));
                }
            }

            lesson.Meeting = new Meeting
            {
                Id = Guid.NewGuid(),
                StartDate = model.Meeting.MeetingStartDate,
                ZoomLicenseId = model.Meeting.ZoomLicenseId.Value,
                Duration = model.Meeting.MeetingDuration * 60, //convert duration from minutes to seconds;
                CreatedBy = lesson.CreatedBy,
                CreatedOn = lesson.CreatedOn,
                UpdatedBy = lesson.UpdatedBy,
                UpdatedOn = lesson.UpdatedOn
            };
            lesson.MeetingId = lesson.Meeting.Id;

            await _unitOfWork
                .GetRepository<Meeting>()
                .InsertAsync(lesson.Meeting)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to update meeting
        /// </summary>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="existingLesson">the instance of <see cref="Lesson"/></param>
        /// <returns></returns>
        private async Task UpdateMeetingAsync(LessonRequestModel model, Lesson existingLesson)
        {
            if (existingLesson.Meeting.StartDate != model.Meeting.MeetingStartDate)
            {
                var startDate = DateTime.UtcNow;
                var startTimeUtc = (
                    DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                ).TimeOfDay;
                var timeDifference = (
                    model.Meeting.MeetingStartDate.TimeOfDay.TotalSeconds
                    - startTimeUtc.TotalSeconds
                );

                if (startDate > model.Meeting.MeetingStartDate)
                {
                    if (Math.Abs(timeDifference) > 60)
                    {
                        throw new InvalidDataException(
                            _localizer.GetString("InvalidMeetingTimeIssue")
                        );
                    }
                }
            }

            existingLesson.Meeting.StartDate = model.Meeting.MeetingStartDate;
            existingLesson.Meeting.ZoomLicenseId = model.Meeting.ZoomLicenseId.Value;
            existingLesson.Meeting.Duration = model.Meeting.MeetingDuration * 60; //convert duration from minutes to seconds;
            existingLesson.Meeting.UpdatedBy = existingLesson.UpdatedBy;
            existingLesson.Meeting.UpdatedOn = existingLesson.UpdatedOn;

            _unitOfWork.GetRepository<Meeting>().Update(existingLesson.Meeting);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Handle to get last lesson order number
        /// </summary>
        /// <param name="entity"> the instance of <see cref="Lesson" /> .</param>
        /// <returns> the int value </returns>
        private async Task<int> LastLessonOrder(Lesson entity)
        {
            var lesson = await _unitOfWork
                .GetRepository<Lesson>()
                .GetFirstOrDefaultAsync(
                    predicate: x =>
                        x.CourseId == entity.CourseId
                        && x.SectionId == entity.SectionId
                        && !x.IsDeleted,
                    orderBy: x => x.OrderByDescending(x => x.Order)
                )
                .ConfigureAwait(false);
            return lesson != null ? lesson.Order + 1 : 1;
        }

        /// <summary>
        /// Handle to validate isLiveClassModerator
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <param name="course"> the instance of <see cref="Course"/></param>
        /// <returns> the boolean value </returns>
        private async Task<bool> IsLiveClassModerator(Guid currentUserId, Course course)
        {
            var isAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
            if (isAdmin)
            {
                return true;
            }

            if (course.CreatedBy == currentUserId)
            {
                return true;
            }

            if (course.CourseTeachers.Any(x => x.UserId == currentUserId))
            {
                return true;
            }

            return false;
        }

        #endregion Private Methods
    }
}
