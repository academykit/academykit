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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Data;
    using System.Linq.Expressions;

    public class CourseService : BaseGenericService<Course, CourseBaseSearchCriteria>, ICourseService
    {
        public CourseService(IUnitOfWork unitOfWork, ILogger<CourseService> logger) : base(unitOfWork, logger)
        {
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
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search)
                 || x.Description.ToLower().Trim().Contains(search) || x.User.LastName.ToLower().Trim().Contains(search)
                 || x.User.FirstName.ToLower().Trim().Contains(search));
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
            Expression<Func<Course, bool>> groupPredicate = PredicateBuilder.New<Course>();
            if (isSuperAdminOrAdmin)
            {
                return predicate;
            }

            var groupIds = GetUserGroupIds(criteria.CurrentUserId).Result;
            groupPredicate = PredicateBuilder.New<Course>(x => x.GroupId.HasValue && groupIds.Contains(x.GroupId ?? Guid.Empty));
            groupPredicate = groupPredicate.And(predicate);
            predicate = predicate.And(x => !x.GroupId.HasValue).Or(groupPredicate);
            return predicate.And(x => x.CreatedBy == criteria.CurrentUserId
                        || (x.CreatedBy != criteria.CurrentUserId && (x.IsUpdate || x.Status.Equals(CourseStatus.Published))));

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
                        .Include(x => x.CourseTeachers);
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
                _logger.LogWarning("Course with id : {courseId} cannot be delete having status : {status}", course.Id, course.Status.ToString());
                throw new ForbiddenException("Only draft course can be delete");
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
                _logger.LogWarning("CurrentUserId is required");
                throw new ForbiddenException("CurrentUserId is required");
            }
            // for creator and course teacher return if exists
            if (entityToReturn.CreatedBy == CurrentUserId || entityToReturn.CourseTeachers.Any(x => x.UserId == CurrentUserId))
            {
                return;
            }
            if (!entityToReturn.IsUpdate && entityToReturn.Status != CourseStatus.Published)
            {
                throw new EntityNotFoundException("The course could not be found");
            }
            if (entityToReturn.GroupId.HasValue)
            {
                var hasAccess = await ValidateUserCanAccessGroupCourse(entityToReturn, CurrentUserId.Value).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException("User not allowed to access this course");
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

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CourseRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task<Course> UpdateAsync(string identity, CourseRequestModel model, Guid currentUserId)
        {
            var existing = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

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
        }

        /// <summary>
        /// Handle to change course status
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="status">the course status</param>
        /// <param name="currentUserId">the current id</param>
        /// <returns></returns>
        public async Task ChangeStatusAsync(string identity, CourseStatus status, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course.Status == status)
            {
                _logger.LogWarning("Course with id : {courseId} cannot be changed to same status by User with id {userId}", course.Id, currentUserId);
                throw new ForbiddenException("Course cannot be changed to same status");
            }

            if ((course.Status == CourseStatus.Draft && (status == CourseStatus.Published || status == CourseStatus.Rejected))
                || (course.Status == CourseStatus.Published && (status == CourseStatus.Review || status == CourseStatus.Rejected))
                || (course.Status == CourseStatus.Rejected && status == CourseStatus.Published))
            {
                _logger.LogWarning("Course with id: {id} cannot be changed from {status} status to {changeStatus} status", course.Id, course.Status, status);
                throw new ForbiddenException($"Course with status: {course.Status} cannot be changed to {status} status");
            }

            var isSuperAdminOrAdminAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
            if (!isSuperAdminOrAdminAccess && (status == CourseStatus.Published || status == CourseStatus.Rejected))
            {
                _logger.LogWarning("User with id: {userId} is unauthorized user to change course with id: {id} status from {status} to {changeStatus}",
                    currentUserId, course.Id, course.Status, status);
                throw new ForbiddenException($"Unauthorized user to change course status to {status}");
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
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task UpdateCourseStatusAsync(string identity, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("Course with identity: {identity} not found for user with id: {userId}", identity, currentUserId);
                throw new ForbiddenException("Course not found");
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

                var existCourseEnrollment = await _unitOfWork.GetRepository<CourseEnrollment>().ExistsAsync(
                            p => p.CourseId == course.Id && p.UserId == userId && !p.IsDeleted
                             && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)
                                ).ConfigureAwait(false);

                if (existCourseEnrollment)
                {
                    _logger.LogWarning("User with userId: {userId} is already enrolled in the course with id: {courseId}", userId, course.Id);
                    throw new ArgumentException("You are already enrolled in this course");
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
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the task complete</returns>
        public async Task DeleteCourseAsync(string identity, Guid currentUserId)
        {
            await ExecuteAsync(async () =>
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Course with identity : {identity} not found for user with id : {currentUserId}", identity, currentUserId);
                    throw new EntityNotFoundException("Course not found");
                }
                if (course.Status != CourseStatus.Draft)
                {
                    _logger.LogWarning("Course with identity : {identity} is in {status} status. So, it cannot be removed", identity, course.Status);
                    throw new EntityNotFoundException("Course with draft status is only allowed to removed.");
                }
                if (course.CourseEnrollments.Count > 0)
                {
                    _logger.LogWarning("Course with identity : {identity} contains enrollments", identity);
                    throw new EntityNotFoundException("Course contains member enrollments. So, it cannot be removed");
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
        /// <param name="id">the course id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<CourseEnrollmentStatus> GetUserCourseEnrollmentStatus(Course course, Guid currentUserId, bool fetchMembers = false)
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
            var enrolledMember = new CourseEnrollment();
            if (fetchMembers)
            {
                enrolledMember = await _unitOfWork.GetRepository<CourseEnrollment>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id && p.UserId == currentUserId && !p.IsDeleted
                            && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled
                                || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)).ConfigureAwait(false);
            }
            else
            {
                enrolledMember = course.CourseEnrollments.FirstOrDefault(p => p.UserId == currentUserId && !p.IsDeleted);
            }

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
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="CourseResponseModel"/></returns>
        public async Task<CourseResponseModel> GetDetailAsync(string identity, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: false).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Course with identity : {identity} not found for user with id : {currentUserId}", identity, currentUserId);
                    throw new EntityNotFoundException("Course not found");
                }
                course.Sections = new List<Section>();
                course.Sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id && !p.IsDeleted,
                    include: src => src.Include(x => x.Lessons)).ConfigureAwait(false);

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
                    UserStatus = GetUserCourseEnrollmentStatus(course, currentUserId, fetchMembers: true).Result,
                    Sections = new List<SectionResponseModel>(),
                };
                if (course.CreatedBy != currentUserId && !course.CourseTeachers.Any(x => x.UserId == currentUserId))
                {
                    course.Sections = course.Sections.Where(x => x.Status == CourseStatus.Published).ToList();
                    course.Sections.ForEach(x => x.Lessons = x.Lessons.Where(x => x.Status == CourseStatus.Published).ToList());
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
                        IsPreview = l.IsPreview,
                        IsMandatory = l.IsMandatory,
                        QuestionSet = l.QuestionSet != null ? new QuestionSetResponseModel(l.QuestionSet) : null,
                        Meeting = l.Meeting != null ? new MeetingResponseModel(l.Meeting) : null,
                        IsCompleted = currentUserWatchHistories.Any(h => h.LessonId == h.LessonId && h.IsCompleted),
                    }).OrderBy(x => x.Order).ToList(),
                }).OrderBy(x => x.Order).ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch course detail.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to fetch course detail.");
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
                _logger.LogWarning("Group with identity: {identity} not found", identity);
                throw new EntityNotFoundException("Group not found");
            }
            predicate = GroupCourseSearchPredicate(group.Id, predicate, criteria);
            var course = await _unitOfWork.GetRepository<Course>().GetAllAsync(
                predicate: predicate,
                include: src => src.Include(x => x.CourseTeachers)
                                .Include(x => x.CourseTags)
                ).ConfigureAwait(false);
            var result = course.ToIPagedList(criteria.Page, criteria.Size);

            await PopulateRetrievedEntities(result.Items).ConfigureAwait(false);
            return result;
        }

        #region Private Methods
        /// <summary>
        /// Course group search predicate
        /// </summary>
        /// <param name="groupId">the group id</param>
        /// <param name="predicate">the course predicate expression</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns></returns>
        private Expression<Func<Course, bool>> GroupCourseSearchPredicate(Guid groupId, Expression<Func<Course, bool>> predicate, BaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search)
                 || x.Description.ToLower().Trim().Contains(search) || x.User.LastName.ToLower().Trim().Contains(search)
                 || x.User.FirstName.ToLower().Trim().Contains(search));
            }
            predicate = predicate.And(p => p.GroupId == groupId);
            return predicate;
        }

        #endregion Private Methods

        #region Statistics

        /// <summary>
        /// Handle to fetch course lesson statistics
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current user id or slug</param>
        /// <returns></returns>
        public async Task<IList<LessonStatisticsResponseModel>> LessonStatistics(string identity, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id,
                    include: src => src.Include(x => x.Section)
                    ).ConfigureAwait(false);

                var response = new List<LessonStatisticsResponseModel>();

                lessons.ForEach(x => response.Add(new LessonStatisticsResponseModel
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
                    IsMandantory = x.IsMandatory,
                    EnrolledStudent = course.CourseEnrollments.Count,
                    LessonWatched = _unitOfWork.GetRepository<WatchHistory>().CountAsync(predicate: p => p.LessonId == x.Id && p.IsCompleted).Result
                }));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch course lesson statistics.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to fetch course lesson statistics.");
            }
        }

        /// <summary>
        /// Handle to fetch student course statistics report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        public async Task<IList<StudentCourseStatisticsResponseModel>> StudentStatistics(string identity, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);

                var enrollments = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(
                predicate: p => p.CourseId == course.Id
                    && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed),
                include: src => src.Include(x => x.Lesson).Include(x => x.User)).ConfigureAwait(false);

                var response = new List<StudentCourseStatisticsResponseModel>();

                enrollments.ForEach(x => response.Add(new StudentCourseStatisticsResponseModel
                {
                    UserId = x.UserId,
                    FullName = x.User?.FullName,
                    LessonId = x.CurrentLessonId,
                    LessonSlug = x.Lesson?.Slug,
                    LessonName = x.Lesson?.Name,
                    Percentage = x.Percentage
                }));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch course student statistics.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to fetch course student statistics.");
            }
        }

        #endregion Statistics
    }
}