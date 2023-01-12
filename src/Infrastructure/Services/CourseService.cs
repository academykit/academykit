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
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Linq.Expressions;

    public class CourseService : BaseGenericService<Course, CourseBaseSearchCriteria>, ICourseService
    {
        private readonly string imageApi;
        private readonly IMediaService _mediaService;
        public CourseService(
            IUnitOfWork unitOfWork,
            ILogger<CourseService> logger,
            IConfiguration configuration,
            IMediaService mediaService) : base(unitOfWork, logger)
        {
            imageApi = configuration.GetSection("AppUrls:ImageApi").Value;
            _mediaService = mediaService;
        }
        #region Protected Methods
        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Course entity)
        {
            entity.Slug = CommonHelper.GetEntityTitleSlug<Course>(_unitOfWork, (slug) => q => q.Slug == slug, entity.Name);
            await _unitOfWork.GetRepository<CourseTag>().InsertAsync(entity.CourseTags).ConfigureAwait(false);
            await _unitOfWork.GetRepository<CourseTeacher>().InsertAsync(entity.CourseTeachers).ConfigureAwait(false);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Course, bool>> ConstructQueryConditions(Expression<Func<Course, bool>> predicate, CourseBaseSearchCriteria criteria)
        {
            if (criteria.Status.HasValue)
            {
                predicate = predicate.And(p => p.Status.Equals(criteria.Status.Value));
            }
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
            }

            if (criteria.CurrentUserId == default)
            {
                return predicate.And(x => x.IsUpdate || x.Status == CourseStatus.Published);
            }

            if (criteria.EnrollmentStatus?.Count > 0)
            {
                var enrollmentStatusPredicate = PredicateBuilder.New<Course>();
                foreach (var enrollmentStatus in criteria.EnrollmentStatus)
                {
                    switch (enrollmentStatus)
                    {
                        case CourseEnrollmentStatus.Enrolled:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.Or(p => p.CourseEnrollments.Any(e => e.UserId == criteria.CurrentUserId));
                            break;
                        case CourseEnrollmentStatus.NotEnrolled:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.Or(p => !p.CourseEnrollments.Any(e => e.UserId == criteria.CurrentUserId));
                            break;
                        case CourseEnrollmentStatus.Author:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.Or(p => p.CreatedBy == criteria.CurrentUserId);
                            break;
                        case CourseEnrollmentStatus.Teacher:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.Or(p => p.CourseTeachers.Any(e => e.UserId == criteria.CurrentUserId));
                            break;
                        default:
                            break;
                    }
                }
                predicate = predicate.And(enrollmentStatusPredicate);
            }

            var isSuperAdminOrAdmin = IsSuperAdminOrAdmin(criteria.CurrentUserId).Result;
            if (isSuperAdminOrAdmin)
            {
                return predicate;
            }

            Expression<Func<Course, bool>> groupPredicate = PredicateBuilder.New<Course>();
            var groupIds = GetUserGroupIds(criteria.CurrentUserId).Result;
            groupPredicate = PredicateBuilder.New<Course>(x => x.GroupId.HasValue && groupIds.Contains(x.GroupId ?? Guid.Empty));
            groupPredicate = groupPredicate.And(predicate);
            predicate = predicate.And(x => !x.GroupId.HasValue).Or(groupPredicate);
            return predicate.And(x => x.CreatedBy == criteria.CurrentUserId || x.CourseTeachers.Any(p => p.UserId == criteria.CurrentUserId)
                        || (x.CreatedBy != criteria.CurrentUserId && (x.IsUpdate || x.Status.Equals(CourseStatus.Published) || x.Status.Equals(CourseStatus.Completed))));
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(CourseBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(Course.CreatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Course, object> IncludeNavigationProperties(IQueryable<Course> query)
        {
            return query.Include(x => x.User)
                        .Include(x => x.CourseTags).ThenInclude(x => x.Tag)
                        .Include(x => x.Level)
                        .Include(x => x.Group)
                        .Include(x => x.CourseTeachers)
                        .Include(x => x.CourseEnrollments);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Course, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }

        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="course teacher"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(Course course, Guid currentUserId)
        {
            if (course.Status != CourseStatus.Draft)
            {
                _logger.LogWarning("Training with id : {courseId} cannot be deleted having status : {status}.", course.Id, course.Status.ToString());
                throw new ForbiddenException("Only training with draft status can be deleted.");
            }
            await ValidateAndGetCourse(currentUserId, courseIdentity: course.Id.ToString(), validateForModify: true).ConfigureAwait(false);
        }

        /// <summary>
        /// Check if entity could be accessed by current user
        /// </summary>
        /// <param name="entityToReturn">The entity being returned</param>
        protected override async Task CheckGetPermissionsAsync(Course entityToReturn, Guid? CurrentUserId = null)
        {
            if (!CurrentUserId.HasValue)
            {
                _logger.LogWarning("CurrentUserId is required.");
                throw new ForbiddenException("CurrentUserId is required.");
            }

            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(CurrentUserId.Value).ConfigureAwait(false);
            // for creator and course teacher and super-admin and admin return if exists
            if (entityToReturn.CreatedBy == CurrentUserId || isSuperAdminOrAdmin || entityToReturn.CourseTeachers.Any(x => x.UserId == CurrentUserId))
            {
                return;
            }
            if (!entityToReturn.IsUpdate && entityToReturn.Status != CourseStatus.Published)
            {
                throw new EntityNotFoundException("The training could not be found.");
            }
            if (entityToReturn.GroupId.HasValue)
            {
                var hasAccess = await ValidateUserCanAccessGroupCourse(entityToReturn, CurrentUserId.Value).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException("User not allowed to access this training.");
                }
            }
        }

        /// <summary>
        /// Handel to populate live session retrieved entity
        /// </summary>
        /// <param name="entity">the instance of <see cref="LiveSession"/></param>
        /// <returns></returns>
        protected override async Task PopulateRetrievedEntity(Course entity)
        {
            var sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(predicate: p => p.CourseId == entity.Id && !p.IsDeleted,
                include: src => src.Include(x => x.Lessons.Where(x => !x.IsDeleted))
                                    .Include(x => x.User)).ConfigureAwait(false);
            entity.Sections = sections;
        }
        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="model">the instance of <see cref="CourseRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task<Course> UpdateAsync(string identity, CourseRequestModel model, Guid currentUserId)
        {
            return await ExecuteWithResultAsync<Course>(async () =>
            {
                var existing = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                var currentTimeStamp = DateTime.UtcNow;

                existing.Group = null;

                existing.Id = existing.Id;
                existing.Name = model.Name;
                existing.Language = model.Language;
                existing.GroupId = model.GroupId;
                existing.LevelId = model.LevelId;
                existing.Duration = model.Duration;
                existing.Description = model.Description;
                existing.ThumbnailUrl = model.ThumbnailUrl;
                existing.UpdatedBy = currentUserId;
                existing.UpdatedOn = currentTimeStamp;

                var newCourseTags = new List<CourseTag>();

                foreach (var tagId in model.TagIds)
                {
                    newCourseTags.Add(new CourseTag
                    {
                        Id = Guid.NewGuid(),
                        TagId = tagId,
                        CourseId = existing.Id,
                        CreatedOn = currentTimeStamp,
                        CreatedBy = currentUserId,
                        UpdatedOn = currentTimeStamp,
                        UpdatedBy = currentUserId,
                    });
                }

                if (existing.CourseTags.Count > 0)
                {
                    _unitOfWork.GetRepository<CourseTag>().Delete(existing.CourseTags);
                }
                if (newCourseTags.Count > 0)
                {
                    await _unitOfWork.GetRepository<CourseTag>().InsertAsync(newCourseTags).ConfigureAwait(false);
                }
                _unitOfWork.GetRepository<Course>().Update(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return await GetByIdOrSlugAsync(identity, currentUserId).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Handle to change course status
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="status">the training status</param>
        /// <param name="currentUserId">the current id</param>
        /// <returns></returns>
        public async Task ChangeStatusAsync(string identity, CourseStatus status, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course.Status == status)
            {
                _logger.LogWarning("Training with id : {courseId} cannot be changed to same status by User with id {userId}.", course.Id, currentUserId);
                throw new ForbiddenException("Training cannot be changed to same status.");
            }

            if ((course.Status == CourseStatus.Draft && (status == CourseStatus.Published || status == CourseStatus.Rejected))
                || (course.Status == CourseStatus.Published && (status == CourseStatus.Review || status == CourseStatus.Rejected))
                || (course.Status == CourseStatus.Rejected && status == CourseStatus.Published)
                || (course.Status != CourseStatus.Published && status == CourseStatus.Completed))
            {
                _logger.LogWarning("Training with id: {id} cannot be changed from {status} status to {changeStatus} status.", course.Id, course.Status, status);
                throw new ForbiddenException($"Training with status: {course.Status} cannot be changed to {status} status.");
            }

            var isSuperAdminOrAdminAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
            if (!isSuperAdminOrAdminAccess && (status == CourseStatus.Published || status == CourseStatus.Rejected))
            {
                _logger.LogWarning("User with id: {userId} is unauthorized user to change training with id: {id} status from {status} to {changeStatus}.",
                    currentUserId, course.Id, course.Status, status);
                throw new ForbiddenException($"Unauthorized user to change training status to {status}.");
            }

            var sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(
                predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
            var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(
               predicate: p => p.CourseId == course.Id).ConfigureAwait(false);

            var currentTimeStamp = DateTime.UtcNow;

            if (course.IsUpdate)
            {
                sections = sections.Where(x => x.Status != CourseStatus.Published).ToList();
                lessons = lessons.Where(x => x.Status != CourseStatus.Published).ToList();
            }

            course.Status = status;
            course.UpdatedBy = currentUserId;
            course.UpdatedOn = currentTimeStamp;

            sections.ForEach(x =>
            {
                x.Status = status;
                x.UpdatedBy = currentUserId;
                x.UpdatedOn = currentTimeStamp;
            });
            lessons.ForEach(x =>
            {
                x.Status = status;
                x.UpdatedBy = currentUserId;
                x.UpdatedOn = currentTimeStamp;
            });

            _unitOfWork.GetRepository<Section>().Update(sections);
            _unitOfWork.GetRepository<Lesson>().Update(lessons);
            _unitOfWork.GetRepository<Course>().Update(course);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to update course status
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task UpdateCourseStatusAsync(string identity, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("Training with identity: {identity} not found for user with id: {userId}.", identity, currentUserId);
                throw new ForbiddenException("Training not found.");
            }
            course.IsUpdate = true;
            course.Status = CourseStatus.Draft;
            course.UpdatedBy = currentUserId;
            course.UpdatedOn = DateTime.UtcNow;

            _unitOfWork.GetRepository<Course>().Update(course);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Course Enrollment
        /// </summary>
        /// <param name="identity"> course id or slug</param>
        /// <param name="userId"> the user id</param>
        public async Task EnrollmentAsync(string identity, Guid userId)
        {
            await ExecuteAsync(async () =>
            {
                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                    predicate: x => x.Id == userId,
                    include: p => p.Include(x => x.CourseEnrollments)).ConfigureAwait(false);
                CommonHelper.CheckFoundEntity(user);

                var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(
                    predicate: x => x.Id.ToString() == identity || x.Slug == identity,
                    include: src => src.Include(x => x.User)
                    ).ConfigureAwait(false);
                CommonHelper.CheckFoundEntity(course);

                if (course.Status == CourseStatus.Completed)
                {
                    _logger.LogWarning("Training with id :{id} is in {status} status for enrollment.", course.Id, course.Status);
                    throw new ForbiddenException($"Cannot enrolled in the training having {course.Status} status.");
                }

                var existCourseEnrollment = await _unitOfWork.GetRepository<CourseEnrollment>().ExistsAsync(
                            p => p.CourseId == course.Id && p.UserId == userId && !p.IsDeleted
                             && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)
                                ).ConfigureAwait(false);

                if (existCourseEnrollment)
                {
                    _logger.LogWarning("User with userId: {userId} is already enrolled in the training with id: {courseId}.", userId, course.Id);
                    throw new ForbiddenException("You are already enrolled in this training.");
                }

                var currentTimeStamp = DateTime.UtcNow;
                CourseEnrollment courseEnrollment = new()
                {
                    Id = Guid.NewGuid(),
                    CourseId = course.Id,
                    EnrollmentDate = currentTimeStamp,
                    CreatedBy = userId,
                    CreatedOn = currentTimeStamp,
                    UserId = userId,
                    UpdatedBy = userId,
                    UpdatedOn = currentTimeStamp,
                    EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Enrolled,
                };

                await _unitOfWork.GetRepository<CourseEnrollment>().InsertAsync(courseEnrollment).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Handle to delete course
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the task complete</returns>
        public async Task DeleteCourseAsync(string identity, Guid currentUserId)
        {
            await ExecuteAsync(async () =>
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Training with identity : {identity} not found for user with id : {currentUserId}.", identity, currentUserId);
                    throw new EntityNotFoundException("Training not found.");
                }
                if (course.Status != CourseStatus.Draft)
                {
                    _logger.LogWarning("Training with identity : {identity} is in {status} status. So, it cannot be removed.", identity, course.Status);
                    throw new EntityNotFoundException("Training with draft status is only allowed to removed.");
                }
                if (course.CourseEnrollments.Count > 0)
                {
                    _logger.LogWarning("Training with identity : {identity} contains enrollments.", identity);
                    throw new EntityNotFoundException("Training contains member enrollments. So, it cannot be removed.");
                }

                var sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
                var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.CourseId == course.Id).ConfigureAwait(false);

                _unitOfWork.GetRepository<Section>().Delete(sections);
                _unitOfWork.GetRepository<Lesson>().Delete(lessons);
                _unitOfWork.GetRepository<CourseTag>().Delete(course.CourseTags);
                _unitOfWork.GetRepository<CourseTeacher>().Delete(course.CourseTeachers);
                _unitOfWork.GetRepository<Course>().Delete(course);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Handle to get user enrollment status in course
        /// </summary>
        /// <param name="id">the training id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public CourseEnrollmentStatus GetUserCourseEnrollmentStatus(Course course, Guid currentUserId)
        {
            CourseEnrollmentStatus userStatus = CourseEnrollmentStatus.NotEnrolled;
            if (course.CreatedBy == currentUserId)
            {
                return CourseEnrollmentStatus.Author;
            }
            else if (course.CourseTeachers.Any(p => p.UserId == currentUserId))
            {
                return CourseEnrollmentStatus.Teacher;
            }
            var enrolledMember = course.CourseEnrollments?.FirstOrDefault(p => p.UserId == currentUserId && !p.IsDeleted);

            if (enrolledMember != null)
            {
                switch (enrolledMember.EnrollmentMemberStatus)
                {
                    case EnrollmentMemberStatusEnum.Enrolled:
                        userStatus = CourseEnrollmentStatus.Enrolled;
                        break;
                    case EnrollmentMemberStatusEnum.Unenrolled:
                        userStatus = CourseEnrollmentStatus.NotEnrolled;
                        break;
                    case EnrollmentMemberStatusEnum.Completed:
                        userStatus = CourseEnrollmentStatus.Enrolled;
                        break;
                }
            }
            return userStatus;
        }

        /// <summary>
        /// Handle to get course detail
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="CourseResponseModel"/></returns>
        public async Task<CourseResponseModel> GetDetailAsync(string identity, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: false).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Training with identity : {identity} not found for user with id : {currentUserId}.", identity, currentUserId);
                    throw new EntityNotFoundException("Training not found.");
                }
                course.Sections = new List<Section>();
                course.Sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id && !p.IsDeleted,
                    include: src => src.Include(x => x.Lessons)).ConfigureAwait(false);

                course.CourseTags = new List<CourseTag>();
                course.CourseTags = await _unitOfWork.GetRepository<CourseTag>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id,
                    include: src => src.Include(x => x.Tag)).ConfigureAwait(false);

                var response = new CourseResponseModel
                {
                    Id = course.Id,
                    Slug = course.Slug,
                    Name = course.Name,
                    ThumbnailUrl = course.ThumbnailUrl,
                    Description = course.Description,
                    Duration = course.Duration,
                    Language = course.Language,
                    LevelId = course.LevelId,
                    GroupId = course.GroupId,
                    User = course.User != null ? new UserModel(course.User) : new UserModel(),
                    Status = course.Status,
                    UserStatus = GetUserCourseEnrollmentStatus(course, currentUserId),
                    Sections = new List<SectionResponseModel>(),
                    Tags = new List<CourseTagResponseModel>()
                };
                course.CourseTags.ToList().ForEach(item => response.Tags.Add(new CourseTagResponseModel(item)));

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (course.CreatedBy != currentUserId && !course.CourseTeachers.Any(x => x.UserId == currentUserId) && !isSuperAdminOrAdmin)
                {
                    course.Sections = course.Sections.Where(x => x.Status == CourseStatus.Published || x.Status == CourseStatus.Completed).ToList();
                    course.Sections.ForEach(x => x.Lessons = x.Lessons.Where(x => x.Status == CourseStatus.Published || x.Status == CourseStatus.Completed).ToList());
                }
                if (course.Sections.Count == 0)
                {
                    return response;
                }
                var currentUserWatchHistories = await _unitOfWork.GetRepository<WatchHistory>().GetAllAsync(
                    predicate: x => currentUserId != default && x.UserId == currentUserId && x.CourseId == course.Id).ConfigureAwait(false);

                response.Sections = course.Sections.Select(x => new SectionResponseModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    Order = x.Order,
                    Description = x.Description,
                    CourseId = x.CourseId,
                    Duration = x.Duration,
                    Lessons = x.Lessons.Select(l => new LessonResponseModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Slug = l.Slug,
                        Order = l.Order,
                        CourseId = l.CourseId,
                        SectionId = l.SectionId,
                        QuestionSetId = l.QuestionSetId,
                        MeetingId = l.MeetingId,
                        VideoUrl = l.VideoUrl,
                        DocumentUrl = l.DocumentUrl,
                        ThumbnailUrl = l.ThumbnailUrl,
                        Description = l.Description,
                        Type = l.Type,
                        Status = l.Status,
                        Duration = l.Duration,
                        IsMandatory = l.IsMandatory,
                        QuestionSet = l.QuestionSet != null ? new QuestionSetResponseModel(l.QuestionSet) : null,
                        Meeting = l.Meeting != null ? new MeetingResponseModel(l.Meeting) : null,
                        IsCompleted = currentUserWatchHistories.Any(h => h.LessonId == h.LessonId && h.IsCompleted),
                        IsPassed = currentUserWatchHistories.Any(h => h.LessonId == h.LessonId && h.IsPassed),
                    }).OrderBy(x => x.Order).ToList(),
                }).OrderBy(x => x.Order).ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch training detail.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to fetch training detail.");
            }
        }

        /// <summary>
        /// Handle to search group courses
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated search result</returns>
        public async Task<SearchResult<Course>> GroupCourseSearchAsync(string identity, BaseSearchCriteria criteria)
        {
            var predicate = PredicateBuilder.New<Course>(true);
            var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(
                predicate: p => p.Id.ToString() == identity || p.Slug == identity).ConfigureAwait(false);
            if (group == null)
            {
                _logger.LogWarning("Group with identity: {identity} not found.", identity);
                throw new EntityNotFoundException("Group not found.");
            }

            var userAccess = await ValidateUserCanAccessGroup(group.Id, criteria.CurrentUserId).ConfigureAwait(false);
            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(criteria.CurrentUserId).ConfigureAwait(false);
            if (!userAccess && !isSuperAdminOrAdmin)
            {
                _logger.LogWarning("User with id: {userId} is not authorized user to access the group with id: {groupId}.", criteria.CurrentUserId, group.Id);
                throw new ForbiddenException("User can't access the group.");
            }

            predicate = GroupCourseSearchPredicate(group.Id, predicate, criteria);
            var course = await _unitOfWork.GetRepository<Course>().GetAllAsync(
                predicate: predicate,
                include: src => src.Include(x => x.CourseTeachers)
                                .Include(x => x.CourseTags)
                                .Include(x => x.CourseEnrollments)
                ).ConfigureAwait(false);
            var result = course.ToIPagedList(criteria.Page, criteria.Size);
            return result;
        }

        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Course group search predicate
        /// </summary>
        /// <param name="groupId">the group id</param>
        /// <param name="predicate">the training predicate expression</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns></returns>
        private static Expression<Func<Course, bool>> GroupCourseSearchPredicate(Guid groupId, Expression<Func<Course, bool>> predicate, BaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search)
                 || x.Description.ToLower().Trim().Contains(search) || x.User.LastName.ToLower().Trim().Contains(search)
                 || x.User.FirstName.ToLower().Trim().Contains(search));
            }
            predicate = predicate.And(p => p.GroupId == groupId);
            predicate = predicate.And(p => p.Status == CourseStatus.Published || p.Status == CourseStatus.Completed || p.IsUpdate);
            return predicate;
        }

        #endregion Private Methods

        #region Statistics

        /// <summary>
        /// Handle to get course statistics 
        /// </summary>
        /// <param name="identity"> the training id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="CourseStatisticsResponseModel" /> . </returns>
        public async Task<CourseStatisticsResponseModel> GetCourseStatisticsAsync(string identity, Guid currentUserId)
        {
            try
            {
                var response = new CourseStatisticsResponseModel();
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.CourseId == course.Id &&
                !p.IsDeleted && (p.Status == CourseStatus.Published || p.Status == CourseStatus.Completed)).ConfigureAwait(false);
                response.TotalTeachers = course.CourseTeachers.Count;
                response.TotalLessons = lessons.Count;
                response.TotalMeetings = lessons.Count(x => (x.Type == LessonType.LiveClass || x.Type == LessonType.RecordedVideo) && x.MeetingId != null);
                response.TotalLectures = lessons.Count(x => x.Type == LessonType.Video || x.Type == LessonType.RecordedVideo);
                response.TotalExams = lessons.Count(x => x.Type == LessonType.Exam);
                response.TotalAssignments = lessons.Count(x => x.Type == LessonType.Assignment);
                response.TotalDocuments = lessons.Count(x => x.Type == LessonType.Document);
                response.TotalEnrollments = await _unitOfWork.GetRepository<CourseEnrollment>().CountAsync(predicate: p => p.CourseId == course.Id &&
                (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch training statistics.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to fetch training statistics.");
            }
        }

        /// <summary>
        /// Handle to fetch course lesson statistics
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="currentUserId">the current user id or slug</param>
        /// <returns></returns>
        public async Task<IList<LessonStatisticsResponseModel>> LessonStatistics(string identity, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id && !p.IsDeleted && !p.Section.IsDeleted,
                    include: src => src.Include(x => x.Section)
                    ).ConfigureAwait(false);

                var response = new List<LessonStatisticsResponseModel>();

                lessons.OrderBy(o => o.Section.Order).ThenBy(o => o.Order).ForEach(x => response.Add(new LessonStatisticsResponseModel
                {
                    Id = x.Id,
                    Slug = x.Slug,
                    Name = x.Name,
                    CourseId = course.Id,
                    CourseSlug = course.Slug,
                    CourseName = course.Name,
                    SectionId = x.SectionId,
                    SectionSlug = x.Section?.Slug,
                    SectionName = x.Section?.Name,
                    IsMandatory = x.IsMandatory,
                    EnrolledStudent = course.CourseEnrollments.Count,
                    LessonWatched = _unitOfWork.GetRepository<WatchHistory>().CountAsync(
                                        predicate: p => p.LessonId == x.Id && p.IsCompleted
                                            && course.CourseEnrollments.Select(x => x.UserId).Contains(p.UserId)).Result
                }));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch training lesson statistics.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to fetch training lesson statistics.");
            }
        }

        /// <summary>
        /// Handle to get lesson students report
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated data</returns>
        public async Task<SearchResult<LessonStudentResponseModel>> LessonStudentsReport(string identity, string lessonIdentity, BaseSearchCriteria criteria)
        {
            var course = await ValidateAndGetCourse(criteria.CurrentUserId, identity, validateForModify: true).ConfigureAwait(false);
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id && (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity),
                include: src => src.Include(x => x.Section)
                ).ConfigureAwait(false);

            var students = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(
                predicate: p => p.CourseId == course.Id,
                include: src => src.Include(x => x.User)
                ).ConfigureAwait(false);

            var watchHistories = await _unitOfWork.GetRepository<WatchHistory>().GetAllAsync(
                predicate: p => p.CourseId == course.Id && p.LessonId == lesson.Id,
                include: src => src.Include(x => x.User)
                ).ConfigureAwait(false);

            var data = from student in students
                       join history in watchHistories on student.UserId equals history.UserId
                       into studentHistory
                       from m in studentHistory.DefaultIfEmpty()
                       select new LessonStudentResponseModel
                       {
                           User = new UserModel(student.User),
                           LessonId = lesson.Id,
                           LessonSlug = lesson.Slug,
                           LessonName = lesson.Name,
                           LessonType = lesson.Type,
                           QuestionSetId = lesson.Type == LessonType.Exam ? lesson.QuestionSetId : null,
                           IsCompleted = m?.IsCompleted,
                           IsPassed = m?.IsPassed,
                           UpdatedOn = m?.UpdatedOn ?? m?.CreatedOn,
                       };

            return data.ToList().ToIPagedList(criteria.Page, criteria.Size);
        }

        /// <summary>
        /// Handle to fetch student course statistics report
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the search result</returns>
        public async Task<SearchResult<StudentCourseStatisticsResponseModel>> StudentStatistics(string identity, Guid currentUserId, BaseSearchCriteria criteria)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);

                var predicate = PredicateBuilder.New<CourseEnrollment>(true);
                predicate = predicate.And(p => p.CourseId == course.Id);
                predicate = predicate.And(p => p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed);

                if (!string.IsNullOrWhiteSpace(criteria.Search))
                {
                    var search = criteria.Search.ToLower().Trim();
                    predicate = predicate.And(x => x.User.LastName.ToLower().Trim().Contains(search) || x.User.FirstName.ToLower().Trim().Contains(search));
                }

                var enrollments = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(
                predicate: predicate,
                include: src => src.Include(x => x.Lesson).Include(x => x.User)).ConfigureAwait(false);

                var searchResult = enrollments.ToIPagedList(criteria.Page, criteria.Size);

                var response = new SearchResult<StudentCourseStatisticsResponseModel>
                {
                    Items = new List<StudentCourseStatisticsResponseModel>(),
                    CurrentPage = searchResult.CurrentPage,
                    PageSize = searchResult.PageSize,
                    TotalCount = searchResult.TotalCount,
                    TotalPage = searchResult.TotalPage,
                };

                searchResult.Items.ForEach(p =>
                     response.Items.Add(new StudentCourseStatisticsResponseModel
                     {
                         UserId = p.UserId,
                         FullName = p.User?.FullName,
                         ImageUrl = p.User?.ImageUrl,
                         LessonId = p.CurrentLessonId,
                         LessonSlug = p.Lesson?.Slug,
                         LessonName = p.Lesson?.Name,
                         Percentage = p.Percentage,
                         HasCertificateIssued = p.HasCertificateIssued,
                         CertificateIssuedDate = p.CertificateIssuedDate,
                         CertificateUrl = p.CertificateUrl
                     }));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch training student statistics.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to fetch training student statistics.");
            }
        }

        /// <summary>
        /// Handle to get student lessons detail
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="userId">the student id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the list of <see cref="LessonStudentResponseModel"/></returns>
        public async Task<IList<LessonStudentResponseModel>> StudentLessonsDetail(string identity, Guid userId, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            var watchHistories = await _unitOfWork.GetRepository<WatchHistory>().GetAllAsync(
                predicate: p => p.CourseId == course.Id && p.UserId == userId,
                include: src => src.Include(x => x.User).Include(x => x.Lesson.Section)
                ).ConfigureAwait(false);
            watchHistories = watchHistories.OrderBy(x => x.Lesson.Section.Order).ThenBy(x => x.Lesson.Order).ToList();
            var response = new List<LessonStudentResponseModel>();
            watchHistories.ForEach(x => response.Add(new LessonStudentResponseModel
            {
                LessonId = x.LessonId,
                LessonSlug = x.Lesson?.Slug,
                LessonName = x.Lesson?.Name,
                LessonType = x.Lesson.Type,
                QuestionSetId = x.Lesson.Type == LessonType.Exam ? x.Lesson?.QuestionSetId : null,
                IsCompleted = x.IsCompleted,
                IsPassed = x.IsPassed,
                UpdatedOn = x.UpdatedOn ?? x.CreatedOn,
                User = new UserModel(x.User)
            }));
            return response;
        }

        #endregion Statistics

        #region Dashboard

        /// <summary>
        /// Handle to get dashboard stats
        /// </summary>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <param name="currentUserRole">the current logged in user role</param>
        /// <returns>the instance of <see cref="DashboardResponseModel"/></returns>
        public async Task<DashboardResponseModel> GetDashboardStats(Guid currentUserId, UserRole currentUserRole)
        {
            var responseModel = new DashboardResponseModel();
            if (currentUserRole == UserRole.SuperAdmin || currentUserRole == UserRole.Admin)
            {
                var users = await _unitOfWork.GetRepository<User>().GetAllAsync().ConfigureAwait(false);

                responseModel.TotalUsers = users.Count();
                responseModel.TotalActiveUsers = users.Count(predicate: p => p.IsActive);
                responseModel.TotalTrainers = users.Count(predicate: p => p.IsActive && p.Role == UserRole.Trainer);

                responseModel.TotalGroups = await _unitOfWork.GetRepository<Group>().CountAsync(predicate: p => p.IsActive).ConfigureAwait(false);
                responseModel.TotalTrainings = await _unitOfWork.GetRepository<Course>().CountAsync(
                    predicate: p => p.Status == CourseStatus.Published || p.Status == CourseStatus.Completed || p.IsUpdate).ConfigureAwait(false);
            }
            if (currentUserRole == UserRole.Trainer)
            {
                responseModel.TotalGroups = await _unitOfWork.GetRepository<Group>().CountAsync(
                    predicate: p => p.GroupMembers.Any(x => x.UserId == currentUserId) && p.IsActive
                    ).ConfigureAwait(false);

                var trainings = await _unitOfWork.GetRepository<Course>().GetAllAsync(
                    predicate: p => p.CourseTeachers.Any(x => x.UserId == currentUserId)
                    ).ConfigureAwait(false);

                responseModel.TotalActiveTrainings = trainings.Count(predicate: p => p.Status == CourseStatus.Published || p.IsUpdate);
                responseModel.TotalCompletedTrainings = trainings.Count(predicate: p => p.Status == CourseStatus.Completed);
            }
            if (currentUserRole == UserRole.Trainee)
            {
                var trainings = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(
                   predicate: p => p.UserId == currentUserId && !p.IsDeleted
                   ).ConfigureAwait(false);

                responseModel.TotalEnrolledCourses = trainings.Count(predicate: p => p.EnrollmentMemberStatus != EnrollmentMemberStatusEnum.Unenrolled);
                responseModel.TotalInProgressCourses = trainings.Count(predicate: p => p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled);
                responseModel.TotalCompletedCourses = trainings.Count(predicate: p => p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed);
            }
            return responseModel;
        }

        /// <summary>
        /// Handle to get dashboard courses
        /// </summary>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <param name="currentUserRole">the current logged in user role</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the search result of <see cref="DashboardResponseModel"/></returns>
        public async Task<SearchResult<DashboardCourseResponseModel>> GetDashboardCourses(Guid currentUserId, UserRole currentUserRole, BaseSearchCriteria criteria)
        {
            var predicate = PredicateBuilder.New<Course>(true);

            if (currentUserRole == UserRole.SuperAdmin || currentUserRole == UserRole.Admin || currentUserRole == UserRole.Trainer)
            {
                predicate = predicate.And(p => p.CourseTeachers.Any(x => x.CourseId == p.Id && x.UserId == currentUserId));
            }

            if (currentUserRole == UserRole.Trainee)
            {
                predicate = predicate.And(p => p.CourseEnrollments.Any(x => x.CourseId == p.Id && x.UserId == currentUserId && !x.IsDeleted &&
                            (x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)));
            }

            var courses = await _unitOfWork.GetRepository<Course>().GetAllAsync(
                predicate: predicate,
                include: src => src.Include(x => x.User).Include(x => x.CourseEnrollments).ThenInclude(x => x.User)
                ).ConfigureAwait(false);

            var result = courses.ToIPagedList(criteria.Page, criteria.Size);
            var response = new SearchResult<DashboardCourseResponseModel>
            {
                Items = new List<DashboardCourseResponseModel>(),
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPage = result.TotalPage,
            };

            if (currentUserRole == UserRole.SuperAdmin || currentUserRole == UserRole.Admin || currentUserRole == UserRole.Trainer)
            {
                result.Items.ForEach(x =>
                {
                    var courseEnrollments = x.CourseEnrollments.Where(
                                    p => !p.IsDeleted
                                    && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled
                                        || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed));

                    var students = new List<UserModel>();
                    courseEnrollments.ForEach(p => students.Add(new UserModel(p.User)));

                    response.Items.Add(new DashboardCourseResponseModel
                    {
                        Id = x.Id,
                        Slug = x.Slug,
                        Name = x.Name,
                        ThumbnailUrl = x.ThumbnailUrl,
                        Students = students,
                        User = new UserModel(x.User),
                    });
                });
            }

            if (currentUserRole == UserRole.Trainee)
            {
                result.Items.ForEach(x =>
                {
                    response.Items.Add(new DashboardCourseResponseModel
                    {
                        Id = x.Id,
                        Slug = x.Slug,
                        Name = x.Name,
                        ThumbnailUrl = x.ThumbnailUrl,
                        Percentage = x.CourseEnrollments.FirstOrDefault(
                                    predicate: p => p.UserId == currentUserId && !p.IsDeleted
                                    && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled
                                        || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed))?.Percentage,

                        User = new UserModel(x.User),
                    });
                });
            }
            return response;
        }
        #endregion Dashboard

        #region Certificate
        /// <summary>
        /// Handle to issue the certificate
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="model">the instance of <see cref="CertificateIssueRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the list of <see cref="CourseCertificateIssuedResponseModel"/></returns>
        public async Task<IList<CourseCertificateIssuedResponseModel>> IssueCertificateAsync(string identity, CertificateIssueRequestModel model, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                course.Signatures = new List<Signature>();
                course.Signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (course.Signatures.Count == 0)
                {
                    _logger.LogWarning("At least one trainer signature detail is required for training with id :{courseId}.", course.Id);
                    throw new EntityNotFoundException("At least one trainer signature detail is required.");
                }
                course.CourseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                     predicate: p => p.CourseId == course.Id
                     ).ConfigureAwait(false);

                if (course.CourseCertificate == null)
                {
                    _logger.LogWarning("Certificate detail information not found for training with id :{courseId}.", course.Id);
                    throw new EntityNotFoundException("Certificate detail information not found.");
                }

                var predicate = PredicateBuilder.New<CourseEnrollment>(true);
                predicate = predicate.And(p => p.CourseId == course.Id && !p.IsDeleted && p.EnrollmentMemberStatus != EnrollmentMemberStatusEnum.Unenrolled);
                if (!model.IssueAll)
                {
                    predicate = predicate.And(p => model.UserIds.Contains(p.UserId));
                }
                predicate = predicate.And(p => p.HasCertificateIssued != true);

                var results = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(
                    predicate: predicate,
                    include: src => src.Include(x => x.User)
                    ).ConfigureAwait(false);

                var currentTimeStamp = DateTime.UtcNow;
                var response = new List<CourseCertificateIssuedResponseModel>();
                foreach (var item in results)
                {
                    item.CertificateUrl = await GetImageFile(course.CourseCertificate, item.User.FullName, course.Signatures).ConfigureAwait(false);
                    item.CertificateIssuedDate = currentTimeStamp;
                    item.HasCertificateIssued = true;
                    response.Add(new CourseCertificateIssuedResponseModel
                    {
                        CourseId = course.Id,
                        CourseName = course.Name,
                        CourseSlug = course.Slug,
                        CertificateIssuedDate = item.CertificateIssuedDate,
                        HasCertificateIssued = item.HasCertificateIssued,
                        CertificateUrl = item.CertificateUrl,
                        Percentage = item.Percentage,
                        User = new UserModel(item.User)
                    });
                }
                _unitOfWork.GetRepository<CourseEnrollment>().Update(results);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to issued the training certificate.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to issued the training certificate.");
            }
        }

        /// <summary>
        /// Create zoom meeting
        /// </summary>
        /// <param name="meetingName">the meeting name</param>
        /// <param name="duration">the duration</param>
        /// <param name="startDate">the start date</param>
        /// <param name="hostEmail">the host email</param>
        /// <returns>the meeting id and passcode and the instance of <see cref="ZoomLicense"/></returns>
        private async Task<string> GetImageFile(CourseCertificate? certificate, string fullName, IList<Signature> signatures)
        {
            var client = new RestClient($"{imageApi}");
            var authors = new ArrayList();
            foreach (var item in signatures)
            {
                authors.Add(new
                {
                    name = item.FullName,
                    position = item.Designation,
                    signatureUrl = item.FileUrl
                });
            }
            var request = new RestRequest().AddHeader("Accept", "application/json")
                    .AddJsonBody(new
                    {
                        name = fullName,
                        training = certificate?.Title,
                        startDate = certificate?.EventStartDate.ToShortDateString(),
                        endDate = certificate?.EventEndDate.ToShortDateString(),
                        authors,
                    });

            var response = await client.PostAsync(request).ConfigureAwait(false);
            var fileName = certificate?.Title ?? "certificate";
            MemoryStream ms = new(response.RawBytes);
            var file = new FormFile(ms, 0, response.RawBytes.Length, fileName, fileName);
            var fileResponse = await _mediaService.UploadFileAsync(new MediaRequestModel { File = file, Type = MediaType.Public }).ConfigureAwait(false);
            return fileResponse;
        }

        #endregion Certificate

        #region Signature

        /// <summary>
        /// Handle to get signature
        /// </summary>
        /// <param name="identity"> the training id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="SignatureResponseModel" /> . </returns>
        public async Task<IList<SignatureResponseModel>> GetAllSignatureAsync(string identity, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Training with identity: {identity} not found.", identity);
                    throw new EntityNotFoundException("Training not found.");
                }

                var signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(predicate: x => x.CourseId == course.Id).ConfigureAwait(false);
                var response = new List<SignatureResponseModel>();
                signatures.ForEach(x => response.Add(new SignatureResponseModel(x)));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying fetch the training signature.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying fetch the training signature.");
            }
        }

        /// <summary>
        /// Handle to upload signature
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="model">the instance of <see cref="SignatureRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="SignatureResponseModel"/></returns>
        public async Task<SignatureResponseModel> InsertSignatureAsync(string identity, SignatureRequestModel model, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Training with identity: {identity} not found.", identity);
                    throw new EntityNotFoundException("Training not found.");
                }
                var signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
                if (signatures.Count >= 3)
                {
                    _logger.LogWarning("training with id: {id} cannot have more than 3 signatures for user with id: {userId}.", course.Id, currentUserId);
                    throw new ForbiddenException("At most 3 signatures are only allowed.");
                }
                var currentTimeStamp = DateTime.UtcNow;
                var signature = new Signature
                {
                    Id = Guid.NewGuid(),
                    FullName = model.FullName,
                    Designation = model.Designation,
                    CourseId = course.Id,
                    FileUrl = model.FileURL,
                    CreatedOn = currentTimeStamp,
                    CreatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId
                };

                var courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (courseCertificate != null)
                {
                    var sampleSignatures = new List<Signature>();
                    sampleSignatures.AddRange(signatures);
                    sampleSignatures.Add(signature);

                    courseCertificate.SampleUrl = await GetImageFile(courseCertificate, "User Name", sampleSignatures).ConfigureAwait(false);
                    courseCertificate.UpdatedBy = currentUserId;
                    courseCertificate.UpdatedOn = currentTimeStamp;

                    _unitOfWork.GetRepository<CourseCertificate>().Update(courseCertificate);
                }
                await _unitOfWork.GetRepository<Signature>().InsertAsync(signature).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                var response = new SignatureResponseModel(signature);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to upload signature in the training.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to upload signature in the training.");
            }
        }

        /// <summary>
        /// Handle to update signature
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="id">the signature id</param>
        /// <param name="model">the instance of <see cref="SignatureRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="SignatureResponseModel"/></returns>
        public async Task<SignatureResponseModel> UpdateSignatureAsync(string identity, Guid id, SignatureRequestModel model, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Training with identity: {identity} not found.", identity);
                    throw new EntityNotFoundException("Training not found.");
                }
                var signature = await _unitOfWork.GetRepository<Signature>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id && p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (signature == null)
                {
                    _logger.LogWarning("Signature with id: {id} and trainingId : {courseId} not found.", id, course.Id);
                    throw new EntityNotFoundException("Signature not found.");
                }
                var currentTimeStamp = DateTime.UtcNow;

                signature.FullName = model.FullName;
                signature.Designation = model.Designation;
                signature.FileUrl = model.FileURL;
                signature.UpdatedOn = currentTimeStamp;
                signature.UpdatedBy = currentUserId;

                var courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (courseCertificate != null)
                {
                    var signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(
                        predicate: p => p.CourseId == course.Id && p.Id != id).ConfigureAwait(false);

                    var sampleSignatures = new List<Signature>();
                    sampleSignatures.AddRange(signatures);
                    sampleSignatures.Add(signature);

                    courseCertificate.SampleUrl = await GetImageFile(courseCertificate, "User Name", sampleSignatures).ConfigureAwait(false);
                    courseCertificate.UpdatedBy = currentUserId;
                    courseCertificate.UpdatedOn = currentTimeStamp;

                    _unitOfWork.GetRepository<CourseCertificate>().Update(courseCertificate);
                }

                _unitOfWork.GetRepository<Signature>().Update(signature);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                var response = new SignatureResponseModel(signature);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update signature in the training.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to update signature in the training.");
            }
        }

        /// <summary>
        /// Handle to delete signature
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="id">the signature id</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the task complete</returns>
        public async Task DeleteSignatureAsync(string identity, Guid id, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Training with identity: {identity} not found.", identity);
                    throw new EntityNotFoundException("Training not found.");
                }
                var signature = await _unitOfWork.GetRepository<Signature>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id && p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (signature == null)
                {
                    _logger.LogWarning("Signature with id: {id} and trainingId : {courseId} not found.", id, course.Id);
                    throw new EntityNotFoundException("Signature not found.");
                }

                var courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (courseCertificate != null)
                {
                    var signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(
                        predicate: p => p.CourseId == course.Id && p.Id != id).ConfigureAwait(false);

                    courseCertificate.SampleUrl = await GetImageFile(courseCertificate, "User Name", signatures).ConfigureAwait(false);
                    courseCertificate.UpdatedBy = currentUserId;
                    courseCertificate.UpdatedOn = DateTime.UtcNow;

                    _unitOfWork.GetRepository<CourseCertificate>().Update(courseCertificate);
                }

                _unitOfWork.GetRepository<Signature>().Delete(signature);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update signature in the training.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to update signature in the training.");
            }
        }

        /// <summary>
        /// Handle to insert course certificate detail
        /// </summary>
        /// <param name="identity">the training id or slug</param>
        /// <param name="model">the instance of <see cref="CourseCertificateRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task<CourseCertificateResponseModel> InsertCertificateDetail(string identity, CourseCertificateRequestModel model, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("Training with identity: {identity} not found.", identity);
                throw new EntityNotFoundException("Training not found.");
            }
            var signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(
                       predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var courseCertificate = new CourseCertificate();
            if (model.Id.HasValue)
            {
                courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == model.Id.Value
                    ).ConfigureAwait(false);
                courseCertificate.Title = model.Title;
                courseCertificate.EventStartDate = model.EventStartDate;
                courseCertificate.EventEndDate = model.EventEndDate;
                courseCertificate.UpdatedBy = currentUserId;
                courseCertificate.UpdatedOn = currentTimeStamp;
                courseCertificate.SampleUrl = await GetImageFile(courseCertificate, "User Name", signatures).ConfigureAwait(false);

                _unitOfWork.GetRepository<CourseCertificate>().Update(courseCertificate);
            }
            else
            {
                courseCertificate = new CourseCertificate
                {
                    Id = Guid.NewGuid(),
                    CourseId = course.Id,
                    Title = model.Title,
                    EventStartDate = model.EventStartDate,
                    EventEndDate = model.EventEndDate,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                };
                courseCertificate.SampleUrl = await GetImageFile(courseCertificate, "User Name", signatures).ConfigureAwait(false);
                await _unitOfWork.GetRepository<CourseCertificate>().InsertAsync(courseCertificate).ConfigureAwait(false);
            }
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return new CourseCertificateResponseModel(courseCertificate);
        }

        /// <summary>
        /// Handle to get certificate detail information
        /// </summary>
        /// <param name="identity">the training id or slug </param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task<CourseCertificateResponseModel?> GetCertificateDetailAsync(string identity, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("Training with identity: {identity} not found.", identity);
                throw new EntityNotFoundException("Training not found.");
            }
            var courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id
                ).ConfigureAwait(false);

            return courseCertificate == null ? null : new CourseCertificateResponseModel(courseCertificate);
        }

        #endregion Signature
    }
}