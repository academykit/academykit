// <copyright file="UserController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using System.Globalization;
    using CsvHelper;
    using FluentValidation;
    using Hangfire;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class UserController : BaseApiController
    {
        private readonly ILogger<UserController> logger;
        private readonly IFileServerService fileServerService;
        private readonly IUserService userService;
        private readonly IGeneralSettingService generalSettingService;
        private readonly IValidator<UserRequestModel> validator;
        private readonly IValidator<ChangeEmailRequestModel> changeEmailValidator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public UserController(
            ILogger<UserController> logger,
            IFileServerService fileServerService,
            IUserService userService,
            IValidator<UserRequestModel> validator,
            IGeneralSettingService generalSettingService,
            IValidator<ChangeEmailRequestModel> changeEmailValidator,
            IStringLocalizer<ExceptionLocalizer> localizer)
        {
            this.fileServerService = fileServerService;
            this.logger = logger;
            this.userService = userService;
            this.validator = validator;
            this.changeEmailValidator = changeEmailValidator;
            this.generalSettingService = generalSettingService;
            this.localizer = localizer;
        }

        /// <summary>
        /// Search the users.
        /// </summary>
        /// <param name="searchCriteria">The user search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        public async Task<SearchResult<UserResponseModel>> SearchAsync(
            [FromQuery] UserSearchCriteria searchCriteria)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            var searchResult = await userService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<UserResponseModel>
            {
                Items = new List<UserResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p => response.Items.Add(new UserResponseModel(p)));

            return response;
        }

        /// <summary>
        /// user create api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="UserRequestModel" /> .</param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        [HttpPost]
        public async Task<UserResponseModel> CreateUser(UserRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            if (
                (model.Role == UserRole.Admin || model.Role == UserRole.SuperAdmin)
                && CurrentUser.Role != UserRole.SuperAdmin)
            {
                logger.LogWarning(
                    "{CurrentUser.Role} cannot create user with role {model.Role}.",
                    CurrentUser.Role,
                    model.Role);
                throw new ForbiddenException(
                    $"{CurrentUser.Role} cannot create user with role {model.Role}.");
            }

            var currentTimeStamp = DateTime.UtcNow;
            await validator
                .ValidateAsync(model, options => options.IncludeRuleSets("Add").ThrowOnFailures())
                .ConfigureAwait(false);

            var entity = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Address = model.Address,
                Email = model.Email.Trim(),
                MobileNumber = model.MobileNumber,
                Bio = model.Bio,
                ImageUrl = model.ImageUrl,
                PublicUrls = model.PublicUrls,
                Status = UserStatus.Pending,
                Profession = model.Profession,
                Role = model.Role,
                DepartmentId = model.DepartmentId,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                MemberId = model.MemberId,
            };

            var password = await userService.GenerateRandomPassword(8).ConfigureAwait(false);
            entity.HashPassword = userService.HashPassword(password);

            var response = await userService.CreateAsync(entity).ConfigureAwait(false);
            var company = await generalSettingService
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);
            BackgroundJob.Enqueue<IHangfireJobService>(
                job =>
                    job.SendUserCreatedPasswordEmail(
                        entity.Email,
                        entity.FirstName,
                        password,
                        company.CompanyName,
                        company.CompanyContactNumber,
                        null));
            return new UserResponseModel(response);
        }

        /// <summary>
        /// get user by id.
        /// </summary>
        /// <param name="userId"> the user id. </param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        [HttpGet("{userId}")]
        public async Task<UserResponseModel> Get(Guid userId)
        {
            return await userService.GetDetailAsync(userId).ConfigureAwait(false);
        }

        /// <summary>
        /// get user by id.
        /// </summary>
        /// <param name="userId"> the user id. </param>
        /// <param name="CourseID">the current course id. </param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        [HttpGet("{userId}/{courseId}")]
        public async Task<List<UserResponseModel>> GetUsersForCourseEnrollment(
            Guid userId,
            string courseId)
        {
            return await userService
                .GetUserForCourseEnrollment(userId, courseId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// get trainer.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>List of trainer.</returns>
        [HttpGet("trainer")]
        public async Task<IList<TrainerResponseModel>> Trainer(
            [FromQuery] TeacherSearchCriteria criteria) => await userService.GetTrainerAsync(CurrentUser.Id, criteria).ConfigureAwait(false);

        /// <summary>
        /// import bulk user api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="UserImportRequestModel" /> . </param>
        /// <returns> the task complete. </returns>
        [HttpPost("bulkUser")]
        public async Task<IActionResult> BulkUser([FromForm] UserImportRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            var response = await userService
                .ImportUserAsync(model.File, CurrentUser.Id)
                .ConfigureAwait(false);

            return Ok(response);
        }

        /// <summary>
        /// update user.
        /// </summary>
        /// <param name="userId"> the user id.</param>
        /// <param name="model"> the  instance of <see cref="UserRequestModel" /> .</param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        [HttpPut("{userId}")]
        public async Task<UserResponseModel> UpdateUser(Guid userId, UserUpdateRequestModel model)
        {
            if (
                CurrentUser.Id != userId
                && CurrentUser.Role != UserRole.SuperAdmin
                && CurrentUser.Role != UserRole.Admin)
            {
                logger.LogWarning(
                    "User with Id : {userId} and role :{role} is not allowed to update user.",
                    CurrentUser.Id,
                    CurrentUser.Role.ToString());
                throw new ForbiddenException(localizer.GetString("OnlySameUserOrAdmin"));
            }

            await validator
                .ValidateAsync(
                    model,
                    options => options.IncludeRuleSets("Update").ThrowOnFailures())
                .ConfigureAwait(false);
            var existing = await userService
                .GetAsync(userId, CurrentUser.Id, includeAllProperties: false)
                .ConfigureAwait(false);

            var currentTimeStamp = DateTime.UtcNow;
            var oldEmail = string.Empty;
            var isEmailChanged = false;
            var oldRole = existing.Role;

            if (model.Email.ToLower().Trim() != existing.Email.ToLower().Trim())
            {
                isEmailChanged = true;
                oldEmail = existing.Email;
            }

            var imageKey = existing.ImageUrl;
            existing.Id = existing.Id;
            #region Basic
            existing.FirstName = model.FirstName;
            existing.MiddleName = model.MiddleName;
            existing.LastName = model.LastName;
            existing.Gender = model.Gender;
            existing.BirthDateAD = model.BirthDateAD;
            existing.BirthDateBS = model.BirthDateBS;
            existing.ImageUrl = model.ImageUrl;
            existing.BloodGroup = model.BloodGroup;
            existing.Nationality = model.Nationality;
            existing.MaritalStatus = model.MaritalStatus;
            #endregion
            #region Official Info
            existing.MemberId = model.MemberId;
            existing.DepartmentId = model.DepartmentId;
            existing.Profession = model.Profession;
            existing.JoinedDateBS = model.JoinedDateBS;
            existing.JoinedDateAD = model.JoinedDateAD;
            existing.EmploymentType = model.EmploymentType;
            existing.BranchId = model.BranchId;
            #endregion
            #region Address
            #region Permanent
            existing.PermanentCountry = model.PermanentCountry;
            existing.PermanentState = model.PermanentState;
            existing.PermanentDistrict = model.PermanentDistrict;
            existing.PermanentCity = model.PermanentCity;
            existing.PermanentMunicipality = model.PermanentMunicipality;
            existing.PermanentWard = model.PermanentWard;
            existing.Address = model.Address; // permanent address of the Employee
            #endregion
            #region Current
            existing.AddressIsSame = model.AddressIsSame;
            existing.CurrentCountry = model.CurrentCountry;
            existing.CurrentState = model.CurrentState;
            existing.CurrentDistrict = model.CurrentDistrict;
            existing.CurrentCity = model.CurrentCity;
            existing.CurrentMunicipality = model.CurrentMunicipality;
            existing.CurrentWard = model.CurrentWard;
            existing.CurrentAddress = model.CurrentAddress;
            #endregion
            #endregion
            #region Contact Details
            existing.MobileNumber = model.MobileNumber; // primary mobile number
            existing.Email = model.Email; // official email
            existing.PersonalEmail = model.PersonalEmail;
            existing.MobileNumberSecondary = model.MobileNumberSecondary;
            #endregion
            #region Identification Details
            existing.IdentityType = model.IdentityType;
            existing.IdentityNumber = model.IdentityNumber;
            existing.IdentityIssuedOn = model.IdentityIssuedOn;
            existing.IdentityIssuedBy = model.IdentityIssuedBy;
            #endregion
            #region Family Details
            existing.FatherName = model.FatherName;
            existing.MotherName = model.MotherName;
            existing.SpouseName = model.SpouseName;
            existing.GrandFatherName = model.GrandFatherName;
            existing.MemberPhone = model.MemberPhone;
            existing.FamilyAddressIsSame = model.FamilyAddressIsSame;
            existing.MemberPermanentAddress = model.MemberPermanentAddress;
            existing.MemberCurrentAddress = model.MemberCurrentAddress;
            #endregion
            existing.Bio = model.Bio;
            existing.PublicUrls = model.PublicUrls;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            if (CurrentUser.Role == UserRole.SuperAdmin || CurrentUser.Role == UserRole.Admin)
            {
                if (
                    (model.Role == UserRole.Admin || model.Role == UserRole.SuperAdmin)
                    && existing.Id != CurrentUser.Id)
                {
                    IsSuperAdmin(CurrentUser.Role);
                }

                existing.Role = model.Role;
            }

            if (model.Status == UserStatus.Active || model.Status == UserStatus.InActive)
            {
                existing.Status = model.Status;
            }

            string password = null;
            if (isEmailChanged == true)
            {
                password = await userService.GenerateRandomPassword(8).ConfigureAwait(false);
                existing.HashPassword = userService.HashPassword(password);
            }

            if (oldRole != model.Role)
            {
                await userService.RemoveRefreshTokenAsync(existing.Id);
            }

            var savedEntity = await userService.UpdateAsync(existing).ConfigureAwait(false);
            if (imageKey != model.ImageUrl && !string.IsNullOrWhiteSpace(imageKey))
            {
                if (
                    imageKey.ToLower().Trim().Contains("/public/")
                    && imageKey.IndexOf("/standalone/") != -1)
                {
                    imageKey = imageKey.Substring(
                        imageKey.IndexOf("/standalone/") + "/standalone/".Length);
                }

                await fileServerService.RemoveFileAsync(imageKey).ConfigureAwait(false);
            }

            var company = await generalSettingService
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (password != null)
            {
                BackgroundJob.Enqueue<IHangfireJobService>(
                    job =>
                        job.AccountUpdatedMailAsync(existing.FullName, model.Email, oldEmail, null));
                BackgroundJob.Enqueue<IHangfireJobService>(
                    job =>
                        job.SendUserCreatedPasswordEmail(
                            existing.Email,
                            existing.FullName,
                            password,
                            company.CompanyName,
                            company.CompanyContactNumber,
                            null));
            }

            return new UserResponseModel(savedEntity);
        }

        /// <summary>
        /// change email request api.
        /// </summary>
        /// <param name="model"></param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPut("changeEmailRequest")]
        public async Task<ChangeEmailResponseModel> ChangeEmailRequestAsync(
            ChangeEmailRequestModel model)
        {
            await changeEmailValidator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            return await userService
                .ChangeEmailRequestAsync(model, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// resend email api.
        /// </summary>
        /// <param name="userId"> the user id. </param>
        /// <returns> the task complete. </returns>
        [HttpPatch("{userId}/resendEmail")]
        public async Task<IActionResult> ResendMail(Guid userId)
        {
            await userService.ResendEmailAsync(userId, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel { Success = true, Message = "Email resend successfully." });
        }

        /// <summary>
        /// change email request api.
        /// </summary>
        /// <param name="model"></param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPut("resendChangeEmailRequest")]
        public async Task<ChangeEmailResponseModel> ResendChangeEmailRequestAsync(
            ResendChangeEmailRequestModel model)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(model.Token, nameof(model.Token));
            return await userService
                .ResendChangeEmailRequestAsync(model.Token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// verify change email api.
        /// </summary>
        /// <param name="token"></param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [AllowAnonymous]
        [HttpGet("verifyChangeEmail")]
        public async Task<IActionResult> VerifyChangeEmailAsync([FromQuery] string token)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(token, nameof(token));
            await userService.VerifyChangeEmailAsync(token).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel
                {
                    Success = true,
                    Message = localizer.GetString("EmailChanged"),
                });
        }

        /// <summary>
        /// download bulk sample file.
        /// </summary>
        /// <returns> the csv file. </returns>
        [HttpGet("sampleFile")]
        public IActionResult SampleFile()
        {
            var data = new List<UserImportDto>();
            var mobileNumber = "+9779801230314";
            data.Add(
                new UserImportDto
                {
                    FirstName = "Adam",
                    MiddleName = string.Empty,
                    LastName = "Max",
                    Email = "bijay@vurilo.com",
                    MobileNumber = mobileNumber,
                    Role = "Trainer",
                    Designation = "Programmer",
                });
            using var memoryStream = new MemoryStream();
            using var steamWriter = new StreamWriter(memoryStream);
            using (var csv = new CsvWriter(steamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data);
                csv.Flush();
            }

            return File(memoryStream.ToArray(), "text/csv", "bulkImportFormat.csv");
        }
    }
}
