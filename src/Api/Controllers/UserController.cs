namespace Lingtren.Api.Controllers
{
    using Application.Common.Models.ResponseModels;
    using FluentValidation;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using LinqKit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : BaseApiController
    {
        private readonly IValidator<UserRequestModel> _validator;
        private readonly IUserService _userService;

        public UserController(
                            IUserService userService,
                            IValidator<UserRequestModel> validator
                           )
        {
            _validator = validator;
            _userService = userService;
        }

        /// <summary>
        /// Search the users.
        /// </summary>
        /// <param name="searchCriteria">The user search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<SearchResult<UserResponseModel>> SearchAsync([FromQuery] UserSearchCriteria searchCriteria)
        {
            var searchResult = await _userService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<UserResponseModel>
            {
                Items = new List<UserResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new UserResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// user create api
        /// </summary>
        /// <param name="model"> the instance of <see cref="UserRequestModel" /> .</param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        [HttpPost]
        public async Task<UserResponseModel> CreateUser(UserRequestModel model)
        {
            var currentTimeStamp = DateTime.UtcNow;
            await this._validator.ValidateAsync(model, options => options.IncludeRuleSets("Add").ThrowOnFailures()).ConfigureAwait(false);

            var entity = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Address = model.Address,
                Email = model.Email,
                MobileNumber = model.MobileNumber,
                Bio = model.Bio,
                PublicUrls = model.PublicUrls,
                IsActive = true,
                Profession = model.Profession,
                Role = model.Role,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
            };

            var response = await _userService.CreateAsync(entity).ConfigureAwait(false);
            return new UserResponseModel(response);
        }

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="id"> the user id </param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<UserResponseModel> Get(Guid id)
        {
            User model = await _userService.GetAsync(id).ConfigureAwait(false);
            return new UserResponseModel(model);
        }

        /// <summary>
        /// update user
        /// </summary>
        /// <param name="userId"> the user id</param>
        /// <param name="model"> the  instance of <see cref="UserRequestModel" /> .</param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        [HttpPut("{userId}")]
        public async Task<UserResponseModel> UpdateUser(Guid userId, UserRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.IncludeRuleSets("Update").ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _userService.GetAsync(userId, CurrentUser.Id.ToString()).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.FirstName = model.FirstName;
            existing.MiddleName = model.MiddleName;
            existing.LastName = model.LastName;
            existing.Address = model.Address;
            existing.Email = model.Email;
            existing.MobileNumber = model.MobileNumber;
            existing.Bio = model.Bio;
            existing.PublicUrls = model.PublicUrls;
            existing.Profession = model.Profession;
            existing.Role = model.Role;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _userService.UpdateAsync(existing).ConfigureAwait(false);
            return new UserResponseModel(savedEntity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="enabled">the boolean</param>
        /// <returns>the instance of <see cref="UserResponseModel"/></returns>
        [HttpPatch("{userId}/status")]
        public async Task<UserResponseModel> ChangeStatus(Guid userId, [FromQuery] bool enabled)
        {
            var existing = await _userService.GetAsync(userId, CurrentUser.Id.ToString()).ConfigureAwait(false);

            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.IsActive = enabled;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _userService.UpdateAsync(existing).ConfigureAwait(false);
            return new UserResponseModel(savedEntity);
        }
    }
}
