using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;

namespace AcademyKit.Application.Common.Interfaces
{
    public interface IEnrollmentService : IGenericService<User, EnrollmentBaseSearchCriteria>
    {
        /// <summary>
        /// enroll user in training
        /// </summary>
        /// <param name="email">user email or mobile number</param>
        /// <param name="currentUserId">current user id</param>
        /// <param name="courseIdentity">course id or slug</param>
        /// <returns>Task completed</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        Task<string> EnrollUserAsync(
            IList<string> emailOrMobileNumber,
            Guid currentUserId,
            string courseIdentity
        );

        /// <summary>
        /// get filtered user list for course
        /// </summary>
        /// <param name="criteria">enrolled user search criteria</param>
        /// <returns>Task completed</returns>
        Task<SearchResult<UserResponseModel>> CourseUserSearchAsync(
            EnrollmentBaseSearchCriteria criteria
        );
    }
}
