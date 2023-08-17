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
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class GroupController : BaseApiController
    {
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IValidator<GroupRequestModel> _validator;
        private readonly ICourseService _courseService;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;
        public GroupController(
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IValidator<GroupRequestModel> validator,
            ICourseService courseService,
            IStringLocalizer<ExceptionLocalizer> localizer)

        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _validator = validator;
            _courseService = courseService;
            _localizer = localizer;
        }

        /// <summary>
        /// Search the groups.
        /// </summary>
        /// <param name="searchCriteria">The group search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        public async Task<SearchResult<GroupResponseModel>> SearchAsync([FromQuery] GroupBaseSearchCriteria searchCriteria)
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            searchCriteria.Role = CurrentUser.Role;
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
                 {
                     response.Items.Add(new GroupResponseModel(p));
                 }
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
            IsSuperAdminOrAdmin(CurrentUser.Role);
            var currentTimeStamp = DateTime.UtcNow;
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);

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
            var model = await _groupService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
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
            IsSuperAdminOrAdmin(CurrentUser.Role);
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
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
        /// delete department api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await _groupService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("GroupRemoved") });
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
            var group = await _groupService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false) ?? throw new EntityNotFoundException("Group not found.");
            GroupMemberBaseSearchCriteria criteria = new()
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
        /// api that searches non members of group
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="searchCriteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        [HttpGet("{identity}/notMembers")]
        public async Task<SearchResult<UserModel>> SearchNotGroupMembers(string identity, [FromQuery] GroupBaseSearchCriteria searchCriteria)
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await _groupService.GetNonGroupMembers(identity, searchCriteria).ConfigureAwait(false);
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
            return await _groupService.AddMemberAsync(identity, model, CurrentUser.Id).ConfigureAwait(false);
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
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("MemberStatus") });
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
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("MemberRemoved") });
        }

        /// <summary>
        /// Group member add api
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="model">the instance of <see cref="AddGroupMemberRequestModel"/></param>
        /// <returns>the instance of <see cref="GroupAddMemberResponseModel"/></returns>
        [HttpGet("{identity}/courses")]
        public async Task<SearchResult<CourseResponseModel>> Courses(string identity, [FromQuery] BaseSearchCriteria criteria)
        {
            criteria.CurrentUserId = CurrentUser.Id;
            var searchResult = await _courseService.GroupCourseSearchAsync(identity, criteria).ConfigureAwait(false);
            var response = new SearchResult<CourseResponseModel>
            {
                Items = new List<CourseResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
            {
                response.Items.Add(new CourseResponseModel(p, _courseService.GetUserCourseEnrollmentStatus(p, CurrentUser.Id)));
            });
            return response;
        }

        /// <summary>
        /// upload file api
        /// </summary>
        /// <param name="model"> the instance of <see cref="GroupFileRequestModel" /> . </param>
        /// <returns> the instance of <see cref="GroupFileResponseModel" /> .</returns>
        [HttpPost("file")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task<GroupFileResponseModel> UploadFile([FromForm] GroupFileRequestModel model)
        {
            var response = await _groupService.UploadGroupFileAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return new GroupFileResponseModel(response);
        }

        /// <summary>
        /// get group files api
        /// </summary>
        /// <param name="searchCriteria"> the instance of <see cref="GroupFileSearchCriteria" /> . </param>
        /// <returns> the list of <see cref="GroupFileResponseModel" /> .</returns>
        [HttpGet("files")]
        public async Task<SearchResult<GroupFileResponseModel>> Files([FromQuery] GroupFileSearchCriteria searchCriteria)
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            var searchResult = await _groupService.GetGroupFilesAsync(searchCriteria).ConfigureAwait(false);
            var response = new SearchResult<GroupFileResponseModel>
            {
                Items = new List<GroupFileResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };
            searchResult.Items.ForEach(p =>
            response.Items.Add(new GroupFileResponseModel(p)));
            return response;
        }

        /// <summary>
        /// delete group file api
        /// </summary>
        /// <param name="identity"> the group id or slug </param>
        /// <param name="fileId">th file id </param>
        /// <returns>the task complete </returns>
        [HttpDelete("{identity}/files/{fileId}")]
        public async Task<IActionResult> RemoveFile(string identity, Guid fileId)
        {
            await _groupService.RemoveGroupFileAsync(identity, fileId, CurrentUser.Id).ConfigureAwait(false);
            return Ok();
        }
    }
}
