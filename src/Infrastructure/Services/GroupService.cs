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
    using System.Linq.Expressions;
    using System.Net;
    using System.Runtime.InteropServices;

    public class GroupService : BaseGenericService<Group, GroupBaseSearchCriteria>, IGroupService
    {
        private readonly IMediaService _mediaService;
        public GroupService(IUnitOfWork unitOfWork,
            ILogger<GroupService> logger, IMediaService mediaService) : base(unitOfWork, logger)
        {
            _mediaService = mediaService;
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Group entity)
        {
            entity.Slug = CommonHelper.GetEntityTitleSlug<Group>(_unitOfWork, (slug) => q => q.Slug == slug, entity.Name);
            await _unitOfWork.GetRepository<GroupMember>().InsertAsync(entity.GroupMembers).ConfigureAwait(false);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Group, bool>> ConstructQueryConditions(Expression<Func<Group, bool>> predicate, GroupBaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search)
                 || x.User.FirstName.ToLower().Trim().Contains(search));
            }
            if (criteria.Role != UserRole.SuperAdmin && criteria.Role != UserRole.Admin)
            {
                predicate = predicate.And(p => p.GroupMembers.Any(x => x.UserId == criteria.CurrentUserId));
            }
            return predicate;
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(GroupBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(Group.CreatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Group, object> IncludeNavigationProperties(IQueryable<Group> query)
        {
            return query
                .Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Group, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }

        /// <summary>
        /// Handle to add member in the group
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="model">the instance of <see cref="AddGroupMemberRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        public async Task<GroupAddMemberResponseModel> AddMemberAsync(string identity, AddGroupMemberRequestModel model, Guid currentUserId)
        {
            return await ExecuteWithResultAsync(async () =>
            {

                if (model.Emails.Any(_ => default))
                {
                    _logger.LogInformation("Please enter user email for group with identity : {identity}", identity);
                    throw new ForbiddenException("Please enter user email.");
                }
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(
                    predicate: p => p.Slug.ToLower().Equals(identity) || p.Id.ToString().Equals(identity)).ConfigureAwait(false);

                CommonHelper.CheckFoundEntity(group);

                var isAdminOrTeacher = await _unitOfWork.GetRepository<User>().ExistsAsync(
                    predicate: p => p.Id == currentUserId && (p.Role == UserRole.Admin || p.Role == UserRole.Trainer)
                                && p.IsActive).ConfigureAwait(false);

                var isAccess = await IsSuperAdminOrAdminOrTrainer(currentUserId).ConfigureAwait(false);
                if (!isAccess)
                {
                    _logger.LogWarning("User with userId : {userId} is not admin/teacher to add member in the group", currentUserId);
                    throw new ForbiddenException("Only user with superadmin or admin or trainer role is allowed to add member in the group.");
                }

                var users = await _unitOfWork.GetRepository<User>().GetAllAsync(
                    predicate: p => model.Emails.Contains(p.Email)).ConfigureAwait(false);

                var userIds = users.Select(x => x.Id).ToList();

                var nonUsers = model.Emails.Select(x => x.Trim().ToLower()).ToList().Except(users.Select(x => x.Email.Trim().ToLower())).ToList();

                var duplicateUsers = await _unitOfWork.GetRepository<GroupMember>().GetAllAsync(
                    predicate: p => p.GroupId == group.Id && userIds.Contains(p.UserId) && p.IsActive,
                    include: src => src.Include(x => x.User)).ConfigureAwait(false);

                var inActiveUsers = await _unitOfWork.GetRepository<GroupMember>().GetAllAsync(
                    predicate: p => p.GroupId == group.Id && userIds.Contains(p.UserId) && !p.IsActive,
                    include: src => src.Include(x => x.User)).ConfigureAwait(false);
               
                var adminAndSuperAdmin = await _unitOfWork.GetRepository<User>().GetAllAsync(
                    predicate: p => (p.Role == UserRole.SuperAdmin || p.Role == UserRole.Admin) && model.Emails.Contains(p.Email)).ConfigureAwait(false);

                var usersToBeAdded = userIds.Except(duplicateUsers.Select(x => x.UserId))
                                            .Except(inActiveUsers.Select(x => x.UserId))
                                            .Except(adminAndSuperAdmin.Select(x => x.Id));

                var groupMembers = new List<GroupMember>();
                var currentTimeStamp = DateTime.UtcNow;
                foreach (var userId in usersToBeAdded)
                {
                    groupMembers.Add(new GroupMember()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        GroupId = group.Id,
                        IsActive = true,
                        CreatedBy = currentUserId,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = currentUserId,
                        UpdatedOn = currentTimeStamp
                    });
                }
                await _unitOfWork.GetRepository<GroupMember>().InsertAsync(groupMembers).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                var result = new GroupAddMemberResponseModel();

                if (duplicateUsers.Count > 0 || inActiveUsers.Count > 0 || nonUsers.Count > 0 || adminAndSuperAdmin.Count > 0)
                {
                    result.HttpStatusCode = HttpStatusCode.PartialContent;
                    if (duplicateUsers.Count > 0)
                    {
                        result.Message += $"{string.Join(',', duplicateUsers.Select(x => x.User.Email))} already exist in group member";
                    }
                    if (inActiveUsers.Count > 0)
                    {
                        result.Message += $" & {string.Join(',', inActiveUsers.Select(x => x.User.Email))} already exist as inactive group member";
                    }
                    if (nonUsers.Count > 0)
                    {
                        result.Message += $" & {string.Join(',', nonUsers.Select(x => x))} is not a users in the system";
                    }
                    if(adminAndSuperAdmin.Count > 0)
                    {
                        result.Message += $" & {string.Join(',', adminAndSuperAdmin.Select(x => x.Email))} is a {string.Join(',', adminAndSuperAdmin.Select(x => x.Role))} in the system";
                    }
                    result.Message = result.Message.TrimStart(' ', '&');
                    if (usersToBeAdded.Count() > 0)
                    {
                        result.Message += " & Other remaining users added successfully in the group";
                    }
                }
                else
                {
                    result.Message = "Group Member Added Successfully";
                }
                return result;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to change group member status
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="id">the group member id</param>
        /// <param name="enabled">the boolean value for status</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public async Task ChangeMemberStatusAsync(string identity, Guid id, bool enabled, Guid currentUserId)
        {
            var group = await GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            if (group == null)
            {
                _logger.LogWarning("Group not found with identity : {identity}", identity);
                throw new EntityNotFoundException("Group not found");
            }
            var isAccess = await IsSuperAdminOrAdminOrTrainer(currentUserId).ConfigureAwait(false);
            if (!isAccess)
            {
                _logger.LogWarning("User with userId : {userId} is not admin/teacher to remove member from the group", currentUserId);
                throw new ForbiddenException("Only user with superadmin or admin or trainer role is allowed to remove member from the group.");
            }
            var groupMember = await _unitOfWork.GetRepository<GroupMember>().GetFirstOrDefaultAsync(
                predicate: p => p.GroupId == group.Id && p.Id == id).ConfigureAwait(false);
            if (groupMember == null)
            {
                _logger.LogWarning("Group member with id : {id} not found in the group with id : {groupId}", id, group.Id);
                throw new ForbiddenException("Group member not found");
            }
            groupMember.IsActive = enabled;
            groupMember.UpdatedBy = currentUserId;
            groupMember.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<GroupMember>().Update(groupMember);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to remove member from the group
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="id">the group member id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public async Task RemoveMemberAsync(string identity, Guid id, Guid currentUserId)
        {
            var group = await GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            if (group == null)
            {
                _logger.LogWarning("Group not found with identity : {identity}", identity);
                throw new EntityNotFoundException("Group not found");
            }
            var isAccess = await IsSuperAdminOrAdminOrTrainer(currentUserId).ConfigureAwait(false);
            if (!isAccess)
            {
                _logger.LogWarning("User with userId : {userId} is not admin/teacher to remove member from the group", currentUserId);
                throw new ForbiddenException("Only user with superadmin or admin or trainer role is allowed to remove member from the group.");
            }
            var groupMember = await _unitOfWork.GetRepository<GroupMember>().GetFirstOrDefaultAsync(
                predicate: p => p.GroupId == group.Id && p.Id == id).ConfigureAwait(false);
            if (groupMember == null)
            {
                _logger.LogWarning("Group member with id : {id} not found in the group with id : {groupId}", id, group.Id);
                throw new ForbiddenException("Group member not found");
            }
            _unitOfWork.GetRepository<GroupMember>().Delete(groupMember);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to upload file in group
        /// </summary>
        /// <param name="model"> the instance of <see cref="GroupFileRequestModel" />. </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="GroupFile" /> .</returns>
        public async Task<GroupFile> UploadGroupFileAsync(GroupFileRequestModel model, Guid currentUserId)
        {
            try
            {
                await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == model.GroupIdentity ||
                             p.Slug.Equals(model.GroupIdentity)).ConfigureAwait(false);
                if (group == null)
                {
                    _logger.LogError($"Group with id {model.GroupIdentity} not found");
                    throw new EntityNotFoundException("Group not found");
                }
                var groupFileDto = await _mediaService.UploadGroupFileAsync(model.File).ConfigureAwait(false);
                var groupFile = new GroupFile
                {
                    Id = Guid.NewGuid(),
                    Name = model.File.FileName,
                    Url = groupFileDto.Url,
                    Key = groupFileDto.Key,
                    MimeType = model.File.ContentType,
                    GroupId = group.Id,
                    CreatedBy = currentUserId,
                    Size = model.File.Length,
                    CreatedOn = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<GroupFile>().InsertAsync(groupFile).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return groupFile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to remove group file
        /// </summary>
        /// <param name="groupIdentity"> the group id or slug </param>
        /// <param name="fileId"> the file id</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task RemoveGroupFileAsync(string groupIdentity, Guid fileId, Guid currentUserId)
        {
            try
            {
                await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                var file = await _unitOfWork.GetRepository<GroupFile>().GetFirstOrDefaultAsync(predicate: x => x.Id == fileId).ConfigureAwait(false);
                if (file == null)
                {
                    _logger.LogError($"File with id : {fileId} not found");
                    throw new EntityNotFoundException("File not found");
                }

                _unitOfWork.GetRepository<GroupFile>().Delete(file);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get group files
        /// </summary>
        /// <param name="searchCriteria"> the instance of <see cref="GroupSearchCriteria" /> . </param>
        /// <returns> the list of <see cref="GroupFile" /> .</returns>
        public async Task<SearchResult<GroupFile>> GetGroupFilesAsync(GroupFileSearchCriteria searchCriteria)
        {
            try
            {
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(predicate : p => p.Id.ToString() == searchCriteria.GroupIdentity ||
                p.Slug.Equals(searchCriteria.GroupIdentity)).ConfigureAwait(false);
                if(group == null)
                {
                    _logger.LogError($"Group with identity {searchCriteria.GroupIdentity} not found");
                    throw new EntityNotFoundException("Group not found");
                }

                var userAccess = await ValidateUserCanAccessGroup(group.Id,searchCriteria.CurrentUserId).ConfigureAwait(false);
                if(!userAccess)
                {
                    throw new ForbiddenException("User can't access the group.");
                }

                var files = await _unitOfWork.GetRepository<GroupFile>().GetAllAsync(predicate: p => p.GroupId == group.Id).ConfigureAwait(false);
                if(files.Count != default && !string.IsNullOrEmpty(searchCriteria.Search))
                {
                    files = files.Where(x => x.Name.ToLower().Trim().Contains(searchCriteria.Search)).ToList();
                }
               return files.ToIPagedList(searchCriteria.Page,searchCriteria.Size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }
    }
}
