using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace AcademyKit.Infrastructure.Services
{
    public class PhysicalLessonService
        : BaseGenericService<PhysicalLessonReview, BaseSearchCriteria>,
            IPhysicalLessonServices
    {
        public PhysicalLessonService(
            IUnitOfWork unitOfWork,
            ILogger<CourseService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        /// <summary>
        /// Inserts or updates physical lesson stats
        /// </summary>
        /// <param name="lessonIdentity">Lesson identity</param>
        /// <param name="currentUserId">Current user id</param>
        /// <returns>Task completed</returns>
        /// <exception cref="ForbiddenException"></exception>
        public async Task PhysicalLessonAttendanceAsync(string lessonIdentity, string currentUserId)
        {
            await ExecuteAsync(async () =>
            {
                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(
                        predicate: p =>
                            p.Id.ToString() == lessonIdentity
                            || p.Slug.ToLower().Trim() == lessonIdentity.ToLower().Trim(),
                        include: src =>
                            src.Include(x => x.Course).ThenInclude(x => x.CourseEnrollments)
                    )
                    .ConfigureAwait(false);
                var physicalReview = new PhysicalLessonReview();
                var user = await _unitOfWork
                    .GetRepository<User>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == currentUserId)
                    .ConfigureAwait(false);
                if (user == default)
                {
                    throw new ForbiddenException("UnauthorizedUser");
                }

                var isAuthorizedUser = await IsSuperAdminOrAdminOrTrainerOfTraining(
                        user.Id,
                        lesson.CourseId.ToString(),
                        TrainingTypeEnum.Course
                    )
                    .ConfigureAwait(false);
                var Review = await _unitOfWork
                    .GetRepository<PhysicalLessonReview>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.UserId.ToString() == currentUserId && p.LessonId == lesson.Id
                    )
                    .ConfigureAwait(false);
                if (
                    lesson.Course.CourseEnrollments.Any(x => x.UserId.ToString() == currentUserId)
                    && !isAuthorizedUser
                )
                {
                    if (Review == default)
                    {
                        physicalReview.UserId = user.Id;
                        physicalReview.CreatedOn = DateTime.UtcNow;
                        physicalReview.UpdatedOn = DateTime.UtcNow;
                        physicalReview.LessonId = lesson.Id;
                        physicalReview.UpdatedBy = user.Id;
                        physicalReview.CreatedBy = user.Id;
                        physicalReview.HasAttended = true;
                        physicalReview.Id = Guid.NewGuid();

                        await ManageStudentCourseComplete(
                                lesson.CourseId,
                                lesson.Id,
                                user.Id,
                                DateTime.UtcNow
                            )
                            .ConfigureAwait(false);
                        _unitOfWork.GetRepository<PhysicalLessonReview>().Insert(physicalReview);
                    }

                    if (Review != default)
                    {
                        physicalReview = Review;
                        physicalReview.UpdatedOn = DateTime.UtcNow;
                        physicalReview.UpdatedBy = user.Id;
                        _unitOfWork.GetRepository<PhysicalLessonReview>().Update(physicalReview);
                    }

                    var watchHistory = await _unitOfWork
                        .GetRepository<WatchHistory>()
                        .GetFirstOrDefaultAsync(predicate: p =>
                            p.LessonId == lesson.Id && p.UserId == user.Id
                        )
                        .ConfigureAwait(false);
                    if (watchHistory == default)
                    {
                        watchHistory = new WatchHistory
                        {
                            Id = Guid.NewGuid(),
                            LessonId = lesson.Id,
                            CourseId = lesson.CourseId,
                            UserId = user.Id,
                            IsCompleted = true,
                            CreatedBy = user.Id,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedBy = user.Id,
                            UpdatedOn = DateTime.UtcNow,
                        };
                        await _unitOfWork.GetRepository<WatchHistory>().InsertAsync(watchHistory);
                    }

                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                }
            });
        }

        /// <summary>
        /// add review for physical lesson
        /// </summary>
        /// <param name="model">instance of <see cref="PhysicalLessonReviewRequestModel"/></param>
        /// <param name="currentUserId">current user id</param>
        /// <returns>Task completed</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        public async Task PhysicalLessonReviewAsync(
            PhysicalLessonReviewRequestModel model,
            Guid currentUserId
        )
        {
            await ExecuteAsync(async () =>
            {
                var user = await _unitOfWork
                    .GetRepository<User>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Id == model.UserId)
                    .ConfigureAwait(false);
                if (user == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("UserNotFound"));
                }

                var lesson = await _unitOfWork
                    .GetRepository<Lesson>()
                    .GetFirstOrDefaultAsync(
                        predicate: p =>
                            p.Id.ToString() == model.Identity
                            || p.Slug.ToLower() == model.Identity.ToLower().Trim(),
                        include: src => src.Include(x => x.Course)
                    )
                    .ConfigureAwait(false);
                var IsAuthorized = await IsSuperAdminOrAdminOrTrainerOfTraining(
                        currentUserId,
                        lesson.Course.Id.ToString(),
                        TrainingTypeEnum.Course
                    )
                    .ConfigureAwait(false);
                if (!IsAuthorized)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var physicalLessonReview = new PhysicalLessonReview();
                var review = await _unitOfWork
                    .GetRepository<PhysicalLessonReview>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.UserId == model.UserId && p.LessonId == lesson.Id
                    )
                    .ConfigureAwait(false);
                if (review == default)
                {
                    physicalLessonReview.UserId = user.Id;
                    physicalLessonReview.CreatedOn = DateTime.UtcNow;
                    physicalLessonReview.UpdatedOn = DateTime.UtcNow;
                    physicalLessonReview.LessonId = lesson.Id;
                    physicalLessonReview.UpdatedBy = user.Id;
                    physicalLessonReview.CreatedBy = user.Id;
                    physicalLessonReview.HasAttended = true;
                    physicalLessonReview.IsReviewed = true;
                    physicalLessonReview.ReviewMessage = model.Message;
                    physicalLessonReview.Id = Guid.NewGuid();
                    var courseEnrollment = await _unitOfWork
                        .GetRepository<CourseEnrollment>()
                        .GetFirstOrDefaultAsync(predicate: p => p.CourseId == lesson.CourseId)
                        .ConfigureAwait(false);
                    await ManageStudentCourseComplete(
                            lesson.CourseId,
                            lesson.Id,
                            user.Id,
                            DateTime.UtcNow
                        )
                        .ConfigureAwait(false);
                    _unitOfWork.GetRepository<PhysicalLessonReview>().Insert(physicalLessonReview);
                }

                if (review != default)
                {
                    physicalLessonReview = review;
                    physicalLessonReview.UpdatedOn = DateTime.UtcNow;
                    physicalLessonReview.UpdatedBy = user.Id;
                    physicalLessonReview.IsReviewed = true;
                    physicalLessonReview.ReviewMessage = model.Message;
                    _unitOfWork.GetRepository<PhysicalLessonReview>().Update(physicalLessonReview);
                }

                var watchHistory = await _unitOfWork
                    .GetRepository<WatchHistory>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.UserId == user.Id && p.LessonId == lesson.Id
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
                        CreatedOn = DateTime.UtcNow,
                        UpdatedBy = currentUserId,
                        UpdatedOn = DateTime.UtcNow,
                    };
                    await _unitOfWork
                        .GetRepository<WatchHistory>()
                        .InsertAsync(watchHistory)
                        .ConfigureAwait(false);
                }

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            });
        }
    }
}
