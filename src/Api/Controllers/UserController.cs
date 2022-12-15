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
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : BaseApiController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IValidator<UserRequestModel> _validator;
        private readonly IValidator<ChangeEmailRequestModel> _changeEmailValidator;

        public UserController(
                            ILogger<UserController> logger,
                            IUserService userService,
                            IEmailService emailService,
                            IValidator<UserRequestModel> validator,
                            IValidator<ChangeEmailRequestModel> changeEmailValidator
                           )
        {
            _logger = logger;
            _userService = userService;
            _emailService = emailService;
            _validator = validator;
            _changeEmailValidator = changeEmailValidator;
        }

        /// <summary>
        /// Search the users.
        /// </summary>
        /// <param name="searchCriteria">The user search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        public async Task<SearchResult<UserResponseModel>> SearchAsync([FromQuery] UserSearchCriteria searchCriteria)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

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
            IsSuperAdminOrAdmin(CurrentUser.Role);

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
                ImageUrl = model.ImageUrl,
                PublicUrls = model.PublicUrls,
                IsActive = model.IsActive,
                Profession = model.Profession,
                Role = model.Role,
                DepartmentId = model.DepartmentId,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
            };

            var password = await _userService.GenerateRandomPassword(8).ConfigureAwait(false);
            entity.HashPassword = _userService.HashPassword(password);

            var response = await _userService.CreateAsync(entity).ConfigureAwait(false);
            await _emailService.SendUserCreatedPasswordEmail(entity.Email, entity.FullName, password).ConfigureAwait(false);
            return new UserResponseModel(response);
        }

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        [HttpGet("{userId}")]
        public async Task<UserResponseModel> Get(Guid userId)
        {
            User model = await _userService.GetAsync(userId).ConfigureAwait(false);
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
            if (CurrentUser.Id != userId && CurrentUser.Role != UserRole.SuperAdmin && CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} and role :{role} is not allowed to update user", CurrentUser.Id, CurrentUser.Role.ToString());
                throw new ForbiddenException("Only same user is allowed to update user or by admin only");
            }
            await _validator.ValidateAsync(model, options => options.IncludeRuleSets("Update").ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _userService.GetAsync(userId, CurrentUser.Id, includeAllProperties: false).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.FirstName = model.FirstName;
            existing.MiddleName = model.MiddleName;
            existing.LastName = model.LastName;
            existing.Address = model.Address;
            existing.MobileNumber = model.MobileNumber;
            existing.Bio = model.Bio;
            existing.PublicUrls = model.PublicUrls;
            existing.ImageUrl = model.ImageUrl;
            existing.Profession = model.Profession;
            existing.Role = model.Role;
            existing.IsActive = model.IsActive;
            existing.DepartmentId = model.DepartmentId;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _userService.UpdateAsync(existing).ConfigureAwait(false);
            return new UserResponseModel(savedEntity);
        }

        /// <summary>
        /// change email request api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("changeEmailRequest")]
        public async Task<IActionResult> ChangeEmailRequestAsync(ChangeEmailRequestModel model)
        {
            await _changeEmailValidator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            await _userService.ChangeEmailRequestAsync(model).ConfigureAwait(false);
            return Ok(new CommonResponseModel { Success = true, Message = "Email changed requested successfully." });
        }

        /// <summary>
        /// verify change email api
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("verifyChangeEmail")]
        public async Task<IActionResult> VerifyChangeEmailAsync([FromQuery] string token)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(token, nameof(token));
            await _userService.VerifyChangeEmailAsync(token).ConfigureAwait(false);
            return Ok(new CommonResponseModel { Success = true, Message = "Email changed successfully." });
        }
    }
}
