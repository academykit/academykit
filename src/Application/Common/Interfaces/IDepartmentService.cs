namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using System;
    using System.Threading.Tasks;

    public interface IDepartmentService : IGenericService<Department, DepartmentBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to get department users
        /// </summary>
        /// <param name="identity">the department id or slug</param>
        /// <param name="searchCriteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task<SearchResult<UserResponseModel>> GetUsers(string identity, BaseSearchCriteria searchCriteria, Guid currentUserId);
    }
}
