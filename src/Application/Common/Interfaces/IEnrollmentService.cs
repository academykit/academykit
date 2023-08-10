using Lingtren.Application.Common.Dtos;
using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Interfaces
{
    public interface IEnrollmentService:IGenericService<User,BaseSearchCriteria>
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
    }
}
