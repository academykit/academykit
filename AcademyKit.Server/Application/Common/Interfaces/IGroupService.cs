﻿namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;

    public interface IGroupService : IGenericService<Group, GroupBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to add member in the group
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="model">the instance of <see cref="AddGroupMemberRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        Task<GroupAddMemberResponseModel> AddMemberAsync(
            string identity,
            AddGroupMemberRequestModel model,
            Guid currentUserId
        );

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

        /// <summary>
        /// Handle to upload file in group
        /// </summary>
        /// <param name="model"> the instance of <see cref="GroupFileRequestModel" />. </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="GroupFile" /> .</returns>
        Task<GroupFile> UploadGroupFileAsync(GroupFileRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to remove group file
        /// </summary>
        /// <param name="groupIdentity"> the group id or slug </param>
        /// <param name="fileId"> the file id</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task RemoveGroupFileAsync(string groupIdentity, Guid fileId, Guid currentUserId);

        /// <summary>
        /// Handle to get group files
        /// </summary>
        /// <param name="searchCriteria"> the instance of <see cref="GroupFileSearchCriteria" /> . </param>
        /// <returns> the list of <see cref="GroupFileResponseModel" /> .</returns>
        Task<SearchResult<GroupFile>> GetGroupFilesAsync(GroupFileSearchCriteria searchCriteria);

        /// <summary>
        /// Handle to get users who is not present in members
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the search result of <see cref="UserModel"/></returns>
        Task<SearchResult<UserModel>> GetNonGroupMembers(
            string identity,
            GroupBaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to get group members by department id
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="departmentId">the department id</param>
        /// <param name="id">The current user id</param>
        /// <returns></returns>
        Task<GroupAddMemberResponseModel> AddMembersByDepartment(
            string identity,
            string departmentId,
            Guid currentUserId
        );
    }
}
