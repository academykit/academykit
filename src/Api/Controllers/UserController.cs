﻿namespace Lingtren.Api.Controllers
{
    using Application.Common.Models.ResponseModels;
    using FluentValidation;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using LinqKit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : BaseApiController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IValidator<UserRequestModel> _validator;
        private readonly IUserService _userService;

        public UserController(
                            ILogger<UserController> logger,
                            IUserService userService,
                            IValidator<UserRequestModel> validator
                           )
        {
            _logger = logger;
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
            var searchResult = await _userService.SearchAsync(searchCriteria, includeAllProperties: false).ConfigureAwait(false);

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
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} and role :{role} is not allowed to create user", CurrentUser.Id, CurrentUser.Role.ToString());
                throw new ForbiddenException("Only user with admin role is allowed to create user");
            }

            var currentTimeStamp = DateTime.UtcNow;
            await _validator.ValidateAsync(model, options => options.IncludeRuleSets("Add").ThrowOnFailures()).ConfigureAwait(false);

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
            User model = await _userService.GetAsync(id, includeAllProperties: false).ConfigureAwait(false);
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
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} and role :{role} is not allowed to update user", CurrentUser.Id, CurrentUser.Role.ToString());
                throw new ForbiddenException("Only user with admin role is allowed to update user");
            }
            await _validator.ValidateAsync(model, options => options.IncludeRuleSets("Update").ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _userService.GetAsync(userId, CurrentUser.Id, includeAllProperties: false).ConfigureAwait(false);
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
        /// change user status api
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="enabled">the boolean</param>
        /// <returns>the instance of <see cref="UserResponseModel"/></returns>
        [HttpPatch("{userId}/status")]
        public async Task<UserResponseModel> ChangeStatus(Guid userId, [FromQuery] bool enabled)
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} and role :{role} is not allowed to change user status", CurrentUser.Id, CurrentUser.Role.ToString());
                throw new ForbiddenException("Only user with admin role is allowed to change user status");
            }
            var existing = await _userService.GetAsync(userId, CurrentUser.Id, includeAllProperties: false).ConfigureAwait(false);

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
