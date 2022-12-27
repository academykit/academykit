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
    using System.Linq;
    using System.Linq.Expressions;

    public class AssignmentService : BaseGenericService<Assignment, AssignmentBaseSearchCriteria>, IAssignmentService
    {
        private readonly ICourseService _courseService;
        public AssignmentService(
            IUnitOfWork unitOfWork,
            ILogger<AssignmentService> logger,
            ICourseService courseService) : base(unitOfWork, logger)
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
        protected override async Task CheckDeletePermissionsAsync(Assignment entity, Guid currentUserId)
        {
            await ValidateAndGetLessonForAssignment(entity).ConfigureAwait(false);

            var assignmentSubmissions = await _unitOfWork.GetRepository<AssignmentSubmission>().ExistsAsync(
                    predicate: p => p.AssignmentId == entity.Id).ConfigureAwait(false);
            if (assignmentSubmissions)
            {
                _logger.LogWarning("Assignment with id : {id} having type : {type} contains assignment submissions", entity.Id, entity.Type);
                throw new ForbiddenException("Assignment contains assignment submissions");
            }

            _unitOfWork.GetRepository<AssignmentAttachment>().Delete(entity.AssignmentAttachments);
            _unitOfWork.GetRepository<AssignmentQuestionOption>().Delete(entity.AssignmentQuestionOptions);
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
        protected override IIncludableQueryable<Assignment, object> IncludeNavigationProperties(IQueryable<Assignment> query)
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

            if (entity.AssignmentQuestionOptions.Count > 0)
            {
                await _unitOfWork.GetRepository<AssignmentQuestionOption>().InsertAsync(entity.AssignmentQuestionOptions).ConfigureAwait(false);
            }
            if (entity.AssignmentAttachments.Count > 0)
            {
                await _unitOfWork.GetRepository<AssignmentAttachment>().InsertAsync(entity.AssignmentAttachments).ConfigureAwait(false);
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
            entity.AssignmentAttachments = await _unitOfWork.GetRepository<AssignmentAttachment>().GetAllAsync(predicate: p => p.AssignmentId == entity.Id).ConfigureAwait(false);
            entity.AssignmentQuestionOptions = await _unitOfWork.GetRepository<AssignmentQuestionOption>().GetAllAsync(predicate: p => p.AssignmentId == entity.Id).ConfigureAwait(false);
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
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                            predicate: p => p.Id == entity.LessonId && !p.IsDeleted).ConfigureAwait(false);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson with id : {lessonId} not found for assignment with id : {id}", entity.LessonId, entity.Id);
                throw new EntityNotFoundException("Lesson not found");
            }
            if (lesson.Type != LessonType.Assignment)
            {
                _logger.LogWarning("Lesson with id : {lessonId} is of invalid lesson type to create,edit or delete assignment for user with id :{userId}", lesson.Id, entity.CreatedBy);
                throw new ForbiddenException("Invalid lesson type for assignment.");
            }
            await ValidateAndGetCourse(entity.CreatedBy, lesson.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);
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
        public async Task<Assignment> UpdateAsync(string identity, AssignmentRequestModel model, Guid currentUserId)
        {
            try
            {
                var existing = await GetByIdOrSlugAsync(identity, currentUserId).ConfigureAwait(false);
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

                if (model.Type == QuestionTypeEnum.SingleChoice || model.Type == QuestionTypeEnum.MultipleChoice)
                {
                    foreach (var item in model.Answers.Select((answer, i) => new { i, answer }))
                    {
                        assignmentQuestionOptions.Add(new AssignmentQuestionOption
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
                        });
                    }
                }
                if ((model.Type == QuestionTypeEnum.SingleChoice || model.Type == QuestionTypeEnum.MultipleChoice) && model.FileUrls.Count > 0)
                {
                    foreach (var item in model.FileUrls.Select((fileUrl, i) => new { i, fileUrl }))
                    {
                        assignmentAttachments.Add(new AssignmentAttachment
                        {
                            Id = Guid.NewGuid(),
                            AssignmentId = existing.Id,
                            FileUrl = item.fileUrl,
                            Order = item.i + 1,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp,
                        });
                    }
                }
                if (existing.AssignmentAttachments.Count > 0)
                {
                    _unitOfWork.GetRepository<AssignmentAttachment>().Delete(existing.AssignmentAttachments);
                }
                if (existing.AssignmentQuestionOptions.Count > 0)
                {
                    _unitOfWork.GetRepository<AssignmentQuestionOption>().Delete(existing.AssignmentQuestionOptions);
                }
                if (assignmentAttachments.Count > 0)
                {
                    await _unitOfWork.GetRepository<AssignmentAttachment>().InsertAsync(assignmentAttachments).ConfigureAwait(false);
                }
                if (assignmentQuestionOptions.Count > 0)
                {
                    await _unitOfWork.GetRepository<AssignmentQuestionOption>().InsertAsync(assignmentQuestionOptions).ConfigureAwait(false);
                }
                _unitOfWork.GetRepository<Assignment>().Update(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update assignment.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to update assignment.");
            }
        }

        /// <summary>
        /// Handle to submit assignments by the user
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="models">the list of <see cref="AssignmentSubmissionRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        public async Task AssignmentSubmissionAsync(string lessonIdentity, IList<AssignmentSubmissionRequestModel> models, Guid currentUserId)
        {
            try
            {
                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException("Lesson not found");
                }
                if (lesson.Type != LessonType.Assignment)
                {
                    _logger.LogWarning("Lesson type not matched for assignment submission for lesson with id: {id} and user with id: {userId}",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException($"Invalid lesson type :{lesson.Type}");
                }
                if (lesson.Status != CourseStatus.Published)
                {
                    _logger.LogWarning("Lesson with id: {id} not published for user with id: {userId}", lesson.Id, currentUserId);
                    throw new EntityNotFoundException("Lesson not published");
                }

                var course = await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);
                if (course.Status == CourseStatus.Completed)
                {
                    _logger.LogWarning("Course with id : {courseId} is in {status} status to give assignment for the user with id: {userId}",
                        course.Id, course.Status, currentUserId);
                    throw new ForbiddenException($"Cannot submit assignment to the course having {course.Status} status");
                }
                if (course.CourseTeachers.Any(x => x.UserId == currentUserId))
                {
                    _logger.LogWarning("User with id: {userId} is a teacher of the course with id: {courseId} and lesson with id: {lessonId} to submit the assignment",
                        currentUserId, course.Id, lesson.Id);
                    throw new ForbiddenException("Course teacher cannot submit the assignment");
                }

                var assignmentReviewExist = await _unitOfWork.GetRepository<AssignmentReview>().ExistsAsync(
                    predicate: p => p.LessonId == lesson.Id && p.UserId == currentUserId && !p.IsDeleted
                    ).ConfigureAwait(false);
                if (assignmentReviewExist)
                {
                    _logger.LogWarning("Assignment review exist for lesson with id: {lessonId} and user with id : {userId}",
                        lesson.Id, currentUserId);
                    throw new ForbiddenException("Review has been already given to current assignment");
                }

                var assignments = await _unitOfWork.GetRepository<Assignment>().GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id && p.IsActive,
                    include: src => src.Include(x => x.AssignmentQuestionOptions)).ConfigureAwait(false);

                var watchHistory = await _unitOfWork.GetRepository<WatchHistory>().GetFirstOrDefaultAsync(
                    predicate: p => p.LessonId == lesson.Id && p.UserId == currentUserId
                    ).ConfigureAwait(false);

                var currentTimeStamp = DateTime.UtcNow;

                foreach (var item in models)
                {
                    var assignment = assignments.FirstOrDefault(x => x.Id == item.AssignmentId);
                    if (assignment != null)
                    {
                        if (item.Id != default)
                        {
                            await UpdateSubmissionAsync(currentUserId, currentTimeStamp, item, assignment).ConfigureAwait(false);
                        }
                        else
                        {
                            await InsertSubmissionAsync(currentUserId, lesson.Id, currentTimeStamp, item, assignment).ConfigureAwait(false);
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
                    await _unitOfWork.GetRepository<WatchHistory>().InsertAsync(watchHistory).ConfigureAwait(false);
                }

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to submit the assignment.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to submit the assignment.");
            }
        }

        /// <summary>
        /// Handle to fetch student submitted assignment
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="userId">the user id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the instance of <see cref="AssignmentSubmissionStudentResponseModel"/></returns>
        public async Task<AssignmentSubmissionStudentResponseModel> GetStudentSubmittedAssignment(string lessonIdentity, Guid userId, Guid currentUserId)
        {
            try
            {
                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                   predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException("Lesson not found");
                }
                if (lesson.Type != LessonType.Assignment)
                {
                    _logger.LogWarning("Lesson type not matched for assignment submission for lesson with id: {id} and user with id: {userId}",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException($"Invalid lesson type :{lesson.Type}");
                }
                var course = await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);

                var predicate = PredicateBuilder.New<Assignment>(true);
                predicate = predicate.And(x => x.LessonId == lesson.Id);

                var assignments = await _unitOfWork.GetRepository<Assignment>().GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id,
                    include: src => src.Include(x => x.AssignmentAttachments).Include(x => x.AssignmentQuestionOptions)
                    ).ConfigureAwait(false);

                var userAssignments = await _unitOfWork.GetRepository<AssignmentSubmission>().GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id && p.UserId == userId,
                    include: src => src.Include(x => x.AssignmentSubmissionAttachments).Include(x => x.User)
                    ).ConfigureAwait(false);

                var assignmentReview = await _unitOfWork.GetRepository<AssignmentReview>().GetFirstOrDefaultAsync(
                    predicate: p => p.LessonId == lesson.Id && p.UserId == userId,
                    include: src => src.Include(x => x.User)
                    ).ConfigureAwait(false);
                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Id == currentUserId).ConfigureAwait(false);

                var response = new AssignmentSubmissionStudentResponseModel
                {
                    LessonId = lesson.Id,
                    LessonSlug = lesson.Slug,
                    User = user != null ? new UserModel(user) : null,
                    Assignments = new List<AssignmentResponseModel>()
                };
                if (assignmentReview != null)
                {
                    var teacher = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                        predicate: p => p.Id == assignmentReview.CreatedBy
                        ).ConfigureAwait(false);
                    response.AssignmentReview = new AssignmentReviewResponseModel
                    {
                        Id = assignmentReview.Id,
                        LessonId = assignmentReview.LessonId,
                        Mark = assignmentReview.Mark,
                        Review = assignmentReview.Review,
                        UserId = assignmentReview.UserId,
                        User = new UserModel(assignmentReview.User),
                        Teacher = teacher != null ? new UserModel(teacher) : null
                    };
                }

                response.UserStatus = await _courseService.GetUserCourseEnrollmentStatus(course, currentUserId, fetchMembers: true).ConfigureAwait(false);

                foreach (var item in assignments)
                {
                    MapAssignment(true, userAssignments, item, response.Assignments);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch the student submitted assignment.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to fetch the student submitted assignment.");
            }
        }

        /// <summary>
        /// Handle to search assignment
        /// </summary>
        /// <param name="searchCriteria">the instance of <see cref="AssignmentBaseSearchCriteria"/></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        public async Task<IList<AssignmentResponseModel>> SearchAsync(AssignmentBaseSearchCriteria searchCriteria)
        {
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
               predicate: p => p.Id.ToString() == searchCriteria.LessonIdentity || p.Slug == searchCriteria.LessonIdentity
               ).ConfigureAwait(false);

            if (lesson == null)
            {
                _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}", searchCriteria.LessonIdentity, searchCriteria.CurrentUserId);
                throw new EntityNotFoundException("Lesson not found");
            }
            if (lesson.Type != LessonType.Assignment)
            {
                _logger.LogWarning("Lesson type not matched for assignment fetch for lesson with id: {id} and user with id: {userId}",
                                    lesson.Id, searchCriteria.CurrentUserId);
                throw new ForbiddenException($"Invalid lesson type :{lesson.Type}");
            }

            var course = await ValidateAndGetCourse(searchCriteria.CurrentUserId, lesson.CourseId.ToString(), validateForModify: false).ConfigureAwait(false);

            var isTeacher = course.CourseTeachers.Any(x => x.UserId == searchCriteria.CurrentUserId);
            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(searchCriteria.CurrentUserId).ConfigureAwait(false);

            if (!isTeacher && !isSuperAdminOrAdmin && searchCriteria.UserId == null)
            {
                searchCriteria.UserId = searchCriteria.CurrentUserId;
            }

            var showCorrectAndHints = isTeacher || isSuperAdminOrAdmin;

            var predicate = PredicateBuilder.New<Assignment>(true);
            predicate = predicate.And(x => x.LessonId == lesson.Id);

            var assignments = await _unitOfWork.GetRepository<Assignment>().GetAllAsync(
                predicate: p => p.LessonId == lesson.Id,
                include: src => src.Include(x => x.AssignmentAttachments).Include(x => x.AssignmentQuestionOptions)
                ).ConfigureAwait(false);

            var userAssignments = await _unitOfWork.GetRepository<AssignmentSubmission>().GetAllAsync(
                predicate: p => p.LessonId == lesson.Id && searchCriteria.UserId.HasValue && p.UserId == searchCriteria.UserId.Value,
                include: src => src.Include(x => x.AssignmentSubmissionAttachments).Include(x => x.User)
                ).ConfigureAwait(false);

            var assignmentReview = await _unitOfWork.GetRepository<AssignmentReview>().GetFirstOrDefaultAsync(
                predicate: p => p.LessonId == lesson.Id && searchCriteria.UserId.HasValue && p.UserId == searchCriteria.UserId.Value
                ).ConfigureAwait(false);

            var response = new List<AssignmentResponseModel>();

            foreach (var item in assignments)
            {
                MapAssignment(showCorrectAndHints, userAssignments, item, response);
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
        public async Task AssignmentReviewAsync(string lessonIdentity, AssignmentReviewRequestModel model, Guid currentUserId)
        {
            try
            {
                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                   predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException("Lesson not found");
                }
                if (lesson.Type != LessonType.Assignment)
                {
                    _logger.LogWarning("Lesson type not matched for assignment submission for lesson with id: {id} and user with id: {userId}",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException($"Invalid lesson type :{lesson.Type}");
                }
                await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);

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
                var watchHistory = await _unitOfWork.GetRepository<WatchHistory>().GetFirstOrDefaultAsync(
                   predicate: p => p.LessonId == lesson.Id && p.UserId == currentUserId
                   ).ConfigureAwait(false);
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
                    await _unitOfWork.GetRepository<WatchHistory>().InsertAsync(watchHistory).ConfigureAwait(false);
                }
                await _unitOfWork.GetRepository<AssignmentReview>().InsertAsync(assignmentReview).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to submit assignment review.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to submit assignment review.");
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
        public async Task UpdateAssignmentReviewAsync(string lessonIdentity, Guid id, AssignmentReviewRequestModel model, Guid currentUserId)
        {
            try
            {
                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                   predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException("Lesson not found");
                }
                if (lesson.Type != LessonType.Assignment)
                {
                    _logger.LogWarning("Lesson type not matched for assignment submission for lesson with id: {id} and user with id: {userId}",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException($"Invalid lesson type :{lesson.Type}");
                }
                await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);

                var assignmentReview = await _unitOfWork.GetRepository<AssignmentReview>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id && p.LessonId == lesson.Id && p.UserId == model.UserId && !p.IsDeleted
                    ).ConfigureAwait(false);
                if (assignmentReview == null)
                {
                    _logger.LogWarning("Assignment review with id: {id} not found for user with id: {userId} and lesson with id: {lessonId}",
                                    id, currentUserId, lesson.Id);
                    throw new EntityNotFoundException("Assignment Review not found");
                }
                var currentTimeStamp = DateTime.UtcNow;

                assignmentReview.Mark = model.Marks;
                assignmentReview.Review = model.Review;
                assignmentReview.UpdatedBy = currentUserId;
                assignmentReview.UpdatedOn = currentTimeStamp;

                var watchHistory = await _unitOfWork.GetRepository<WatchHistory>().GetFirstOrDefaultAsync(
                   predicate: p => p.LessonId == lesson.Id && p.UserId == currentUserId
                   ).ConfigureAwait(false);
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
                    await _unitOfWork.GetRepository<WatchHistory>().InsertAsync(watchHistory).ConfigureAwait(false);
                }
                _unitOfWork.GetRepository<AssignmentReview>().Update(assignmentReview);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update assignment review.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to update assignment review.");
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
                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                   predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException("Lesson not found");
                }
                if (lesson.Type != LessonType.Assignment)
                {
                    _logger.LogWarning("Lesson type not matched for assignment submission for lesson with id: {id} and user with id: {userId}",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException($"Invalid lesson type :{lesson.Type}");
                }
                await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);

                var assignmentReview = await _unitOfWork.GetRepository<AssignmentReview>().GetFirstOrDefaultAsync(
                       predicate: p => p.Id == id && p.LessonId == lesson.Id && p.UserId == currentUserId && !p.IsDeleted
                       ).ConfigureAwait(false);
                if (assignmentReview == null)
                {
                    _logger.LogWarning("Assignment review with id: {id} not found for user with id: {userId} and lesson with id: {lessonId}",
                                    id, currentUserId, lesson.Id);
                    throw new EntityNotFoundException("Assignment Review not found");
                }

                var watchHistory = await _unitOfWork.GetRepository<WatchHistory>().GetFirstOrDefaultAsync(
                       predicate: p => p.LessonId == lesson.Id && p.UserId == currentUserId
                       ).ConfigureAwait(false);
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
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to delete assignment review.");
            }
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
        private async Task InsertSubmissionAsync(Guid currentUserId, Guid lessonId, DateTime currentTimeStamp, AssignmentSubmissionRequestModel item, Assignment assignment)
        {
            var assignmentSubmission = await _unitOfWork.GetRepository<AssignmentSubmission>().GetFirstOrDefaultAsync(
                predicate: p => p.Id == item.Id && p.UserId == currentUserId
                ).ConfigureAwait(false);
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
            if (assignment.Type == QuestionTypeEnum.SingleChoice || assignment.Type == QuestionTypeEnum.MultipleChoice)
            {
                var answerIds = assignment.AssignmentQuestionOptions?.Where(x => x.IsCorrect).Select(x => x.Id);
                bool? isCorrect = answerIds?.OrderBy(x => x).ToList().SequenceEqual(item.SelectedOption.OrderBy(x => x).ToList());

                assignmentSubmission.IsCorrect = isCorrect ?? false;
                assignmentSubmission.SelectedOption = string.Join(",", item.SelectedOption);
            }
            if (assignment.Type == QuestionTypeEnum.Subjective)
            {
                assignmentSubmission.Answer = item.Answer;
                assignmentSubmission.AssignmentSubmissionAttachments = new List<AssignmentSubmissionAttachment>();

                if (item.Id != default)
                {
                    var existingAssignmentSubmissionAttachments = await _unitOfWork.GetRepository<AssignmentSubmissionAttachment>().GetAllAsync(
                   predicate: p => p.AssignmentSubmissionId == item.Id).ConfigureAwait(false);

                    _unitOfWork.GetRepository<AssignmentSubmissionAttachment>().Delete(existingAssignmentSubmissionAttachments);
                }

                item.AttachmentUrls?.ForEach(attachment => assignmentSubmission.AssignmentSubmissionAttachments.Add(new AssignmentSubmissionAttachment
                {
                    Id = Guid.NewGuid(),
                    AssignmentSubmissionId = assignmentSubmission.Id,
                    FileUrl = attachment,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                }));

                await _unitOfWork.GetRepository<AssignmentSubmissionAttachment>().InsertAsync(assignmentSubmission.AssignmentSubmissionAttachments).ConfigureAwait(false);
            }
            await _unitOfWork.GetRepository<AssignmentSubmission>().InsertAsync(assignmentSubmission).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to update assignment submission
        /// </summary>
        /// <param name="currentUserId">the current logged in user</param>
        /// <param name="currentTimeStamp">the current time stamp</param>
        /// <param name="item">the instance of <see cref="AssignmentSubmissionRequestModel"/></param>
        /// <param name="assignment">the instance of <see cref="Assignment"/></param>
        /// <returns></returns>
        private async Task UpdateSubmissionAsync(Guid currentUserId, DateTime currentTimeStamp, AssignmentSubmissionRequestModel item, Assignment assignment)
        {
            var assignmentSubmission = await _unitOfWork.GetRepository<AssignmentSubmission>().GetFirstOrDefaultAsync(
                                            predicate: p => p.Id == item.Id && p.UserId == currentUserId
                                            ).ConfigureAwait(false);

            assignmentSubmission.UpdatedOn = currentTimeStamp;
            assignmentSubmission.UpdatedBy = currentUserId;
            if (assignment.Type == QuestionTypeEnum.SingleChoice || assignment.Type == QuestionTypeEnum.MultipleChoice)
            {
                var answerIds = assignment.AssignmentQuestionOptions?.Where(x => x.IsCorrect).Select(x => x.Id);
                bool? isCorrect = answerIds?.OrderBy(x => x).ToList().SequenceEqual(item.SelectedOption.OrderBy(x => x).ToList());

                assignmentSubmission.IsCorrect = isCorrect ?? false;
                assignmentSubmission.SelectedOption = string.Join(",", item.SelectedOption);
            }
            if (assignment.Type == QuestionTypeEnum.Subjective)
            {
                assignmentSubmission.Answer = item.Answer;
                assignmentSubmission.AssignmentSubmissionAttachments = new List<AssignmentSubmissionAttachment>();

                if (item.Id != default)
                {
                    var existingAssignmentSubmissionAttachments = await _unitOfWork.GetRepository<AssignmentSubmissionAttachment>().GetAllAsync(
                    predicate: p => p.AssignmentSubmissionId == item.Id).ConfigureAwait(false);

                    _unitOfWork.GetRepository<AssignmentSubmissionAttachment>().Delete(existingAssignmentSubmissionAttachments);
                }

                item.AttachmentUrls?.ForEach(attachment => assignmentSubmission.AssignmentSubmissionAttachments.Add(new AssignmentSubmissionAttachment
                {
                    Id = Guid.NewGuid(),
                    AssignmentSubmissionId = assignmentSubmission.Id,
                    FileUrl = attachment,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                }));

                await _unitOfWork.GetRepository<AssignmentSubmissionAttachment>().InsertAsync(assignmentSubmission.AssignmentSubmissionAttachments).ConfigureAwait(false);
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
        private static void MapAssignment(bool showCorrectAndHints, IList<AssignmentSubmission> userAssignments, Assignment item, IList<AssignmentResponseModel> response)
        {
            var userAssignment = userAssignments.FirstOrDefault(x => x.AssignmentId == item.Id);
            var data = new AssignmentResponseModel
            {
                Id = item.Id,
                LessonId = item.LessonId,
                Name = item.Name,
                Description = item.Description,
                Order = item.Order,
                Hints = item.Hints,
                Type = item.Type,
                IsActive = item.IsActive,
                User = item.User == null ? new UserModel() : new UserModel(item.User),
                Student = userAssignment == null ? null : new UserModel(userAssignment.User),
                AssignmentSubmissionId = userAssignment?.Id,
                Answer = userAssignment?.Answer,
                AssignmentAttachments = new List<AssignmentAttachmentResponseModel>(),
                AssignmentQuestionOptions = new List<AssignmentQuestionOptionResponseModel>(),
                AssignmentSubmissionAttachments = new List<AssignmentSubmissionAttachmentResponseModel>(),
            };
            if (item.Type == QuestionTypeEnum.Subjective)
            {
                item.AssignmentAttachments?.ToList().ForEach(x => data.AssignmentAttachments.Add(new AssignmentAttachmentResponseModel(x)));
            }

            if (item.Type == QuestionTypeEnum.SingleChoice || item.Type == QuestionTypeEnum.MultipleChoice)
            {
                var selectedAnsIds = !string.IsNullOrWhiteSpace(userAssignment?.SelectedOption) ?
                                        userAssignment?.SelectedOption.Split(",").Select(Guid.Parse).ToList() : new List<Guid>();
                item.AssignmentQuestionOptions?.ToList().ForEach(x =>
                                data.AssignmentQuestionOptions.Add(new AssignmentQuestionOptionResponseModel()
                                {
                                    Id = x.Id,
                                    AssignmentId = x.AssignmentId,
                                    AssignmentName = x.Assignment?.Name,
                                    Option = x.Option,
                                    IsCorrect = showCorrectAndHints ? x.IsCorrect : null,
                                    IsSelected = userAssignment != null ? selectedAnsIds?.Contains(x.Id) : null,
                                    Order = x.Order,
                                }));
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

        #endregion Private Methods

    }
}
