using System.Globalization;
using AcademyKit.Api.Common;
using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Helpers;
using AcademyKit.Infrastructure.Localization;
using CsvHelper;
using FluentValidation;
using Hangfire;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AcademyKit.Api.Controllers;

public class UserController : BaseApiController
{
    private readonly ILogger<UserController> logger;
    private readonly IFileServerService fileServerService;
    private readonly IUserService userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILicenseService _licenseService;
    private readonly IGeneralSettingService generalSettingService;
    private readonly IDepartmentService departmentService;
    private readonly IValidator<UserRequestModel> validator;
    private readonly IValidator<ChangeEmailRequestModel> changeEmailValidator;
    private readonly IStringLocalizer<ExceptionLocalizer> localizer;
    private readonly IPasswordHasher _passwordHasher;

    public UserController(
        ILogger<UserController> logger,
        IFileServerService fileServerService,
        IUserService userService,
        IUnitOfWork unitOfWork,
        ILicenseService licenseService,
        IValidator<UserRequestModel> validator,
        IGeneralSettingService generalSettingService,
        IValidator<ChangeEmailRequestModel> changeEmailValidator,
        IStringLocalizer<ExceptionLocalizer> localizer,
        IDepartmentService departmentService,
        IPasswordHasher passwordHasher
    )
    {
        this.fileServerService = fileServerService;
        this.logger = logger;
        this.userService = userService;
        this.validator = validator;
        this.changeEmailValidator = changeEmailValidator;
        this.generalSettingService = generalSettingService;
        this.localizer = localizer;
        this.departmentService = departmentService;
        _passwordHasher = passwordHasher;
        _licenseService = licenseService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Search the users.
    /// </summary>
    /// <param name="searchCriteria">The user search criteria.</param>
    /// <returns>The paginated search result.</returns>
    [HttpGet]
    public async Task<SearchResult<UserResponseModel>> SearchAsync(
        [FromQuery] UserSearchCriteria searchCriteria
    )
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
            && CurrentUser.Role != UserRole.SuperAdmin
        )
        {
            logger.LogWarning(
                "{CurrentUser.Role} cannot create user with role {model.Role}.",
                CurrentUser.Role,
                model.Role.ToString().SanitizeForLogger()
            );
            throw new ForbiddenException(
                $"{CurrentUser.Role} cannot create user with role {model.Role}."
            );
        }

        var currentTimeStamp = DateTime.UtcNow;
        await validator
            .ValidateAsync(model, options => options.ThrowOnFailures())
            .ConfigureAwait(false);

        var userCount = await _unitOfWork.GetRepository<User>().CountAsync().ConfigureAwait(false);
        var licenseData = await _licenseService
            .ValidateLicenseAsync(userCount + 1)
            .ConfigureAwait(false);
        if (!licenseData.Valid)
        {
            logger.LogWarning("License is not valid. User count: {userCount}.", userCount);
            throw new ForbiddenException(
                licenseData.Error
                    ?? "You have reached the maximum number of users. Please contact support."
            );
        }

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
        entity.HashPassword = _passwordHasher.HashPassword(password);

