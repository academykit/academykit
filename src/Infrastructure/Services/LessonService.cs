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
    using System;
    using System.Linq.Expressions;

    public class LessonService : BaseGenericService<Lesson, LessonBaseSearchCriteria>, ILessonService
    {
        private readonly IZoomLicenseService _zoomLicenseService;
        public LessonService(
            IUnitOfWork unitOfWork,
            ILogger<LessonService> logger,
            IZoomLicenseService zoomLicenseService) : base(unitOfWork, logger)
        {
            _zoomLicenseService = zoomLicenseService;
        }

        #region Protected Region

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Lesson, bool>> ConstructQueryConditions(Expression<Func<Lesson, bool>> predicate, LessonBaseSearchCriteria criteria)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(criteria.CourseIdentity, nameof(criteria.CourseIdentity));
            CommonHelper.ValidateArgumentNotNullOrEmpty(criteria.SectionIdentity, nameof(criteria.SectionIdentity));
            var course = ValidateAndGetCourse(criteria.CurrentUserId, criteria.CourseIdentity).Result;
            var section = _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id && (p.Id.ToString() == criteria.SectionIdentity || p.Slug == criteria.SectionIdentity)).Result;

            return predicate.And(p => p.CourseId == course.Id && p.SectionId == section.Id && !p.IsDeleted);
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Lesson, object> IncludeNavigationProperties(IQueryable<Lesson> query)
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
        /// <returns>the instance of <see cref="Lesson"/></returns>
        public async Task<Lesson> GetLessonAsync(string identity, string lessonIdentity, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("Course with identity: {identity} not found for user with :{id}", identity, currentUserId);
                throw new EntityNotFoundException("Course not found");
            }

            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: p => p.CourseId == course.Id && (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity),
                include: src => src.Include(x => x.User)
                                    .Include(x => x.Course)
                                    .Include(x => x.Section)).ConfigureAwait(false);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson with identity : {identity} and course with id: {courseId} not found for user with :{id}", identity, course.Id, currentUserId);
                throw new EntityNotFoundException("Lesson not found");
            }
            return lesson;
        }

        /// <summary>
        /// Handle to create lesson
        /// </summary>
        /// <param name="courseIdentity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></see></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<Lesson> AddAsync(string courseIdentity, LessonRequestModel model, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, courseIdentity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Course with identity: {identity} not found for user with :{id}", courseIdentity, currentUserId);
                    throw new EntityNotFoundException("Course not found");
                }
                var section = await _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id &&
                                (p.Id.ToString() == model.SectionIdentity || p.Slug == model.SectionIdentity)).ConfigureAwait(false);
                if (section == null)
                {
                    _logger.LogWarning("Section with identity: {identity} not found for user with id:{id} and course with id: {courseId}",
                                            courseIdentity, currentUserId, course.Id);
                    throw new EntityNotFoundException("Course not found");
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
                    IsPreview = model.IsPreview,
                    IsMandatory = model.IsMandatory,
                    SectionId = section.Id,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                };
                if (lesson.Type == LessonType.Exam)
                {
                    lesson.Name = model.QuestionSet.Name;
                }
                lesson.Slug = CommonHelper.GetEntityTitleSlug<Course>(_unitOfWork, (slug) => q => q.Slug == slug, lesson.Name);
                if (lesson.Type == LessonType.Document)
                {
                    lesson.DocumentUrl = model.DocumentUrl;
                }
                if (lesson.Type == LessonType.Video)
                {
                    lesson.VideoUrl = model.VideoUrl;
                }
                if (lesson.Type == LessonType.LiveClass)
                {
                    await CreateMeetingAsync(model, lesson).ConfigureAwait(false);
                }
                if (lesson.Type == LessonType.Exam)
                {
                    await CreateQuestionSetAsync(model, lesson).ConfigureAwait(false);
                }

                await _unitOfWork.GetRepository<Lesson>().InsertAsync(lesson).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return lesson;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to create the lesson");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to create the lesson");
            }
        }

        /// <summary>
        /// Handle to delete lesson
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task DeleteLessonAsync(string identity, string lessonIdentity, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("DeleteLessonAsync(): Course with identity : {0} not found for user with id :{1}.", identity, currentUserId);
                throw new EntityNotFoundException("Course was not found");
            }

            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: p => (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity) && p.CourseId == course.Id
                ).ConfigureAwait(false);
            if (lesson == null)
            {
                _logger.LogWarning("DeleteLessonAsync(): Lesson with identity : {0} was not found for user with id : {1} and having course with id : {2}",
                    lessonIdentity, currentUserId, course.Id);
                throw new EntityNotFoundException("Lesson was not found");
            }

            if (lesson.Type == LessonType.RecordedVideo)
            {
                _logger.LogWarning("DeleteLessonAsync(): Lesson with id : {0} has type : {1}", lesson.Id, lesson.Type);
                throw new ForbiddenException($"Lesson with type {lesson.Type} cannot be delete");
            }

            if (lesson.Type == LessonType.Exam)
            {
                var questionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == lesson.QuestionSetId,
                    include: src => src.Include(x => x.QuestionSetQuestions)).ConfigureAwait(false);
                if (questionSet == null)
                {
                    _logger.LogWarning("DeleteLessonAsync(): Lesson with id:{0} and type: {1} doesnot contain question set with id : {2}",
                                        lesson.Id, lesson.Type, lesson.QuestionSetId);
                    throw new EntityNotFoundException($"Question set not found for lesson with type: {lesson.Type}");
                }

                var hasAnyAttempt = await _unitOfWork.GetRepository<QuestionSetSubmission>().ExistsAsync(
                    predicate: p => p.QuestionSetId == lesson.QuestionSetId).ConfigureAwait(false);
                if (hasAnyAttempt)
                {
                    _logger.LogWarning("DeleteLessonAsync(): Lesson with id: {0} and question set with id: {1} having type: {2} contains exam submission",
                                        lesson.Id, lesson.QuestionSetId, lesson.Type);
                    throw new ForbiddenException($"Lesson with type {lesson.Type} contains exam submission. So, it cannot be delete");
                }

                _unitOfWork.GetRepository<QuestionSetQuestion>().Delete(questionSet.QuestionSetQuestions);
                _unitOfWork.GetRepository<QuestionSet>().Delete(questionSet);
            }

            if (lesson.Type == LessonType.LiveClass)
            {
                var meeting = await _unitOfWork.GetRepository<Meeting>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == lesson.MeetingId).ConfigureAwait(false);
                if (meeting == null)
                {
                    _logger.LogWarning("DeleteLessonAsync(): Lesson with id:{0} and type: {1} doesn't contain meeting with id : {2}",
                                       lesson.Id, lesson.Type, lesson.MeetingId);
                    throw new EntityNotFoundException($"Meeting not found for lesson with type: {lesson.Type}");
                }
            }

            _unitOfWork.GetRepository<Lesson>().Delete(lesson);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        }

        /// <summary>
        /// Handle to join meeting
        /// </summary>
        /// <param name="identity">the course identity</param>
        /// <param name="lessonIdentity">the lesson identity</param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        public async Task<MeetingJoinResponseModel> GetJoinMeetingAsync(string identity, string lessonIdentity, Guid currentUserId)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == currentUserId && !p.IsActive).ConfigureAwait(false);
                if (user == null)
                {
                    _logger.LogWarning("User with id: {id} not found", currentUserId);
                    throw new EntityNotFoundException("User not found");
                }

                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: false).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Course with identity: {identity} not found for user with id :{id}", identity, currentUserId);
                    throw new EntityNotFoundException("Course not found");
                }

                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id && (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity),
                    include: src => src.Include(x => x.User)
                                        .Include(x => x.Meeting.ZoomLicense)).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity : {identity} and course with id: {courseId} not found for user with :{id}", identity, course.Id, currentUserId);
                    throw new EntityNotFoundException("Lesson not found");
                }

                if (lesson.Type != LessonType.LiveClass)
                {
                    _logger.LogWarning("Lesson with id : {id} type not match for join meeting", lesson.Id);
                    throw new ForbiddenException("Lesson type not match for join meeting");
                }

                if (lesson.Meeting == null)
                {
                    _logger.LogWarning("Lesson with id : {id}  meeting not found for join meeting", lesson.Id);
                    throw new EntityNotFoundException("Meeting not found");
                }

                if (lesson.Meeting.ZoomLicense == null)
                {
                    _logger.LogWarning("Zoom license with id : {id} not found.", lesson.Meeting.ZoomLicenseId);
                    throw new ServiceException("Zoom license not found");
                }

                var isModerator = course.CreatedBy == currentUserId || lesson.CreatedBy == currentUserId || course.CourseTeachers.Any(x => x.UserId == currentUserId);

                //validate user is enroll in the course or not
                if (!isModerator)
                {
                    var isMember = course.CourseEnrollments.Any(x => x.UserId == currentUserId && !x.IsDeleted
                                    && (x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled || x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled));
                    if (!isMember)
                    {
                        _logger.LogWarning("User with id : {currentUserId} is invalid user to attend this meeting having lesson with id :{id}", currentUserId, lesson.Id);
                        throw new ForbiddenException("You are not allowed to access this meeting");
                    }
                }

                if (lesson.Meeting.MeetingNumber == default)
                {
                    await _zoomLicenseService.CreateZoomMeetingAsync(lesson);
                }

                var signature = await _zoomLicenseService.GenerateZoomSignatureAsync(lesson.Meeting.MeetingNumber.ToString(), isModerator).ConfigureAwait(false);

                var zak = isModerator ? await _zoomLicenseService.GetZAKAsync(lesson.Meeting.ZoomLicense.HostId).ConfigureAwait(false) : null;

                var response = new MeetingJoinResponseModel();
                response.RoomName = lesson.Name;
                response.JwtToken = signature;
                response.ZAKToken = zak;
                response.HostId = lesson.Meeting.ZoomLicense.HostId;
                response.UserName = user.FullName;
                response.UserEmail = user.Email;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        #region Private Methods

        /// <summary>
        /// Handle to  create question set
        /// </summary>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="lesson"></param>
        /// <returns></returns>
        private async Task CreateQuestionSetAsync(LessonRequestModel model, Lesson lesson)
        {
            lesson.QuestionSet = new QuestionSet();
            lesson.QuestionSet = new QuestionSet
            {
                Id = Guid.NewGuid(),
                Name = lesson.Name,
                Slug = string.Concat(lesson.Slug, "-", lesson.Id.ToString().AsSpan(0, 5)),
                ThumbnailUrl = lesson.ThumbnailUrl,
                Description = lesson.Description,
                NegativeMarking = lesson.QuestionSet.NegativeMarking,
                QuestionMarking = lesson.QuestionSet.QuestionMarking,
                PassingWeightage = lesson.QuestionSet.PassingWeightage,
                AllowedRetake = lesson.QuestionSet.AllowedRetake,
                StartTime = lesson.QuestionSet.StartTime,
                EndTime = lesson.QuestionSet.EndTime,
                Duration = lesson.QuestionSet.Duration,
                CreatedBy = lesson.CreatedBy,
                CreatedOn = lesson.CreatedOn,
                UpdatedBy = lesson.UpdatedBy,
                UpdatedOn = lesson.UpdatedOn
            };
            lesson.Duration = model.QuestionSet.Duration;
            lesson.QuestionSetId = lesson.QuestionSet.Id;
            await _unitOfWork.GetRepository<QuestionSet>().InsertAsync(lesson.QuestionSet).ConfigureAwait(false);
        }

        private async Task CreateMeetingAsync(LessonRequestModel model, Lesson lesson)
        {
            lesson.Meeting = new Meeting();
            lesson.Meeting = new Meeting
            {
                Id = Guid.NewGuid(),
                StartDate = model.Meeting.MeetingStartDate,
                ZoomLicenseId = model.Meeting.ZoomLicenseId.Value,
                Duration = model.Meeting.MeetingDuration,
                CreatedBy = lesson.CreatedBy,
                CreatedOn = lesson.CreatedOn,
                UpdatedBy = lesson.UpdatedBy,
                UpdatedOn = lesson.UpdatedOn
            };
            lesson.MeetingId = lesson.Meeting.Id;
            lesson.Duration = model.Meeting.MeetingDuration;

            await _unitOfWork.GetRepository<Meeting>().InsertAsync(lesson.Meeting).ConfigureAwait(false);
        }

        #endregion Private Methods
    }
}