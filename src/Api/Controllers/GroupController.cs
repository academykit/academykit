namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class GroupController : BaseApiController
    {
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IValidator<GroupRequestModel> _validator;
        public GroupController(
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IValidator<GroupRequestModel> validator)
        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _validator = validator;
        }

        /// <summary>
        /// Search the groups.
        /// </summary>
        /// <param name="searchCriteria">The group search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        public async Task<SearchResult<GroupResponseModel>> SearchAsync([FromQuery] BaseSearchCriteria searchCriteria)
        {
            var searchResult = await _groupService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<GroupResponseModel>
            {
                Items = new List<GroupResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new GroupResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// group create api
        /// </summary>
        /// <param name="model"> the instance of <see cref="GroupRequestModel" /> .</param>
        /// <returns> the instance of <see cref="GroupResponseModel" /> .</returns>
        [HttpPost]
        public async Task<GroupResponseModel> CreateGroup(GroupRequestModel model)
        {
            var currentTimeStamp = DateTime.UtcNow;
            await _validator.ValidateAsync(model, options => options.IncludeRuleSets("Add").ThrowOnFailures()).ConfigureAwait(false);

            var entity = new Group()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                IsActive = true,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                GroupMembers = new List<GroupMember>()
            };

            entity.GroupMembers.Add(new GroupMember()
            {
                Id = Guid.NewGuid(),
                GroupId = entity.Id,
                UserId = CurrentUser.Id,
                IsActive = true,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
            });
            var response = await _groupService.CreateAsync(entity).ConfigureAwait(false);
            return new GroupResponseModel(response);
        }

        /// <summary>
        /// get group by id or slug
        /// </summary>
        /// <param name="identity"> the group id or slug</param>
        /// <returns> the instance of <see cref="GroupResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<GroupResponseModel> Get(string identity)
        {
            Group model = await _groupService.GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            return new GroupResponseModel(model);
        }

        /// <summary>
        /// update group
        /// </summary>
        /// <param name="groupId"> the group id</param>
        /// <param name="model"> the  instance of <see cref="GroupRequestModel" /> .</param>
        /// <returns> the instance of <see cref="GroupResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<GroupResponseModel> UpdateGroup(string identity, GroupRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.IncludeRuleSets("Update").ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _groupService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.IsActive = model.IsActive;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _groupService.UpdateAsync(existing).ConfigureAwait(false);
            return new GroupResponseModel(savedEntity);
        }

        /// <summary>
        /// group member search api
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="searchCriteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>

        [HttpGet("{identity}/members")]
        public async Task<SearchResult<GroupMemberResponseModel>> SearchGroupMembers(string identity, [FromQuery] BaseSearchCriteria searchCriteria)
        {
            var group = await _groupService.GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            if (group == null)
            {
                throw new EntityNotFoundException("Group not found");
            }
            GroupMemberBaseSearchCriteria criteria = new GroupMemberBaseSearchCriteria
            {
                GroupId = group.Id,
                CurrentUserId = searchCriteria.CurrentUserId,
                Page = searchCriteria.Page,
                Search = searchCriteria.Search,
                Size = searchCriteria.Size,
                SortBy = searchCriteria.SortBy,
                SortType = searchCriteria.SortType
            };
            var searchResult = await _groupMemberService.SearchAsync(criteria).ConfigureAwait(false);

            var response = new SearchResult<GroupMemberResponseModel>
            {
                Items = new List<GroupMemberResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new GroupMemberResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// Group member add api
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="model">the instance of <see cref="AddGroupMemberRequestModel"/></param>
        /// <returns>the instance of <see cref="GroupAddMemberResponseModel"/></returns>
        [HttpPost("{identity}/addMember")]
        public async Task<GroupAddMemberResponseModel> AddMember(string identity, AddGroupMemberRequestModel model)
        {
            var response = await _groupService.AddMemberAsync(identity, model, CurrentUser.Id).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// group member status change api
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="id">the group member id</param>
        /// <param name="enabled">the status</param>
        /// <returns></returns>
        [HttpPatch("{identity}/status/{id}")]
        public async Task<IActionResult> ChangeStatus(string identity, Guid id, [FromQuery] bool enabled)
        {
            await _groupService.ChangeMemberStatusAsync(identity, id, enabled, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Member status changed successfully." });
        }

        /// <summary>
        /// group member remove api
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="id">group member id</param>
        /// <returns></returns>

        [HttpDelete("{identity}/removeMember/{id}")]
        public async Task<IActionResult> RemoveMember(string identity, Guid id)
        {
            await _groupService.RemoveMemberAsync(identity, id, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Member removed successfully." });
        }
    }
}
