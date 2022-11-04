namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using LinqKit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class GroupController : BaseApiController
    {
        private readonly IGroupService _groupService;
        private readonly IValidator<GroupRequestModel> _validator;
        public GroupController(IGroupService groupService,
            IValidator<GroupRequestModel> validator)
        {
            _groupService = groupService;
            _validator = validator;
        }

        /// <summary>
        /// Search the groups.
        /// </summary>
        /// <param name="searchCriteria">The group search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<SearchResult<GroupResponseModel>> SearchAsync([FromQuery] BaseSearchCriteria searchCriteria)
        {
            var searchResult = await _groupService.SearchAsync(searchCriteria, includeAllProperties: false).ConfigureAwait(false);

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
            await this._validator.ValidateAsync(model, options => options.IncludeRuleSets("Add").ThrowOnFailures()).ConfigureAwait(false);

            var entity = new Group()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                IsActive = true,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
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
        [AllowAnonymous]
        public async Task<GroupResponseModel> Get(string identity)
        {
            Group model = await _groupService.GetByIdOrSlugAsync(identity, includeProperties: false).ConfigureAwait(false);
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
            var existing = await _groupService.GetByIdOrSlugAsync(identity, CurrentUser.Id.ToString(), includeProperties: false).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.IsActive = model.IsActive;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _groupService.UpdateAsync(existing).ConfigureAwait(false);
            return new GroupResponseModel(savedEntity);
        }

        //[HttpPost("{identity}/addMember")]
        //public async Task<GroupResponseModel> AddMember(string identity, AddGroupMemberRequestModel model)
        //{

        //}
    }
}
