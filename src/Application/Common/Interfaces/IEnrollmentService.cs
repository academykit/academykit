using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Models.ResponseModels;
using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Interfaces
{
    public interface IEnrollmentService:IGenericService<User, EnrollmentBaseSearchCritera>
    {
        /// <summary>
        /// enroll user in training
        /// </summary>
        /// <param name="email">user email or mobile number</param>
        /// <param name="currentUserId">current user id</param>
        /// <param name="courseIdentity">course id or slug</param>
        /// <returns>Task completed</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        Task<string> EnrollUserAsync(IList<string> emailOrMobileNumber, Guid currentUserId, string courseIdentity);

        /// <summary>
        /// to get unenrolled user Id in course
        /// </summary>
        /// <param name="critera">enrolled user search critera</param>
        /// <returns>Task completed</returns>
        Task<SearchResult<UserResponseModel>> GetUnenrolledUser(EnrollmentBaseSearchCritera critera);
    }
}
