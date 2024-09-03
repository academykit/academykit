using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Domain.Entities;

namespace AcademyKit.Infrastructure.Services
{
    public interface IPhysicalLessonServices
        : IGenericService<PhysicalLessonReview, BaseSearchCriteria>
    {
        /// <summary>
        /// Inserts or updates physical lesson stats
        /// </summary>
        /// <param name="lessonIdentity">Lesson identity</param>
        /// <param name="currentUserId">Current user id</param>
        /// <returns>Task completed</returns>
        /// <exception cref="ForbiddenException"></exception>
        Task PhysicalLessonAttendanceAsync(string lessonIdentity, string currentUserId);

        /// <summary>
        /// add review for physical lesson
        /// </summary>
        /// <param name="model">instance of <see cref="PhysicalLessonReviewRequestModel"/></param>
        /// <param name="currentUserId">current user id</param>
        /// <returns>Task completed</returns>
        Task PhysicalLessonReviewAsync(PhysicalLessonReviewRequestModel model, Guid currentUserId);
    }
}
