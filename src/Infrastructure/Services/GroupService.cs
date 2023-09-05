namespace Lingtren.Infrastructure.Services
{
    using Hangfire;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;
    using System.Net;

    public class GroupService : BaseGenericService<Group, GroupBaseSearchCriteria>, IGroupService
    {
        private readonly IMediaService _mediaService;
        private readonly IFileServerService _fileServerService;
        public GroupService(
            IUnitOfWork unitOfWork,
            ILogger<GroupService> logger,
            IMediaService mediaService,
            IFileServerService fileServerService,
            IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {
            _mediaService = mediaService;
            _fileServerService = fileServerService;
        }

        #region Protected Region

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
                 || x.User.FirstName.ToLower().Trim().Contains(search)
                 || x.User.Email.ToLower().Trim().Contains(search));
            }
            if (criteria.Role != UserRole.SuperAdmin && criteria.Role != UserRole.Admin)
            {
                predicate = predicate.And(p => p.GroupMembers.Any(x => x.UserId == criteria.CurrentUserId && x.IsActive));
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
                .Include(x => x.User)
                .Include(x => x.GroupMembers)
                .Include(x => x.Courses)
                .Include(x => x.GroupFiles);
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
        /// Check if entity could be accessed by current user
        /// </summary>
        /// <param name="entityToReturn">The entity being returned</param>
        protected override async Task CheckGetPermissionsAsync(Group entityToReturn, Guid? CurrentUserId = null)
        {
            if (!CurrentUserId.HasValue)
            {
                _logger.LogWarning("CurrentUserId is required.");
                throw new ForbiddenException(_localizer.GetString("CurrentUserRequired"));
            }
            var userAccess = await ValidateUserCanAccessGroup(entityToReturn.Id, CurrentUserId.Value).ConfigureAwait(false);
            var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(CurrentUserId.Value).ConfigureAwait(false);
            if (!userAccess && !isSuperAdminOrAdmin)
            {
                _logger.LogWarning("User with id: {userId} is not authorized user to access the group with id: {groupId}", CurrentUserId.Value, entityToReturn.Id);
                throw new ForbiddenException(_localizer.GetString("UserCannotAccessGroup"));
            }

        }

        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="entity">the instance of <see cref="Assignment"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(Group entity, Guid currentUserId)
        {
            var courseCount = await _unitOfWork.GetRepository<Course>().CountAsync(
                predicate: p => p.GroupId == entity.Id && (p.IsUpdate || p.Status == CourseStatus.Draft)
                ).ConfigureAwait(false);

            if (courseCount > 0)
            {
                _logger.LogWarning("Group with id: {id} cannot be removed since some trainings is associated with it.", entity.Id);
                throw new ForbiddenException(_localizer.GetString("TrainingAssociateToGroupCannotRemoved"));
            }

            var groupFiles = await _unitOfWork.GetRepository<GroupFile>().GetAllAsync(predicate: p => p.GroupId == entity.Id).ConfigureAwait(false);
            var groupMembers = await _unitOfWork.GetRepository<GroupMember>().GetAllAsync(predicate: p => p.GroupId == entity.Id).ConfigureAwait(false);

            _unitOfWork.GetRepository<GroupFile>().Delete(groupFiles);
            _unitOfWork.GetRepository<GroupMember>().Delete(groupMembers);
        }


        #endregion Protected Region

        #region Group Member

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
                    throw new ForbiddenException(_localizer.GetString("EnterUserEmail"));
                }
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(
                    predicate: p => p.Slug.ToLower().Equals(identity) || p.Id.ToString().Equals(identity),
                    include: source => source.Include(x => x.Courses)).ConfigureAwait(false);

                CommonHelper.CheckFoundEntity(group);

                var isAdminOrTeacher = await _unitOfWork.GetRepository<User>().ExistsAsync(
                    predicate: p => p.Id == currentUserId && (p.Role == UserRole.Admin || p.Role == UserRole.Trainer)
                                && p.Status == UserStatus.Active).ConfigureAwait(false);

                var isAccess = await IsSuperAdminOrAdminOrTrainer(currentUserId).ConfigureAwait(false);
                if (!isAccess)
                {
                    _logger.LogWarning("User with userId : {userId} is not admin/teacher to add member in the group", currentUserId);
                    throw new ForbiddenException(_localizer.GetString("OnlySuperAdminTranerAccessToAddMember"));
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

                if (inActiveUsers?.Count > 0)
                {
                    inActiveUsers.ForEach(x =>
                    {
                        x.IsActive = true;
                        x.UpdatedBy = currentUserId;
                        x.UpdatedOn = currentTimeStamp;
                    });
                    _unitOfWork.GetRepository<GroupMember>().Update(inActiveUsers);
                }

                if (group.Courses?.Count > 0)
                {
                    var unenrollUsers = inActiveUsers?.Select(x => x.UserId).ToList();
                    unenrollUsers?.AddRange(usersToBeAdded);
                    if (unenrollUsers?.Count > 0)
                    {
                        var inactiveEnrollment = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(predicate: p => p.IsDeleted && p.EnrollmentMemberStatus
                                           == EnrollmentMemberStatusEnum.Unenrolled && unenrollUsers.Contains(p.UserId) && group.Courses.Select(x =>
                                           x.Id).Contains(p.CourseId)).ConfigureAwait(false);
                        if (inactiveEnrollment?.Count > 0)
                        {
                            inactiveEnrollment.ForEach(x =>
                            {
                                x.IsDeleted = false;
                                x.EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Enrolled;
                                x.UpdatedBy = currentUserId;
                                x.UpdatedOn = currentTimeStamp;
                            });
                            _unitOfWork.GetRepository<CourseEnrollment>().Update(inactiveEnrollment);
                        }
                    }
                }
                await _unitOfWork.GetRepository<GroupMember>().InsertAsync(groupMembers).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                if (usersToBeAdded.ToList().Count != default)
                {
                    BackgroundJob.Enqueue<IHangfireJobService>(job => job.SendMailNewGroupMember(group.Name, group.Slug, usersToBeAdded.ToList(), null));
                }

                var result = new GroupAddMemberResponseModel();

                if (duplicateUsers.Count > 0 || nonUsers.Count > 0 || adminAndSuperAdmin.Count > 0)
                {
                    result.HttpStatusCode = HttpStatusCode.PartialContent;
                    if (duplicateUsers.Count > 0)
                    {
                        result.Message += _localizer.GetString($"AlreadyAddedMember") + " : " + string.Join(", ", duplicateUsers.Select(x => x.User.Email)) + Environment.NewLine;

                    }
                    //if (inActiveUsers.Count > 0)
                    //{
                    //    result.Message = _localizer.GetString("InactiveGroupMember") + " : " + string.Join(", ", inActiveUsers.Select(x => x.User.Email)) + Environment.NewLine;
                    //}
                    if (nonUsers.Count > 0)
                    {
                        result.Message += _localizer.GetString("NotASystemUser") + " : " + string.Join(", ", adminAndSuperAdmin.Select(x => x.Email)) + Environment.NewLine;
                    }
                    if (adminAndSuperAdmin.Count > 0)
                    {
                        result.Message += _localizer.GetString("AdminOrSuperAdmin") + " : " + string.Join(", ", adminAndSuperAdmin.Select(x => x.Email)) + Environment.NewLine;
                    }
                    result.Message = result.Message.TrimStart(' ', '&');
                    if (usersToBeAdded.Any())
                    {
                        result.Message += _localizer.GetString("OtherAddedSuccessfully");
                        result.Message += " & Other remaining users are added successfully in the group";
                    }
                }
                else
                {
                    result.Message = _localizer.GetString("GroupMemberAdded");
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
            await ExecuteAsync(async () =>
            {
                var group = await GetByIdOrSlugAsync(identity).ConfigureAwait(false);
                if (group == null)
                {
                    _logger.LogWarning("Group not found with identity : {identity}.", identity);
                    throw new EntityNotFoundException(_localizer.GetString("GroupNotFound"));
                }
                var isAccess = await IsSuperAdminOrAdminOrTrainer(currentUserId).ConfigureAwait(false);
                if (!isAccess)
                {
                    _logger.LogWarning("User with userId : {userId} is not admin/teacher to remove member from the group.", currentUserId);
                    throw new ForbiddenException(_localizer.GetString("OnlySuperAdminTrainerAllowedToRemoveMember"));
                }
                var groupMember = await _unitOfWork.GetRepository<GroupMember>().GetFirstOrDefaultAsync(
                    predicate: p => p.GroupId == group.Id && p.Id == id).ConfigureAwait(false);
                if (groupMember == null)
                {
                    _logger.LogWarning("Group member with id : {id} not found in the group with id : {groupId}.", id, group.Id);
                    throw new ForbiddenException(_localizer.GetString("GroupMemberNotFound"));
                }
                groupMember.IsActive = enabled;
                groupMember.UpdatedBy = currentUserId;
                groupMember.UpdatedOn = DateTime.UtcNow;
                _unitOfWork.GetRepository<GroupMember>().Update(groupMember);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            });
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
            await ExecuteAsync(async () =>
            {
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(predicate: x => x.Id.ToString() == identity ||
                            x.Slug.Equals(identity), include: source => source.Include(x => x.GroupMembers.Where(x => x.Id == id)).
                            Include(x => x.Courses).ThenInclude(x => x.CourseTeachers)).ConfigureAwait(false);
                if (group == null)
                {
                    _logger.LogWarning("Group not found with identity : {identity}.", identity);
                    throw new EntityNotFoundException(_localizer.GetString("GroupNotFound"));
                }

                var isAccess = await IsSuperAdminOrAdminOrTrainer(currentUserId).ConfigureAwait(false);
                if (!isAccess)
                {
                    _logger.LogWarning("User with userId : {userId} is not admin/teacher to remove member from the group.", currentUserId);
                    throw new ForbiddenException(_localizer.GetString("OnlySuperAdminTrainerAllowedToRemoveMember"));
                }
                var groupMember = group.GroupMembers.FirstOrDefault(x => x.Id == id);
                if (groupMember == null)
                {
                    _logger.LogWarning("Group member with id : {id} not found in the group with id : {groupId}.", id, group.Id);
                    throw new ForbiddenException(_localizer.GetString("GroupMemberNotFound"));
                }

                if (group.Courses.Any())
                {
                    var courseTeacher = group.Courses.SelectMany(x => x.CourseTeachers.Where(x => x.UserId == groupMember.UserId)).ToList();
                    var courseAuthor = group.Courses.Where(x => x.CreatedBy == groupMember.UserId).ToList();
                    courseAuthor.ForEach(x =>
                    {
                        x.CreatedBy = currentUserId;
                        x.UpdatedOn = DateTime.UtcNow;
                        x.UpdatedBy = currentUserId;
                    });

                    var courseEnrollmentUsers = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(predicate: p => p.UserId == groupMember.UserId &&
                                               group.Courses.Select(x => x.Id).Contains(p.CourseId)).ConfigureAwait(false);
                    if (courseEnrollmentUsers.Any())
                    {
                        courseEnrollmentUsers.ForEach(x =>
                        {
                            x.UpdatedOn = DateTime.UtcNow;
                            x.UpdatedBy = currentUserId;
                            x.IsDeleted = true;
                            x.EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Unenrolled;
                        });
                        _unitOfWork.GetRepository<CourseEnrollment>().Update(courseEnrollmentUsers);
                    }
                    _unitOfWork.GetRepository<Course>().Update(courseAuthor);
                    _unitOfWork.GetRepository<CourseTeacher>().Delete(courseTeacher);
                }
                groupMember.IsActive = false;
                groupMember.UpdatedBy = currentUserId;
                groupMember.UpdatedOn = DateTime.UtcNow;
                _unitOfWork.GetRepository<GroupMember>().Update(groupMember);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            });

        }

        #endregion Group Member

        #region Group File

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
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == model.GroupIdentity ||
                             p.Slug.Equals(model.GroupIdentity)).ConfigureAwait(false);
                if (group == null)
                {
                    _logger.LogWarning("Group with identity: {identity} not found.", model.GroupIdentity);
                    throw new EntityNotFoundException(_localizer.GetString("GroupNotFound"));
                }

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                var isGroupTeacher = await IsGroupTeacher(group.Id, currentUserId).ConfigureAwait(false);

                if (!isSuperAdminOrAdmin && !isGroupTeacher)
                {
                    _logger.LogWarning("User with id: {userId} is not super-admin or admin or teacher for the group with id :{groupId}.", currentUserId, group.Id);
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUserToCreateAttachment"));
                }

                var url = await _mediaService.UploadGroupFileAsync(model.File).ConfigureAwait(false);

                var groupFile = new GroupFile
                {
                    Id = Guid.NewGuid(),
                    Name = model.File.FileName,
                    Url = url,
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
                _logger.LogError(ex, "An error occurred while trying to upload the file in the group.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredUploadFileToGroup"));
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
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id.ToString() == groupIdentity || p.Slug == groupIdentity
                    ).ConfigureAwait(false);
                if (group == null)
                {
                    _logger.LogWarning("Group with identity : {identity} not found.", groupIdentity);
                    throw new EntityNotFoundException(_localizer.GetString("GroupNotFound"));
                }

                var file = await _unitOfWork.GetRepository<GroupFile>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == fileId && p.GroupId == group.Id
                    ).ConfigureAwait(false);
                if (file == null)
                {
                    _logger.LogWarning("File with id : {fileId} not found for group with id: {groupId}.", fileId, group.Id);
                    throw new EntityNotFoundException(_localizer.GetString("FileNotFound"));
                }

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (file.CreatedBy != currentUserId && !isSuperAdminOrAdmin)
                {
                    _logger.LogWarning("User with id: {userId} is not authorized user to remove file from the group with id : {groupId}.", currentUserId, group.Id);
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUserToRemoveFileFromGroup"));
                }

                _unitOfWork.GetRepository<GroupFile>().Delete(file);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(file.Url))
                {
                    await _fileServerService.RemoveFileAsync(file.Url).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to remove the file from group.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurrecdRemoveFileFromGroup"));
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
                var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id.ToString() == searchCriteria.GroupIdentity || p.Slug.Equals(searchCriteria.GroupIdentity)
                    ).ConfigureAwait(false);
                if (group == null)
                {
                    _logger.LogWarning("Group with identity: {identity} not found.", searchCriteria.GroupIdentity);
                    throw new EntityNotFoundException(_localizer.GetString("GroupNotFound"));
                }

                var userAccess = await ValidateUserCanAccessGroup(group.Id, searchCriteria.CurrentUserId).ConfigureAwait(false);
                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(searchCriteria.CurrentUserId).ConfigureAwait(false);
                if (!userAccess && !isSuperAdminOrAdmin)
                {
                    _logger.LogWarning("User with id: {userId} is not authorized user to access the group with id: {groupId}.", searchCriteria.CurrentUserId, group.Id);
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var files = await _unitOfWork.GetRepository<GroupFile>().GetAllAsync(
                    predicate: p => p.GroupId == group.Id,
                    include: src => src.Include(x => x.User)).ConfigureAwait(false);
                if (files.Count != default && !string.IsNullOrEmpty(searchCriteria.Search))
                {
                    files = files.Where(x => x.Name.ToLower().Trim().Contains(searchCriteria.Search)).ToList();
                }

                return files.ToIPagedList(searchCriteria.Page, searchCriteria.Size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch the group files.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFtechGroupFiles"));
            }
        }

        #endregion Group File

        #region Private Methods

        /// <summary>
        /// Handle to get whether user is group member or not with teacher role
        /// </summary>
        /// <param name="groupId">the group id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the boolean true or false</returns>
        private async Task<bool> IsGroupTeacher(Guid groupId, Guid currentUserId)
        {
            var groupMember = await _unitOfWork.GetRepository<GroupMember>().GetFirstOrDefaultAsync(
               predicate: p => p.GroupId == groupId && p.UserId == currentUserId && p.IsActive,
               include: src => src.Include(x => x.User)
               ).ConfigureAwait(false);
            return groupMember?.User.Role == UserRole.Trainer;
        }

        #endregion Private Methods

        /// <summary>
        /// Handle to get users who is not present in members
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the search result of <see cref="UserModel"/></returns>
        public async Task<SearchResult<UserModel>> GetNonGroupMembers(string identity, GroupBaseSearchCriteria criteria)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var group = await GetByIdOrSlugAsync(identity, criteria.CurrentUserId).ConfigureAwait(false);
                if (group == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("GroupNotFound"));
                }

                var predicate = PredicateBuilder.New<User>(true);
                if (!string.IsNullOrWhiteSpace(criteria.Search))
                {
                    var search = criteria.Search.ToLower().Trim();
                    predicate = predicate.And(x =>
                        ((x.FirstName.Trim() + " " + x.MiddleName.Trim()).Trim() + " " + x.LastName.Trim()).Trim().Contains(search)
                     || x.Email.ToLower().Trim().Contains(search)
                     || x.MobileNumber.ToLower().Trim().Contains(search));
                }
                if (!string.IsNullOrWhiteSpace(criteria.DepartmentIdentity))
                {
                    var departmentId = criteria.DepartmentIdentity.ToLower().Trim();
                    predicate = predicate.And(x => x.DepartmentId.ToString() == departmentId || x.Department.Slug.ToLower().Trim() == departmentId);
                }
                predicate = predicate.And(p => !p.GroupMembers.Any(x => x.GroupId == group.Id && x.UserId == p.Id && x.IsActive));
                predicate = predicate.And(p => p.Status == UserStatus.Active && (p.Role != UserRole.SuperAdmin && p.Role != UserRole.Admin));

                var users = await _unitOfWork.GetRepository<User>().GetAllAsync(predicate, include: (x) => x.Include(p => p.Department)).ConfigureAwait(false);
                var result = users.ToIPagedList(criteria.Page, criteria.Size);
                var response = new SearchResult<UserModel>
                {
                    Items = new List<UserModel>(),
                    CurrentPage = result.CurrentPage,
                    PageSize = result.PageSize,
                    TotalCount = result.TotalCount,
                    TotalPage = result.TotalPage
                };
                result.Items.ForEach(x => response.Items.Add(new UserModel(x)));
                return response;
            });
        }

        public Task<GroupAddMemberResponseModel> AddMembersByDepartment(string identity, string departmentId, Guid currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
