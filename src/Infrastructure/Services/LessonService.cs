namespace Lingtren.Infrastructure.Services
{
    using AngleSharp.Common;
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
        private readonly IZoomSettingService _zoomSettingService;
        public LessonService(
            IUnitOfWork unitOfWork,
            ILogger<LessonService> logger,
            IZoomLicenseService zoomLicenseService,
            IZoomSettingService zoomSettingService) : base(unitOfWork, logger)
        {
            _zoomLicenseService = zoomLicenseService;
            _zoomSettingService = zoomSettingService;
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
            var course = ValidateAndGetCourse(criteria.CurrentUserId, criteria.CourseIdentity, validateForModify: false).Result;
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
        /// <returns>the instance of <see cref="LessonResponseModel"/></returns>
        public async Task<LessonResponseModel> GetLessonAsync(string identity, string lessonIdentity, Guid currentUserId)
        {
            var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: false).ConfigureAwait(false);
            if (course == null)
            {
                _logger.LogWarning("Course with identity: {identity} not found for user with :{id}", identity, currentUserId);
                throw new EntityNotFoundException("Course not found");
            }
            var lesson = new Lesson();
            if (!string.IsNullOrWhiteSpace(lessonIdentity))
            {
                var requestedLesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
               predicate: p => p.CourseId == course.Id && (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity),
               include: src => src.Include(x => x.User)
                                   .Include(x => x.Course)
                                   .Include(x => x.Section)
               ).ConfigureAwait(false);

                if (requestedLesson == null)
                {
                    _logger.LogWarning("Lesson with identity : {identity} and course with id: {courseId} not found for user with :{id}", identity, course.Id, currentUserId);
                    throw new EntityNotFoundException("Lesson not found");
                }
                lesson = requestedLesson;
            }
            else
            {
                lesson = await GetCurrentLesson(currentUserId, course).ConfigureAwait(false);
            }

            var userCompletedWatchHistories = await _unitOfWork.GetRepository<WatchHistory>().GetAllAsync(
                predicate: p => p.UserId == currentUserId && p.CourseId == lesson.CourseId && p.IsCompleted && p.IsPassed
                ).ConfigureAwait(false);

            var sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(
                predicate: p => p.CourseId == lesson.CourseId,
                include: src => src.Include(x => x.Lessons),
                orderBy: o => o.OrderBy(x => x.Order)
                ).ConfigureAwait(false);

            var lessons = new List<Lesson>();
            lessons = sections.SelectMany(x => x.Lessons.OrderBy(x => x.Order)).ToList();

            var currentIndex = lessons.FindIndex(x => x.Id == lesson.Id);
            var orderLessons = lessons.GetRange(0, currentIndex).Where(x => x.IsMandatory);

            var containMandatoryLesson = orderLessons.Select(x => x.Id).Except(userCompletedWatchHistories.Select(x => x.LessonId));
            if (containMandatoryLesson.Count() > 0)
            {
                _logger.LogWarning("User with id: {userId} needs to view other mandatory lesson before viewing current lesson with id: {lessonId}", currentUserId, lesson.Id);
                throw new ForbiddenException("Please complete above remaining mandatory lesson before viewing current lesson.");
            }

            if (lesson.Type == LessonType.LiveClass)
            {
                lesson.Meeting = new Meeting();
                lesson.Meeting = await _unitOfWork.GetRepository<Meeting>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == lesson.MeetingId).ConfigureAwait(false);
            }

            bool? hasResult = null;
            if (lesson.Type == LessonType.Exam)
            {
                lesson.QuestionSet = new QuestionSet();
                lesson.QuestionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == lesson.QuestionSetId).ConfigureAwait(false);
                var containResults = await _unitOfWork.GetRepository<QuestionSetResult>().ExistsAsync(
                    predicate: p => p.UserId == currentUserId && p.QuestionSetId == lesson.QuestionSetId
                    ).ConfigureAwait(false);
                hasResult = containResults;
            }

            if (lesson.Type == LessonType.Assignment)
            {
                lesson.Assignments = new List<Assignment>();
                lesson.Assignments = await _unitOfWork.GetRepository<Assignment>().GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id).ConfigureAwait(false);
            }
            var responseModel = new LessonResponseModel(lesson);
            var nextLessonIndex = currentIndex + 1;
            if ((nextLessonIndex + 1) <= lessons.Count)
            {
                var nextLessonId = lessons.GetItemByIndex(nextLessonIndex)?.Id;
                responseModel.NextLessonId = nextLessonId;
            }
            responseModel.HasResult = hasResult;
            return responseModel;
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
                    Status = CourseStatus.Draft,
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
                    lesson.Duration = model.Meeting.MeetingDuration * 60; //convert duration from minutes to seconds;
                    await CreateMeetingAsync(model, lesson).ConfigureAwait(false);
                }
                if (lesson.Type == LessonType.Exam)
                {
                    lesson.Duration = model.QuestionSet.Duration * 60; //convert duration from minutes to seconds;
                    await CreateQuestionSetAsync(model, lesson).ConfigureAwait(false);
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
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to create the lesson");
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
        public async Task<Lesson> UpdateAsync(string identity, string lessonIdentity, LessonRequestModel model, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Course with identity: {identity} not found for user with :{id}", identity, currentUserId);
                    throw new EntityNotFoundException("Course not found");
                }

                var section = await _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id && (p.Id.ToString() == model.SectionIdentity || p.Slug == model.SectionIdentity)
                    ).ConfigureAwait(false);
                if (section == null)
                {
                    _logger.LogWarning("Section with identity: {identity} not found for user with id:{id} and course with id: {courseId}",
                                            identity, currentUserId, course.Id);
                    throw new EntityNotFoundException("Course not found");
                }

                var existingLesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id && p.SectionId == section.Id && (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity)
                    ).ConfigureAwait(false);
                if (existingLesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id} and course with id: {courseId} and section with id: {sectionId}",
                                            lessonIdentity, currentUserId, course.Id, section.Id);
                    throw new EntityNotFoundException("Lesson not found");
                }

                if (model.Type != existingLesson.Type)
                {
                    _logger.LogWarning("Lesson type not matched for lesson with id: {id}", existingLesson.Id);
                    throw new ForbiddenException("Lesson type not matched.");
                }
                var currentTimeStamp = DateTime.UtcNow;

                existingLesson.Name = model.Name;
                existingLesson.Description = model.Description;
                existingLesson.ThumbnailUrl = model.ThumbnailUrl;
                existingLesson.IsPreview = model.IsPreview;
                existingLesson.IsMandatory = model.IsMandatory;
                existingLesson.UpdatedBy = currentUserId;
                existingLesson.UpdatedOn = currentTimeStamp;

                if (existingLesson.Type == LessonType.Exam)
                {
                    existingLesson.Name = model.QuestionSet.Name;
                }
                if (existingLesson.Type == LessonType.Document)
                {
                    existingLesson.DocumentUrl = model.DocumentUrl;
                }
                if (existingLesson.Type == LessonType.Video)
                {
                    existingLesson.VideoUrl = model.VideoUrl;
                }
                if (existingLesson.Type == LessonType.LiveClass)
                {
                    existingLesson.Meeting = new Meeting();
                    existingLesson.Meeting = await _unitOfWork.GetRepository<Meeting>().GetFirstOrDefaultAsync(predicate: p => p.Id == existingLesson.MeetingId).ConfigureAwait(false);
                    existingLesson.Duration = model.Meeting.MeetingDuration * 60; //convert duration from minutes to seconds;
                    await UpdateMeetingAsync(model, existingLesson).ConfigureAwait(false);
                }
                if (existingLesson.Type == LessonType.Exam)
                {
                    existingLesson.QuestionSet = new QuestionSet();
                    existingLesson.QuestionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(predicate: p => p.Id == existingLesson.QuestionSetId).ConfigureAwait(false);
                    existingLesson.Duration = model.QuestionSet.Duration * 60; //convert duration from minutes to seconds;
                    await UpdateQuestionSetAsync(model, existingLesson).ConfigureAwait(false);
                }

                _unitOfWork.GetRepository<Lesson>().Update(existingLesson);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return existingLesson;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to update the lesson information");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to update the lesson information");
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
            await ExecuteAsync(async () =>
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("DeleteLessonAsync(): Course with identity : {identity} not found for user with id :{currentUserId}.", identity, currentUserId);
                    throw new EntityNotFoundException("Course was not found");
                }

                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                    predicate: p => (p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity) && p.CourseId == course.Id
                    ).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("DeleteLessonAsync(): Lesson with identity : {lessonIdentity} was not found for user with id : {userId} and having course with id : {courseId}",
                        lessonIdentity, currentUserId, course.Id);
                    throw new EntityNotFoundException("Lesson was not found");
                }

                if (lesson.Type == LessonType.RecordedVideo)
                {
                    _logger.LogWarning("DeleteLessonAsync(): Lesson with id : {lessonId} has type : {type}", lesson.Id, lesson.Type);
                    throw new ForbiddenException($"Lesson with type {lesson.Type} cannot be delete");
                }

                if (lesson.Type == LessonType.Exam)
                {
                    var questionSet = await _unitOfWork.GetRepository<QuestionSet>().GetFirstOrDefaultAsync(
                        predicate: p => p.Id == lesson.QuestionSetId,
                        include: src => src.Include(x => x.QuestionSetQuestions)).ConfigureAwait(false);
                    if (questionSet == null)
                    {
                        _logger.LogWarning("DeleteLessonAsync(): Lesson with id:{lessonId} and type: {lessonType} does not contain question set with id : {questionSetId}",
                                            lesson.Id, lesson.Type, lesson.QuestionSetId);
                        throw new EntityNotFoundException($"Question set not found for lesson with type: {lesson.Type}");
                    }

                    var hasAnyAttempt = await _unitOfWork.GetRepository<QuestionSetSubmission>().ExistsAsync(
                        predicate: p => p.QuestionSetId == lesson.QuestionSetId).ConfigureAwait(false);
                    if (hasAnyAttempt)
                    {
                        _logger.LogWarning("DeleteLessonAsync(): Lesson with id: {lessonId} and question set with id: {questionSetId} having type: {type} contains exam submission",
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
                        _logger.LogWarning("DeleteLessonAsync(): Lesson with id:{lessonId} and type: {type} does not contain meeting with id : {meetingId}",
                                           lesson.Id, lesson.Type, lesson.MeetingId);
                        throw new EntityNotFoundException($"Meeting not found for lesson with type: {lesson.Type}");
                    }
                }

                if (lesson.Type == LessonType.Assignment)
                {
                    var assignments = await _unitOfWork.GetRepository<Assignment>().GetAllAsync(
                        predicate: p => p.LessonId == lesson.Id).ConfigureAwait(false);
                    var assignmentIds = assignments.Select(x => x.Id).ToList();

                    var assignmentSubmissions = await _unitOfWork.GetRepository<AssignmentSubmission>().GetAllAsync(
                        predicate: p => assignmentIds.Contains(p.AssignmentId)).ConfigureAwait(false);
                    if (assignmentSubmissions.Count > 0)
                    {
                        _logger.LogWarning("DeleteLessonAsync(): Lesson with id:{lessonId} and type: {type} contains assignmentSubmissions",
                                           lesson.Id, lesson.Type);
                        throw new EntityNotFoundException($"Assignment contains submission for lesson with type: {lesson.Type}");
                    }

                    var assignmentAttachments = await _unitOfWork.GetRepository<AssignmentAttachment>().GetAllAsync(
                        predicate: p => assignmentIds.Contains(p.AssignmentId)).ConfigureAwait(false);

                    var assignmentOptions = await _unitOfWork.GetRepository<AssignmentQuestionOption>().GetAllAsync(
                        predicate: p => assignmentIds.Contains(p.AssignmentId)).ConfigureAwait(false);

                    _unitOfWork.GetRepository<AssignmentQuestionOption>().Delete(assignmentOptions);
                    _unitOfWork.GetRepository<AssignmentAttachment>().Delete(assignmentAttachments);
                    _unitOfWork.GetRepository<Assignment>().Delete(assignments);
                }

                _unitOfWork.GetRepository<Lesson>().Delete(lesson);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);


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
                    predicate: p => p.Id == currentUserId && p.IsActive).ConfigureAwait(false);
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
                    include: src => src.Include(x => x.User)).ConfigureAwait(false);
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
                lesson.Meeting = await _unitOfWork.GetRepository<Meeting>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == lesson.MeetingId, include: src => src.Include(x => x.ZoomLicense)).ConfigureAwait(false);
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

                var zoomSetting = await _zoomSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
                if (zoomSetting == null)
                {
                    _logger.LogWarning("GetJoinMeetingAsync(): Zoom setting not found for user with id: {id}", currentUserId);
                    throw new ServiceException("Zoom setting not found");
                }

                if (lesson.Meeting.MeetingNumber == default)
                {
                    await _zoomLicenseService.CreateZoomMeetingAsync(lesson);
                }

                var signature = await _zoomLicenseService.GenerateZoomSignatureAsync(lesson.Meeting.MeetingNumber.ToString(), isModerator).ConfigureAwait(false);
                var zak = await _zoomLicenseService.GetZAKAsync(lesson.Meeting.ZoomLicense.HostId).ConfigureAwait(false);

                var response = new MeetingJoinResponseModel
                {
                    RoomName = lesson.Name,
                    JwtToken = signature,
                    ZAKToken = zak,
                    SdkKey = zoomSetting.SdkKey,
                    MeetingId = lesson.Meeting?.MeetingNumber,
                    Passcode = lesson.Meeting?.Passcode,
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to join live class.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to join live class.");
            }
        }

        /// <summary>
        /// Handle to reorder lesson
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="LessonReorderRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task ReorderAsync(string identity, LessonReorderRequestModel model, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("ReorderAsync(): Course with identity : {identity} not found for user with id :{userId}.", identity, currentUserId);
                    throw new EntityNotFoundException("Course was not found");
                }

                var section = await _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(
                    predicate: p => p.CourseId == course.Id && (p.Id.ToString() == model.SectionIdentity || p.Slug == model.SectionIdentity)
                    ).ConfigureAwait(false);
                if (section == null)
                {
                    _logger.LogWarning("ReorderAsync(): Section with identity : {identity} not found for course with id : {id} and user with id :{userId}",
                        model.SectionIdentity, course.Id, currentUserId);
                    throw new EntityNotFoundException("Section was not found");
                }

                var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id && model.Ids.Contains(p.Id)).ConfigureAwait(false);

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
                _logger.LogError(ex, "An error occurred while attempting to reorder the lessons");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to reorder the lessons");
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
            var currentLessonWatched = course.CourseEnrollments.FirstOrDefault(x => x.UserId == currentUserId);
            var currentLessonId = currentLessonWatched?.CurrentLessonId;
            if (currentLessonId == default)
            {
                var watchHistories = await _unitOfWork.GetRepository<WatchHistory>().GetAllAsync(
                   predicate: p => p.CourseId == course.Id && p.UserId == currentUserId).ConfigureAwait(false);
                if (watchHistories.Count == 0)
                {
                    var section = await _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(
                        predicate: p => p.CourseId == course.Id,
                        orderBy: o => o.OrderBy(x => x.Order),
                        include: src => src.Include(x => x.Lessons)).ConfigureAwait(false);
                    currentLessonId = section.Lessons.OrderBy(x => x.Order).FirstOrDefault()?.Id;
                }
                else
                {
                    currentLessonId = watchHistories.OrderByDescending(x => x.UpdatedOn).FirstOrDefault()?.LessonId;
                }
            }
            var currentLesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == currentLessonId,
                    include: src => src.Include(x => x.User)
                                .Include(x => x.Course)
                                .Include(x => x.Section)
                   ).ConfigureAwait(false);
            if (currentLesson == null)
            {
                _logger.LogWarning("Current watch lesson not found for course with id : {courseId} and user with id : {userId}", course.Id, currentUserId);
                throw new EntityNotFoundException("Current watched lesson not found");
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
            await _unitOfWork.GetRepository<QuestionSet>().InsertAsync(lesson.QuestionSet).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to update question set
        /// </summary>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="existingLesson">the instance of <see cref="Lesson"/></param>
        /// <returns></returns>
        private async Task UpdateQuestionSetAsync(LessonRequestModel model, Lesson existingLesson)
        {
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

            _unitOfWork.GetRepository<QuestionSet>().Update(existingLesson.QuestionSet);
            await Task.FromResult(0);
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
            lesson.Duration = model.Meeting.MeetingDuration;

            await _unitOfWork.GetRepository<Meeting>().InsertAsync(lesson.Meeting).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to update meeting 
        /// </summary>
        /// <param name="model">the instance of <see cref="LessonRequestModel"/></param>
        /// <param name="existingLesson">the instance of <see cref="Lesson"/></param>
        /// <returns></returns>
        private async Task UpdateMeetingAsync(LessonRequestModel model, Lesson existingLesson)
        {
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
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == entity.CourseId && x.SectionId == entity.SectionId && !x.IsDeleted,
                orderBy: x => x.OrderByDescending(x => x.Order)).ConfigureAwait(false);
            return lesson != null ? lesson.Order + 1 : 1;
        }

        #endregion Private Methods
    }
}