        var response = await userService.CreateAsync(entity).ConfigureAwait(false);
        var company = await generalSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
        BackgroundJob.Enqueue<IHangfireJobService>(job =>
            job.SendUserCreatedPasswordEmail(
                entity.Email,
                entity.FirstName,
                password,
                company.CompanyName,
                company.CompanyContactNumber,
                null
            )
        );
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
        string courseId
    )
    {
        return await userService.GetUserForCourseEnrollment(userId, courseId).ConfigureAwait(false);
    }

    /// <summary>
    /// get trainer.
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns>List of trainer.</returns>
    [HttpGet("trainer")]
    public async Task<IList<TrainerResponseModel>> Trainer(
        [FromQuery] TeacherSearchCriteria criteria
    ) => await userService.GetTrainerAsync(CurrentUser.Id, criteria).ConfigureAwait(false);

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
            && CurrentUser.Role != UserRole.Admin
        )
        {
            logger.LogWarning(
                "User with Id : {userId} and role :{role} is not allowed to update user.",
                CurrentUser.Id,
                CurrentUser.Role.ToString()
            );
            throw new ForbiddenException(localizer.GetString("OnlySameUserOrAdmin"));
        }

        await validator
            .ValidateAsync(model, options => options.ThrowOnFailures())
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

        if (model.DepartmentId != null)
        {
            var existingDepartment = await departmentService
                .GetByIdOrSlugAsync(model.DepartmentId.ToString(), CurrentUser.Id, false)
                .ConfigureAwait(false);

            existing.Department = existingDepartment;
        }

        var imageKey = existing.ImageUrl;
        existing.Id = existing.Id;
        #region Basic
        existing.FirstName = model.FirstName;
        existing.MiddleName = model.MiddleName;
        existing.LastName = model.LastName;
        existing.ImageUrl = model.ImageUrl;
        #endregion
        #region Official Info
        existing.MemberId = model.MemberId;
        existing.DepartmentId = model.DepartmentId;
        existing.Profession = model.Profession;
        #endregion
        #region Address
        #region Permanent
        existing.Address = model.Address; // permanent address of the Employee
        #endregion
        #endregion
        #region Contact Details
        existing.MobileNumber = model.MobileNumber; // primary mobile number
        existing.Email = model.Email; // official email
        #endregion
        existing.Bio = model.Bio;
        existing.PublicUrls = model.PublicUrls;
        existing.UpdatedBy = CurrentUser.Id;
        existing.UpdatedOn = currentTimeStamp;
        if (
            oldRole == UserRole.Admin
            && (model.Role == UserRole.Trainee || model.Role == UserRole.Trainer)
        )
        {
            await userService.AddUserToDefaultGroup(userId, CurrentUser.Id);
        }
        else if (
            (oldRole == UserRole.Trainee || oldRole == UserRole.Trainer)
            && model.Role == UserRole.Admin
        )
        {
            await userService.RemoveFromDefaultGroup(userId, CurrentUser.Id);
        }

        if (CurrentUser.Role == UserRole.SuperAdmin || CurrentUser.Role == UserRole.Admin)
        {
            if (
                (model.Role == UserRole.Admin || model.Role == UserRole.SuperAdmin)
                && existing.Id != CurrentUser.Id
            )
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
            existing.HashPassword = _passwordHasher.HashPassword(password);
        }

        if (oldRole != model.Role)
        {
            await userService.RemoveRefreshTokenAsync(existing.Id);
        }

        var savedEntity = await userService.UpdateAsync(existing, false).ConfigureAwait(false);
        if (imageKey != model.ImageUrl && !string.IsNullOrWhiteSpace(imageKey))
        {
            if (
                imageKey.ToLower().Trim().Contains("/public/")
                && imageKey.IndexOf("/standalone/") != -1
            )
            {
                imageKey = imageKey.Substring(
                    imageKey.IndexOf("/standalone/") + "/standalone/".Length
                );
            }

            await fileServerService.RemoveFileAsync(imageKey).ConfigureAwait(false);
        }

        var company = await generalSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);

        if (model.Email != oldEmail)
        {
            BackgroundJob.Enqueue<IHangfireJobService>(job =>
                job.SendEmailChangedMailAsync(existing.FullName, model.Email, oldEmail, null)
            );
        }

        if (password != null)
        {
            BackgroundJob.Enqueue<IHangfireJobService>(job =>
                job.SendUserCreatedPasswordEmail(
                    existing.Email,
                    existing.FullName,
                    password,
                    company.CompanyName,
                    company.CompanyContactNumber,
                    null
                )
            );
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
        ChangeEmailRequestModel model
    )
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
            new CommonResponseModel { Success = true, Message = "Email resend successfully." }
        );
    }

    /// <summary>
    /// change email request api.
    /// </summary>
    /// <param name="model"></param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    [HttpPut("resendChangeEmailRequest")]
    public async Task<ChangeEmailResponseModel> ResendChangeEmailRequestAsync(
        ResendChangeEmailRequestModel model
    )
    {
        CommonHelper.ValidateArgumentNotNullOrEmpty(model.Token, nameof(model.Token));
        return await userService.ResendChangeEmailRequestAsync(model.Token).ConfigureAwait(false);
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
            }
        );
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
                Email = "hello@academykit.co",
                MobileNumber = mobileNumber,
                Role = "Trainer",
                Designation = "Programmer",
            }
        );
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
