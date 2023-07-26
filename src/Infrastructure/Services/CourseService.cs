namespace Lingtren.Infrastructure.Services
{
    using Amazon.S3.Model;
    using AngleSharp.Text;
    using Application.Common.Dtos;
    using Application.Common.Models.ResponseModels;
    using Hangfire;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using System;
    using System.Collections;
    using System.Collections.Immutable;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;

    public class CourseService : BaseGenericService<Course, CourseBaseSearchCriteria>, ICourseService 
    {
        private readonly string imageApi;
        private readonly IMediaService _mediaService;
        private readonly IFileServerService _fileServerService;
        public CourseService(
            IUnitOfWork unitOfWork,
            ILogger<CourseService> logger,
            IConfiguration configuration,
            IMediaService mediaService,
            IFileServerService fileServerService,
            IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {
            imageApi = configuration.GetSection("AppUrls:ImageApi").Value;
            _mediaService = mediaService;
            _fileServerService = fileServerService;
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
                            enrollmentStatusPredicate = enrollmentStatusPredicate.And(p => p.CourseEnrollments.Any(e => e.UserId == criteria.CurrentUserId));
                            break;
                        case CourseEnrollmentStatus.NotEnrolled:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.And(p => !p.CourseEnrollments.Any(e => e.UserId == criteria.CurrentUserId)).And(p=>p.CreatedBy != criteria.CurrentUserId);
                            break;
                        case CourseEnrollmentStatus.Author:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.And(p => p.CreatedBy == criteria.CurrentUserId);
                            break;
                        case CourseEnrollmentStatus.Teacher:
                            enrollmentStatusPredicate = enrollmentStatusPredicate.And(p => p.CourseTeachers.Any(e => e.UserId == criteria.CurrentUserId));
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
                throw new ForbiddenException(_localizer.GetString("OnlyDraftTrainingDeleted"));
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
                throw new ForbiddenException(_localizer.GetString("CurrentUserRequired"));
            }

            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(CurrentUserId.Value).ConfigureAwait(false);
            // for creator and course teacher and super-admin and admin return if exists
            if (entityToReturn.CreatedBy == CurrentUserId || isSuperAdminOrAdmin || entityToReturn.CourseTeachers.Any(x => x.UserId == CurrentUserId))
            {
                return;
            }
            if (!entityToReturn.IsUpdate && entityToReturn.Status != CourseStatus.Published)
            {
                throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            }
            if (entityToReturn.GroupId.HasValue)
            {
                var hasAccess = await ValidateUserCanAccessGroupCourse(entityToReturn, CurrentUserId.Value).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
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
                if(existing.Status == CourseStatus.Completed)
                {
                    throw new InvalidOperationException(_localizer.GetString("CompletedCourseIssue"));
                }
                var currentTimeStamp = DateTime.UtcNow;

                var imageKey = existing.ThumbnailUrl;

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

                if (imageKey != model.ThumbnailUrl && !string.IsNullOrWhiteSpace(imageKey))
                {
                    if (imageKey.ToLower().Trim().Contains("/public/") && imageKey.IndexOf("/standalone/") != -1)
                    {
                        imageKey = imageKey.Substring(imageKey.IndexOf("/standalone/") + "/standalone/".Length);
                    }
                    await _fileServerService.RemoveFileAsync(imageKey).ConfigureAwait(false);
                }

                return await GetByIdOrSlugAsync(identity, currentUserId).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Handle to change course status
        /// </summary>
        /// <param name="model">the instance of <see cref="CourseStatusRequestModel" /> .</param>
        /// <param name="currentUserId">the current id</param>
        /// <returns></returns>
        public async Task ChangeStatusAsync(CourseStatusRequestModel model, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, model.Identity, validateForModify: true).ConfigureAwait(false);

            if ((course.Status == CourseStatus.Draft && (model.Status == CourseStatus.Published || model.Status == CourseStatus.Rejected))
                || (course.Status == CourseStatus.Published && (model.Status == CourseStatus.Review || model.Status == CourseStatus.Rejected))
                || (course.Status == CourseStatus.Rejected && model.Status == CourseStatus.Published)
                || (course.Status != CourseStatus.Published && model.Status == CourseStatus.Completed))
            {
                _logger.LogWarning("Training with id: {id} cannot be changed from {status} status to {changeStatus} status.", course.Id, course.Status, model.Status);
                throw new ForbiddenException(_localizer.GetString("TrainingStatusCannotChanged"));
            }

            var isSuperAdminOrAdminAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
            if (!isSuperAdminOrAdminAccess && (model.Status == CourseStatus.Published || model.Status == CourseStatus.Rejected))
            {
                _logger.LogWarning("User with id: {userId} is unauthorized user to change training with id: {id} status from {status} to {changeStatus}.",
                    currentUserId, course.Id, course.Status, model.Status);
                throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
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

            course.Status = model.Status;
            course.UpdatedBy = currentUserId;
            course.UpdatedOn = currentTimeStamp;

            sections.ForEach(x =>
            {
                x.Status = model.Status;
                x.UpdatedBy = currentUserId;
                x.UpdatedOn = currentTimeStamp;
            });
            lessons.ForEach(x =>
            {
                x.Status = model.Status;
                x.UpdatedBy = currentUserId;
                x.UpdatedOn = currentTimeStamp;
            });

            _unitOfWork.GetRepository<Section>().Update(sections);
            _unitOfWork.GetRepository<Lesson>().Update(lessons);
            _unitOfWork.GetRepository<Course>().Update(course);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            if (model.Status == CourseStatus.Review)
            {
                BackgroundJob.Enqueue<IHangfireJobService>(job => job.SendCourseReviewMailAsync(course.Name, null));
            }

            if (model.Status == CourseStatus.Published)
            {
                if (course.CourseEnrollments.Count == default)
                {
                    BackgroundJob.Enqueue<IHangfireJobService>(job => job.GroupCoursePublishedMailAsync(course.GroupId.Value, course.Name,course.Slug, null));
                }
                else
                {
                    BackgroundJob.Enqueue<IHangfireJobService>(job => job.SendLessonAddedMailAsync(course.Name,course.Slug, null));
                }

            }

            if (model.Status == CourseStatus.Rejected)
            {
                BackgroundJob.Enqueue<IHangfireJobService>(job => job.CourseRejectedMailAsync(course.Id,model.Message, null));
            }
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
                throw new ForbiddenException(_localizer.GetString("TrainingNotFound"));
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
                    throw new ForbiddenException(_localizer.GetString("CannotEnrolledOnCompletedTraining"));
                }

                var existCourseEnrollment = await _unitOfWork.GetRepository<CourseEnrollment>().ExistsAsync(
                            p => p.CourseId == course.Id && p.UserId == userId && !p.IsDeleted
                             && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)
                                ).ConfigureAwait(false);

                if (existCourseEnrollment)
                {
                    _logger.LogWarning("User with userId: {userId} is already enrolled in the training with id: {courseId}.", userId, course.Id);
                    throw new ForbiddenException(_localizer.GetString("AlreadyEnrolledInTraining"));
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

                if (courseEnrollment.EnrollmentMemberStatus.Equals(EnrollmentMemberStatusEnum.Enrolled))
                {
                    BackgroundJob.Enqueue<IHangfireJobService>(job => job.SendCourseEnrollmentMailAsync(user.FullName,user.Email,course.Id, course.Name, null));
                }

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
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }
                if (course.Status != CourseStatus.Draft)
                {
                    _logger.LogWarning("Training with identity : {identity} is in {status} status. So, it cannot be removed.", identity, course.Status);
                    throw new EntityNotFoundException(_localizer.GetString("TrainingWithDraftgStatusCanOnlyRemoved"));
                }
                if (course.CourseEnrollments.Count > 0)
                {
                    _logger.LogWarning("Training with identity : {identity} contains enrollments.", identity);
                    throw new EntityNotFoundException(_localizer.GetString("EnrollmentFoundInTraining"));
                }

                var privateFiles = new List<string>();
                var publicFiles = new List<string>();

                var sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
                var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
                var lessonIds = lessons.Select(x => x.Id).ToList();

                var documentPaths = lessons.Select(x => x.DocumentUrl).ToList();
                documentPaths.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                var videoPaths = lessons.Select(x => x.DocumentUrl).ToList();
                videoPaths.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                privateFiles.AddRange(documentPaths);
                privateFiles.AddRange(videoPaths);

                var lessonThumbnails = lessons.Select(x => x.ThumbnailUrl).ToList();
                lessonThumbnails.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                publicFiles.Add(course.ThumbnailUrl);
                publicFiles.AddRange(lessonThumbnails);

                var assignments = await _unitOfWork.GetRepository<Assignment>().GetAllAsync(
                    predicate: p => lessonIds.Contains(p.LessonId),
                    include: src => src.Include(x => x.AssignmentQuestionOptions)
                                .Include(x => x.AssignmentAttachments)).ConfigureAwait(false);

                if (assignments.Count > 0)
                {
                    var assignmentQuestionOptions = assignments.SelectMany(x => x.AssignmentQuestionOptions);
                    var assignmentAttachments = assignments.SelectMany(x => x.AssignmentAttachments);

                    _unitOfWork.GetRepository<AssignmentQuestionOption>().Delete(assignmentQuestionOptions);
                    _unitOfWork.GetRepository<AssignmentAttachment>().Delete(assignmentAttachments);
                    _unitOfWork.GetRepository<Assignment>().Delete(assignments);
                }

                _unitOfWork.GetRepository<Lesson>().Delete(lessons);
                _unitOfWork.GetRepository<Section>().Delete(sections);
                _unitOfWork.GetRepository<CourseTag>().Delete(course.CourseTags);
                _unitOfWork.GetRepository<CourseTeacher>().Delete(course.CourseTeachers);
                _unitOfWork.GetRepository<Course>().Delete(course);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                // Process to remove the file associated with course and lessons
                foreach (var item in publicFiles)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var key = item;
                        if (item.ToLower().Trim().Contains("/public/") && item.IndexOf("/standalone/") != -1)
                        {
                            key = item.Substring(item.IndexOf("/standalone/") + "/standalone/".Length);
                        }
                        await _fileServerService.RemoveFileAsync(key).ConfigureAwait(false);
                    }
                }

                foreach (var item in privateFiles)
                {
                    await _fileServerService.RemoveFileAsync(item).ConfigureAwait(false);
                }
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
            else if (course.CourseTeachers.Any(p => p.UserId == currentUserId) && !course.CourseEnrollments.Any(p=>p.UserId == currentUserId))
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
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
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
                        StartDate = l.StartDate,
                        EndDate = l.EndDate,
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
                        QuestionSet = l.Type == LessonType.Exam ? new QuestionSetResponseModel(_unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefault(predicate: x => x.Id == l.QuestionSetId)) : null,
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
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFetchTrainingDetail"));
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
                throw new EntityNotFoundException(_localizer.GetString("GroupNotFound"));
            }

            var userAccess = await ValidateUserCanAccessGroup(group.Id, criteria.CurrentUserId).ConfigureAwait(false);
            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(criteria.CurrentUserId).ConfigureAwait(false);
            if (!userAccess && !isSuperAdminOrAdmin)
            {
                _logger.LogWarning("User with id: {userId} is not authorized user to access the group with id: {groupId}.", criteria.CurrentUserId, group.Id);
                throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
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

        /// <summary>
        /// Handle to get user courses list with progress detail
        /// </summary>
        /// <param name="userId">the requested user id</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the search result of <see cref="CourseResponseModel"/></returns>
        public async Task<SearchResult<CourseResponseModel>> GetUserCourses(Guid userId, BaseSearchCriteria criteria)
        {
            try
            {
                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(criteria.CurrentUserId).ConfigureAwait(false);

                if (!isSuperAdminOrAdmin && criteria.CurrentUserId != userId)
                {
                    return new SearchResult<CourseResponseModel>
                    {
                        Items = new List<CourseResponseModel>(),
                        CurrentPage = criteria.Page,
                        PageSize = criteria.Size,
                    };
                }
                var predicate = PredicateBuilder.New<Course>(true);
                predicate = predicate.And(p => p.CourseEnrollments.Any(x => x.CourseId == p.Id && x.UserId == userId && !x.IsDeleted &&
                               (x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed)));

                var courses = await _unitOfWork.GetRepository<Course>().GetAllAsync(
                   predicate: predicate,
                   include: src => src.Include(x => x.User).Include(x => x.CourseEnrollments).Include(x => x.Group).Include(x => x.Level)
                   ).ConfigureAwait(false);

                var result = courses.ToIPagedList(criteria.Page, criteria.Size);

                var response = new SearchResult<CourseResponseModel>
                {
                    Items = new List<CourseResponseModel>(),
                    CurrentPage = result.CurrentPage,
                    PageSize = result.PageSize,
                    TotalCount = result.TotalCount,
                    TotalPage = result.TotalPage,
                };

                result.Items.ForEach(x =>
                {
                    response.Items.Add(new CourseResponseModel
                    {
                        Id = x.Id,
                        Slug = x.Slug,
                        Name = x.Name,
                        ThumbnailUrl = x.ThumbnailUrl,
                        Percentage = x.CourseEnrollments.FirstOrDefault(
                                    predicate: p => p.UserId == userId && !p.IsDeleted
                                    && (p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled
                                        || p.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Completed))?.Percentage,
                        Language = x.Language,
                        GroupId = x.GroupId,
                        GroupName = x.Group?.Name,
                        LevelId = x.LevelId,
                        LevelName = x.Level?.Name,
                        CreatedOn = x.CreatedOn,
                        User = new UserModel(x.User),
                    });
                });
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch training list of the user.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFetachTrainingUserList"));
            }
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
                var responses = new List<CourseStatisticsResponseModel>();
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.CourseId == course.Id && !p.IsDeleted).ConfigureAwait(false);
                var lessonId = lessons.Select(x => x.Id);
                var meetings = await _unitOfWork.GetRepository<Meeting>().GetAllAsync(predicate: p=> lessonId.Contains(p.Lesson.Id),include:src=>src.Include(x=>x.Lesson)).ConfigureAwait(false);
                var meetingName = meetings.Select(x => x.Lesson.Name).ToList();
                var lessonSlug = lessons.Select(x => x.Slug).ToList();
                var passcode = meetings.Select(x => x.Passcode).ToList();
                var startdate = meetings.Select(x => x.StartDate).ToList();
                var ZoomId = meetings.Select(x => x.Id).ToList();
                var meetingCredential1 = new Dictionary<string, (string LessonSlug, string Passcode, DateTime? StartDate, Guid ZoomId)>();
                for (int i = 0; i < meetingName.Count; i++)
                {
                    meetingCredential1.Add(meetingName[i], (LessonSlug: lessonSlug[i], Passcode: passcode[i], StartDate: startdate[i], ZoomId: ZoomId[i]));
                }

                foreach (var metingcredential in meetingCredential1)
                {
                    responses.Add(new CourseStatisticsResponseModel {
                        Meetings1 = (LessonSlug: metingcredential.Value.LessonSlug, Passcode: metingcredential.Value.Passcode, StartDate: metingcredential.Value.StartDate, ZoomId: metingcredential.Value.ZoomId)
                    });
                }
                var response = new CourseStatisticsResponseModel();

                var newMeeting = new List<MeetingDashboardResponseMpodel>();
                foreach (var meetings12 in responses.Select(x => x.Meetings1))
                {
                    newMeeting.Add(new MeetingDashboardResponseMpodel
                    {
                        Passcode = meetings12.Passcode,
                        LessonSlug = meetings12.LessonSlug,
                        StartDate = meetings12.StartDate,
                        ZoomId = meetings12.ZoomId
                    });
                }
                response.MeetingsList = newMeeting.OrderByDescending(x => x.StartDate);
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
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFetchTrainingStatistics"));
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
                var LessonIds = lessons.Select(x=>x.Id).ToList();
                var watchHistory = await _unitOfWork.GetRepository<WatchHistory>().GetAllAsync(predicate: p => p.CourseId == course.Id && LessonIds.Contains(p.LessonId)).ConfigureAwait(false);
                watchHistory = watchHistory
                              .GroupBy(x => x.LessonId)
                              .SelectMany(group => group.DistinctBy(x => x.UserId))
                              .ToList();
                foreach (var lesson in lessons.OrderBy(o=>o.Section.Id).ThenBy(o=>o.Order))
                {
                    response.Add(new LessonStatisticsResponseModel
                    {
                        Id =lesson.Id,
                        Slug = lesson.Slug,
                        Name = lesson.Name,
                        CourseId = course.Id,
                        CourseSlug = course.Slug,
                        CourseName = course.Name,
                        LessonType = lesson.Type,
                        SectionId = lesson.SectionId,
                        SectionSlug = lesson.Section?.Slug,
                        SectionName = lesson.Section?.Name,
                        IsMandatory = lesson.IsMandatory,
                        EnrolledStudent = course.CourseEnrollments.Count,
                        LessonWatched = watchHistory.Where(x=>x.LessonId == lesson.Id && x.CourseId == course.Id).Count(),
                    });
                } 
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch training lesson statistics.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFetchLessonStatistics"));
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
                predicate: p => p.CourseId == course.Id && (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity),include: src => src.Include(x=>x.CourseEnrollments).
                ThenInclude(x=>x.User)).ConfigureAwait(false);
            var enrolledUserlist = lesson.CourseEnrollments.Select(x => x.User);

            var userScore = new List<QuestionSetResultDetailModel>();
            decimal totalMarks = 0;
            List<(Guid UserId, bool UserResult)> userResults = new List<(Guid, bool)>();
            if (lesson.QuestionSetId != null)
            {
                lesson.QuestionSet = new QuestionSet();
                lesson.QuestionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(predicate: p => p.Id == lesson.QuestionSetId,
                include: src => src.Include(x => x.QuestionSetSubmissions).Include(x => x.QuestionSetResults).Include(x => x.QuestionSetQuestions)).ConfigureAwait(false);
                if(lesson.QuestionSet.QuestionSetQuestions.Count != default)
                {
                    totalMarks = lesson.QuestionSet.QuestionMarking * lesson.QuestionSet.QuestionSetQuestions.Count;
                }
                foreach (var user in enrolledUserlist)
                {
                    var userQuestionSubmissionSet = lesson.QuestionSet.QuestionSetResults.Where(x => x.UserId == user.Id).ToList();
                    userScore.Add(new QuestionSetResultDetailModel
                    {
                        ObtainedMarks = userQuestionSubmissionSet.Count > 0 ? ((userQuestionSubmissionSet.Max(x=>x.TotalMark) - userQuestionSubmissionSet.Max(x=>x.NegativeMark))
                           > 0 ? (userQuestionSubmissionSet.Max(x=>x.TotalMark) - userQuestionSubmissionSet.Max(x=>x.NegativeMark)) : 0).ToString() : "",
                        Duration = lesson.QuestionSet.Duration != 0 ? TimeSpan.FromSeconds(lesson.QuestionSet.Duration).ToString(@"hh\:mm\:ss") : string.Empty,
                    });
                    if (userScore.Count() != 0)
                    {
                        var maxScore = userScore.Where(x => x.ObtainedMarks == userScore.Max(y => y.ObtainedMarks)).ToList();
                        var maxnumber = maxScore.Max(x => decimal.Parse(x.ObtainedMarks));
                        if (maxnumber != 0)
                        {
                            bool userResult = maxnumber/totalMarks*100 >= lesson.QuestionSet.PassingWeightage;
                            userResults.Add((user.Id, userResult));
                        }
                        if(maxnumber == 0 & lesson.QuestionSet.PassingWeightage != 0)
                        {
                            bool userResult = false;
                            userResults.Add((user.Id, userResult));
                        }
                        if(maxnumber == 0 && lesson.QuestionSet.PassingWeightage == 0)
                        {
                            bool userResult = true;
                            userResults.Add((user.Id, userResult));
                        }
                    }              
                }
            }
            List<(Guid UserId, bool UserResult)?> assignmentStatus = new List<(Guid, bool)?>();
            if (lesson.Type == LessonType.Assignment)
            {
                var assignmentSubmission = await _unitOfWork.GetRepository<AssignmentSubmission>().GetAllAsync(predicate: p=> p.LessonId == lesson.Id).ConfigureAwait(false);
                var assignmentReview = await _unitOfWork.GetRepository<AssignmentReview>().GetAllAsync(predicate: p=>p.LessonId == lesson.Id).ConfigureAwait(false);
                if (assignmentSubmission.Count != default) 
                {
                    var userIds = assignmentSubmission.Select(x => x.UserId).ToList();
                    foreach(var userId in userIds)
                    {
                        bool hasSubmitted = false;
                        if (assignmentSubmission.Any(x => x.UserId == userId) && assignmentReview.Any(x=>x.UserId == userId))
                        {
                            hasSubmitted = true;
                        }
                        assignmentStatus.Add((userId,hasSubmitted));
                    }
                }
            }
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
                           IsPassed = userResults.FirstOrDefault(ur => ur.UserId == student.UserId) != default ? true : m?.IsPassed ?? false,
                           UpdatedOn = m?.UpdatedOn ?? m?.CreatedOn,
                           IsAssignmentReviewed = (bool?)(assignmentStatus.FirstOrDefault(ur => ur.Value.UserId == student.UserId)?.UserResult),
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
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFetchTrainingStudentStatistics"));
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
            var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p=>p.Type ==LessonType.Assignment && p.CourseId == course.Id).ConfigureAwait(false);

            List<(Guid LessonId, bool UserResult)?> assignmentStatus = new List<(Guid, bool)?>();
            if (lessons.Count != default)
            {
                var lessonIds = lessons.Select(x => x.Id).ToList();
                var assignmentSubmission = await _unitOfWork.GetRepository<AssignmentSubmission>().GetAllAsync(predicate: p => lessonIds.Contains(p.LessonId)).ConfigureAwait(false);
                var assignmentReview = await _unitOfWork.GetRepository<AssignmentReview>().GetAllAsync(predicate: p =>lessonIds.Contains(p.LessonId)).ConfigureAwait(false);
                if (assignmentSubmission.Count != default)
                {
                    foreach (var lessonId in lessonIds)
                    {
                        bool hasSubmitted = false;
                        if (assignmentSubmission.Any(x => x.LessonId == lessonId && x.UserId == userId) && assignmentReview.Any(x => x.LessonId == lessonId && x.UserId == userId))
                        {
                            hasSubmitted = true;
                        }
                        assignmentStatus.Add((lessonId,hasSubmitted));
                    }
                }
            }
            watchHistories.ForEach(x => response.Add(new LessonStudentResponseModel
            {
                IsAssignmentReviewed = (bool?)(assignmentStatus.FirstOrDefault(ur => ur.Value.LessonId == x.LessonId)?.UserResult),
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

        /// <summary>
        /// Handel to get Role of curent user 
        /// </summary>
        /// <param name="CurrentUserID">current user id</param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        public async Task ISSuperAdminAdminOrTrainerAsync(Guid CurrentUserID)
        {
            await ExecuteAsync( async () =>
            {
                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Id == CurrentUserID && p.Role != UserRole.Trainee).ConfigureAwait(false);
                if (user == default)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }
            });
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
                responseModel.TotalActiveUsers = users.Count(predicate: p => p.Status == UserStatus.Active);
                responseModel.TotalTrainers = users.Count(predicate: p => p.Status == UserStatus.Active && p.Role == UserRole.Trainer);

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
                    predicate: p => p.CourseTeachers.Any(x => x.UserId == currentUserId)).ConfigureAwait(false);

                responseModel.TotalEnrolledCourses = await _unitOfWork.GetRepository<CourseEnrollment>().CountAsync(predicate: p => p.EnrollmentMemberStatus.Equals(EnrollmentMemberStatusEnum.Enrolled) &&
                    p.UserId.Equals(currentUserId));

                responseModel.TotalActiveTrainings = trainings.Count(predicate: p => p.Status == CourseStatus.Published || p.IsUpdate && p.Status != CourseStatus.Completed);
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


        /// <summary>
        /// Handles to get upcomming lesson
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns> the list of lesson <see cref="Lesson" /> .</returns>
        public async Task<List<DashboardLessonResponseModel>> GetUpcommingLesson(Guid currentUserId)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Id == currentUserId,
                    include : src =>src.Include(x =>x.CourseEnrollments)).ConfigureAwait(false);
                   
                    if (user.Role == UserRole.SuperAdmin || user.Role == UserRole.Admin)
                    {
                        var course = await _unitOfWork.GetRepository<Course>().GetAllAsync(include: src => src.Include(x => x.Lessons).ThenInclude(x => x.Meeting)).ConfigureAwait(false);
                        var currentDateTime = DateTime.Now;
                        var response = new List<DashboardLessonResponseModel>();
                        var upcommingLessons = course.SelectMany(x => x.Lessons).Where(x => x.Meeting.StartDate > currentDateTime).ToList();
                        foreach (var lesson in upcommingLessons)
                        {
                            response.Add(new DashboardLessonResponseModel
                            {
                                LessonSlug = lesson.Slug,
                                LessonType = lesson.Type,
                                LessonName = lesson.Name,
                                StartDate = lesson.Meeting.StartDate,
                            });
                        }

                        response = response.OrderByDescending(x => x.StartDate).Reverse().ToList();
                        return response;
                    }

                if (user.Role == UserRole.Trainer)
                {
                    var courseLiveLessons = await _unitOfWork.GetRepository<Course>().GetAllAsync(predicate: p => p.CourseTeachers.Any(x => x.UserId == currentUserId ||
                    p.CourseEnrollments.Any(x => x.UserId == currentUserId)),
                        include: src => src.Include(x => x.CourseEnrollments).Include(x => x.Lessons).ThenInclude(x => x.Meeting)).ConfigureAwait(false);

                    var courseExamLessons = await _unitOfWork.GetRepository<Course>().GetAllAsync(predicate: p => p.CourseTeachers.Any(x => x.UserId == currentUserId ||
                    p.CourseEnrollments.Any(x => x.UserId == currentUserId)),
                        include: src => src.Include(x => x.CourseEnrollments).Include(x => x.Lessons).ThenInclude(x => x.QuestionSet)).ConfigureAwait(false);

                    var CourseAssignmentLesson = await _unitOfWork.GetRepository<Course>().GetAllAsync(predicate: p => p.CourseTeachers.Any(x => x.UserId == currentUserId ||
                    p.CourseEnrollments.Any(x => x.UserId == currentUserId)),
                        include: src => src.Include(x => x.CourseEnrollments).Include(x => x.Lessons).ThenInclude(x => x.Assignments)).ConfigureAwait(false);


                    var currentDateTime = DateTime.Now;
                    var upcommingLiveLessons = courseLiveLessons.SelectMany(x => x.Lessons).Where(x => x.Meeting.StartDate > currentDateTime).ToList();
                    var upcommingLessonExams = courseExamLessons.SelectMany(x => x.Lessons).Where(x => x.QuestionSet.StartTime > currentDateTime).ToList();
                    var upcommingAssignments = CourseAssignmentLesson.SelectMany(x => x.Lessons).Where(x => x.StartDate > currentDateTime).ToList();

                    var response = new List<DashboardLessonResponseModel>();

                    foreach (var lesson in upcommingLiveLessons)
                    {
                        response.Add(new DashboardLessonResponseModel
                        {
                            LessonSlug = lesson.Slug,
                            LessonType = lesson.Type,
                            LessonName = lesson.Name,
                            StartDate = lesson.Meeting.StartDate,
                            CourseEnrollmentBool = courseLiveLessons.Any(x => x.CourseEnrollments.Any(x => x.CourseId == lesson.CourseId && x.UserId == currentUserId))
                        }) ;
                    }

                    foreach (var lesson in upcommingAssignments)
                    {
                        response.Add(new DashboardLessonResponseModel
                        {
                            LessonSlug = lesson.Slug,
                            LessonType = lesson.Type,
                            LessonName = lesson.Name,
                            StartDate = lesson.StartDate,
                            CourseEnrollmentBool = CourseAssignmentLesson.Any(x => x.CourseEnrollments.Any(x => x.CourseId == lesson.CourseId && x.UserId == currentUserId))
                        });
                    }

                    foreach (var lesson in upcommingLessonExams)
                    {
                        response.Add(new DashboardLessonResponseModel
                        {
                            LessonSlug = lesson.Slug,
                            LessonType = lesson.Type,
                            LessonName = lesson.Name,
                            StartDate = lesson.QuestionSet.StartTime,
                            CourseEnrollmentBool = courseExamLessons.Any(x => x.CourseEnrollments.Any(x => x.CourseId == lesson.CourseId && x.UserId == currentUserId))
                        });
                    }
                    response = response.OrderByDescending(x => x.StartDate).Reverse().ToList();
                    return response;
                }
             

                if (user.Role == UserRole.Trainee)
                {
                    var lessonLiveClass= await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.Course.CourseEnrollments.All(x => x.UserId == currentUserId),
                       include: src =>src.Include(x=>x.Meeting)).ConfigureAwait(false);
                    var lessonExam = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.Course.CourseEnrollments.All(x => x.UserId == currentUserId),
                       include: src => src.Include(x => x.QuestionSet)).ConfigureAwait(false);
                    var lessonAssignments = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.Course.CourseEnrollments.All(x => x.UserId == currentUserId),
                      include: src => src.Include(x => x.Assignments)).ConfigureAwait(false);

                    var currentDateTime = DateTime.Now;

                    var upcommingAssignments = lessonAssignments.Where(x => x.StartDate > currentDateTime).ToList();
                    var upcommingLiveLessons = lessonLiveClass.Where(x => x.Meeting.StartDate > currentDateTime).ToList();
                    var upcommingLessonExams = lessonExam.Where(x => x.QuestionSet.StartTime > currentDateTime).ToList();

                    var response = new List<DashboardLessonResponseModel>();
                    foreach (var lesson in upcommingLiveLessons)
                    { 
                        response.Add(new DashboardLessonResponseModel
                            {
                                LessonSlug = lesson.Slug,
                                LessonType = lesson.Type,
                                LessonName = lesson.Name,
                                StartDate = lesson.Meeting.StartDate,
                            });   
                    }

                    foreach (var lesson in upcommingAssignments)
                    {
                        response.Add(new DashboardLessonResponseModel
                        {
                            LessonSlug = lesson.Slug,
                            LessonType = lesson.Type,
                            LessonName = lesson.Name,
                            StartDate = lesson.StartDate,
                        });
                    }

                    foreach (var lesson in upcommingLessonExams)
                    {
                        response.Add(new DashboardLessonResponseModel
                        {
                            LessonSlug = lesson.Slug,
                            LessonType = lesson.Type,
                            LessonName = lesson.Name,
                            StartDate = lesson.QuestionSet.StartTime
                        });
                    }
                    response = response.OrderByDescending(x => x.StartDate).Reverse().ToList();
                    return response;
                }

                else
                {
                    throw new UnauthorizedAccessException(_localizer.GetString("UnauthorizedUser"));
                }
            }
            catch
            {
                throw new NullReferenceException(_localizer.GetString("UnauthorizedUser"));
            }
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

                course.CourseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id
                    ).ConfigureAwait(false);

                if (course.CourseCertificate == null)
                {
                    _logger.LogWarning("Certificate detail information not found for training with id :{courseId}.", course.Id);
                    throw new EntityNotFoundException(_localizer.GetString("CertificateNotFound"));
                }

                course.Signatures = new List<Signature>();
                course.Signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (course.Signatures.Count == 0)
                {
                    _logger.LogWarning("At least one trainer signature detail is required for training with id :{courseId}.", course.Id);
                    throw new EntityNotFoundException(_localizer.GetString("AtLeastOneTrainerSignatureRequired"));
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
                var certificateissueduser = new List<CertificateUserIssuedDto>();
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
                    certificateissueduser.Add(new CertificateUserIssuedDto
                    {
                        UserName = item.User.FirstName,
                        CourseName = course.Name,
                        Email= item.User.Email
                    });
                }
                _unitOfWork.GetRepository<CourseEnrollment>().Update(results);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                if (certificateissueduser.Count != default)
                {
                    BackgroundJob.Enqueue<IHangfireJobService>(job => job.SendCertificateIssueMailAsync(course.Name,certificateissueduser, null));
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to issued the training certificate.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredOnCertificateIssued"));
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
                        startDate = certificate?.EventStartDate.ToString("dd MMMM yyyy"),
                        endDate = certificate?.EventEndDate.ToString("dd MMMM yyyy"),
                        authors,
                    });

            var response = await client.PostAsync(request).ConfigureAwait(false);
            var fileName = certificate?.Title ?? "certificate";
            MemoryStream stream = new(response.RawBytes);
            var formFile = new FormFile(stream, 0, stream.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = response.ContentType
            };
            var fileResponse = await _mediaService.UploadFileAsync(new MediaRequestModel { File = formFile, Type = MediaType.Public }).ConfigureAwait(false);
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
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }

                var signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(predicate: x => x.CourseId == course.Id).ConfigureAwait(false);
                var response = new List<SignatureResponseModel>();
                signatures.ForEach(x => response.Add(new SignatureResponseModel(x)));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying fetch the training signature.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFetchTrainingSignature"));
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
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }
                var signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
                if (signatures.Count >= 3)
                {
                    _logger.LogWarning("training with id: {id} cannot have more than 3 signatures for user with id: {userId}.", course.Id, currentUserId);
                    throw new ForbiddenException(_localizer.GetString("AtMostThreeSignaturesAllowed"));
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
                var existingCertificateUrlKey = courseCertificate?.SampleUrl;

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

                if (!string.IsNullOrWhiteSpace(existingCertificateUrlKey))
                {
                    if (existingCertificateUrlKey.ToLower().Trim().Contains("/public/") && existingCertificateUrlKey.IndexOf("/standalone/") != -1)
                    {
                        existingCertificateUrlKey = existingCertificateUrlKey.Substring(existingCertificateUrlKey.IndexOf("/standalone/") + "/standalone/".Length);
                    }
                    await _fileServerService.RemoveFileAsync(existingCertificateUrlKey).ConfigureAwait(false);
                }

                var response = new SignatureResponseModel(signature);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to upload signature in the training.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredUploadSignature"));
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
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }
                var signature = await _unitOfWork.GetRepository<Signature>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id && p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (signature == null)
                {
                    _logger.LogWarning("Signature with id: {id} and trainingId : {courseId} not found.", id, course.Id);
                    throw new EntityNotFoundException(_localizer.GetString("SignatureNotFound"));
                }
                var currentTimeStamp = DateTime.UtcNow;

                var existingSignatureUrlKey = signature.FileUrl;

                signature.FullName = model.FullName;
                signature.Designation = model.Designation;
                signature.FileUrl = model.FileURL;
                signature.UpdatedOn = currentTimeStamp;
                signature.UpdatedBy = currentUserId;

                var courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id
                    ).ConfigureAwait(false);

                var existingCertificateUrlKey = courseCertificate.SampleUrl;


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

                if (!string.IsNullOrWhiteSpace(existingCertificateUrlKey))
                {
                    if (existingCertificateUrlKey.ToLower().Trim().Contains("/public/") && existingCertificateUrlKey.IndexOf("/standalone/") != -1)
                    {
                        existingCertificateUrlKey = existingCertificateUrlKey.Substring(existingCertificateUrlKey.IndexOf("/standalone/") + "/standalone/".Length);
                    }
                    await _fileServerService.RemoveFileAsync(existingCertificateUrlKey).ConfigureAwait(false);
                }
                if (existingSignatureUrlKey != signature.FileUrl && !string.IsNullOrWhiteSpace(existingSignatureUrlKey))
                {
                    if (existingSignatureUrlKey.ToLower().Trim().Contains("/public/") && existingSignatureUrlKey.IndexOf("/standalone/") != -1)
                    {
                        existingSignatureUrlKey = existingSignatureUrlKey.Substring(existingSignatureUrlKey.IndexOf("/standalone/") + "/standalone/".Length);
                    }
                    await _fileServerService.RemoveFileAsync(existingSignatureUrlKey).ConfigureAwait(false);
                }

                var response = new SignatureResponseModel(signature);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update signature in the training.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredUpdateSignature"));
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
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }
                var signature = await _unitOfWork.GetRepository<Signature>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == id && p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (signature == null)
                {
                    _logger.LogWarning("Signature with id: {id} and trainingId : {courseId} not found.", id, course.Id);
                    throw new EntityNotFoundException(_localizer.GetString("SignatureNotFound"));
                }

                var courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id
                    ).ConfigureAwait(false);
                
                var existingCertificateUrlKey = courseCertificate?.SampleUrl;

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

                //Delete the previous certificate sample and signature file from server

                if (!string.IsNullOrWhiteSpace(existingCertificateUrlKey))
                {
                    if (existingCertificateUrlKey.ToLower().Trim().Contains("/public/") && existingCertificateUrlKey.IndexOf("/standalone/") != -1)
                    {
                        existingCertificateUrlKey = existingCertificateUrlKey.Substring(existingCertificateUrlKey.IndexOf("/standalone/") + "/standalone/".Length);
                    }
                    await _fileServerService.RemoveFileAsync(existingCertificateUrlKey).ConfigureAwait(false);
                }
                if (!string.IsNullOrWhiteSpace(signature.FileUrl))
                {
                    if (signature.FileUrl.ToLower().Trim().Contains("/public/") && signature.FileUrl.IndexOf("/standalone/") != -1)
                    {
                        signature.FileUrl = signature.FileUrl.Substring(signature.FileUrl.IndexOf("/standalone/") + "/standalone/".Length);
                    }
                    await _fileServerService.RemoveFileAsync(signature.FileUrl).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update signature in the training.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredDeleteSignature"));
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
                throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            }
            var signatures = await _unitOfWork.GetRepository<Signature>().GetAllAsync(
                       predicate: p => p.CourseId == course.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var courseCertificate = new CourseCertificate();
            var cstZone = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            if (model.Id.HasValue)
            {
                courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == model.Id.Value
                    ).ConfigureAwait(false);
                courseCertificate.Title = model.Title;
                courseCertificate.EventStartDate = TimeZoneInfo.ConvertTimeFromUtc(model.EventStartDate, cstZone);
                courseCertificate.EventEndDate = TimeZoneInfo.ConvertTimeFromUtc(model.EventEndDate, cstZone);
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
                    EventStartDate = TimeZoneInfo.ConvertTimeFromUtc(model.EventStartDate, TimeZoneInfo.Local),
                    EventEndDate = TimeZoneInfo.ConvertTimeFromUtc(model.EventEndDate, TimeZoneInfo.Local),
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
                throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            }
            var courseCertificate = await _unitOfWork.GetRepository<CourseCertificate>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id
                ).ConfigureAwait(false);

            return courseCertificate == null ? null : new CourseCertificateResponseModel(courseCertificate);
        }




        #endregion Signature
    }
}