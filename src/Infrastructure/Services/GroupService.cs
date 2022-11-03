namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;
    using System.Net;

    public class GroupService : BaseGenericService<Group, BaseSearchCriteria>, IGroupService
    {
        public GroupService(IUnitOfWork unitOfWork,
            ILogger<GroupService> logger) : base(unitOfWork, logger)
        {

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
            await Task.FromResult(0);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Group, bool>> ConstructQueryConditions(Expression<Func<Group, bool>> predicate, BaseSearchCriteria criteria)
        {

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search)
                 || x.User.FirstName.ToLower().Trim().Contains(search));
            }

            return predicate.And(x => x.CreatedBy == criteria.CurrentUserId);
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(BaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(Group.CreatedOn);
            criteria.SortType = SortType.Ascending;
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

        //public async Task<TeamSubTeamAddMemberResponseDto> AddTeamMember(string identity, AddGroupMemberRequestModel model, Guid currentUserId)
        //{
        //    if (model.Users.Any(x => string.IsNullOrEmpty(x)))
        //    {
        //        throw new ForbiddenException("Email or mobile number is required");
        //    }
        //    var group = await _unitOfWork.GetRepository<Group>().GetFirstOrDefaultAsync(predicate: p => p.Slug.ToLower().Equals(identity)
        //                                                            || p.Id.ToString().Equals(identity)).ConfigureAwait(false);

        //    CommonHelper.CheckFoundEntity(group);

        //    var isAdmin = await _unitOfWork.GetRepository<User>().ExistsAsync(
        //        predicate: p => p.Id == currentUserId && p.Role == UserRole.Admin && p.IsActive == true).ConfigureAwait(false);

        //    if (!isAdmin)
        //    {
        //        throw new ForbiddenException("Only user with role is allowed to access");
        //    }
        //    var duplicateAndNewUserDto = await GetDuplicateAndNewUsersAndValidateTeamSizeLimit(team, model.UserRequests);
        //    var validUsersToAddMembers = GetUserEmailOrPhoneNumbers(duplicateAndNewUserDto.ValidUsersToAdd);

        //    IDictionary<string, string> duplicateUsers = new Dictionary<string, string>();
        //    IList<string> notFoundUsers = new List<string>();

        //    //loop throw each user request
        //    var teamInvitationIds = new List<Guid>();
        //    foreach (var userEmailOrPhoneNumber in validUsersToAddMembers)
        //    {
        //        var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
        //         predicate: p => p.Email.Trim().ToLower() == userEmailOrPhoneNumber.Trim().ToLower() || p.MobileNumber.Trim() == userEmailOrPhoneNumber.Trim());

        //        if (user == null)
        //        {
        //            notFoundUsers.Add(new KeyValuePair<string, string>(user.UserId, userEmailOrPhoneNumber.EmailOrPhoneNumber));
        //        }
        //        if (duplicateUsers.ContainsKey(user.UserId))
        //        {
        //            continue;
        //        }
        //        duplicateUsers.Add(new KeyValuePair<string, string>(user.UserId, userEmailOrPhoneNumber.EmailOrPhoneNumber));

        //        // add as team member
        //        var groupMember = new GroupMember
        //        {
        //            CreatedBy = currentUserId,
        //            UpdatedBy = currentUserId,
        //            GroupId = group.Id,
        //            UserId = user.UserId,
        //            IsActive = true
        //        };
        //        await _teamUserService.CreateWithAutoSaveOff(newTeamUser, _unitOfWork).ConfigureAwait(false);
        //    }
        //    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        //    var result = new GroupAddMemberResponseModel();
        //    if (duplicateAndNewUserDto.FoundInTeamUser.Count > 0)
        //    {
        //        result.HttpStatusCode = HttpStatusCode.PartialContent;
        //        result.Messages += $"{string.Join(',', duplicateAndNewUserDto.FoundInTeamUser)} already exist in group member.";
        //    }
        //    if (duplicateAndNewUserDto.FoundAsInActiveTeamUser.Count > 0)
        //    {
        //        result.HttpStatusCode = HttpStatusCode.PartialContent;
        //        result.Messages += $" & {string.Join(',', duplicateAndNewUserDto.FoundAsInActiveTeamUser)} already exist as inactive group member.";
        //    }
        //    if (result.Messages.StartsWith(" &"))
        //    {
        //        result.Messages = result.Messages.Replace(" &", "");
        //    }
        //    return result;
        //}

        //private async Task<DuplicateAndNewUserForAddMemberDto> GetDuplicateAndNewUsers(Group group, List<string> requestedUsers)
        //{
        //    requestedUsers = requestedUsers.ConvertAll(x => x.ToLower().Trim());

        //    var users = await _unitOfWork.GetRepository<User>().GetAllAsync(predicate: p => requestedUsers.Contains(p.Email.ToLower())
        //                                                       || requestedUsers.Contains(p.MobileNumber.ToLower())).ConfigureAwait(false);
        //    var userIds = users.Select(x => x.Id).ToList();

        //    var notFoundUsers = requestedUsers.Except(users.Contains())

        //    var groupMembers = await _unitOfWork.GetRepository<GroupMember>().GetAllAsync(predicate: p => userIds.Contains(p.UserId)
        //                                                       && p.GroupId == group.Id).ConfigureAwait(false);

        //    var inActiveUsers = groupMembers.Where(x => !x.IsActive);

        //    var activeTeamUsers = groupMembers.Except(inActiveUsers);

        //    var foundInGroupMembers = requestedUsers.Where(x => activeTeamUsers.Select(x => x.Email).ToList().Contains(x)
        //                                            || activeTeamUsers.Select(x => x.MobileNumber).ToList().Contains(x)).ToList();

        //    var foundAsInActiveTeamUser = requestedUsers.Where(x => inActiveOrDeletedUser.Select(x => x.Email).ToList().Contains(x)
        //                                            || inActiveOrDeletedUser.Select(x => x.MobileNumber).ToList().Contains(x)).ToList();

        //    var usersToAddInTeamMember = requestedUsers.Except(foundInGroupMembers).Except(foundAsInActiveTeamUser).ToList();

        //    return new DuplicateAndNewUserForAddMemberDto
        //    {
        //        FoundInGroupMembers = foundInGroupMembers,
        //        NewUsers = usersToAddInTeamMember,
        //        FoundAsInActiveTeamUser = foundAsInActiveTeamUser
        //    };
        //}
    }
}
