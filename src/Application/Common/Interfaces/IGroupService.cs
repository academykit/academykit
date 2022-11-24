namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;

    public interface IGroupService : IGenericService<Group, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to add member in the group
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="model">the instance of <see cref="AddGroupMemberRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        Task<GroupAddMemberResponseModel> AddMemberAsync(string identity, AddGroupMemberRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to change group member status
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="id">the group member id</param>
        /// <param name="enabled">the boolean value for status</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        Task ChangeMemberStatusAsync(string identity, Guid id, bool enabled, Guid currentUserId);

        /// <summary>
        /// Handle to remove member from the group
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="id">the group member id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        Task RemoveMemberAsync(string identity, Guid id, Guid currentUserId);
    }
}
