using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Configurations;
using AcademyKit.Infrastructure.Helpers;
using AcademyKit.Infrastructure.Localization;
using CsvHelper;
using Hangfire;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using GroupMember = AcademyKit.Domain.Entities.GroupMember;

namespace AcademyKit.Infrastructure.Services;

public class UserService : BaseGenericService<User, UserSearchCriteria>, IUserService
{
    private readonly IEmailService _emailService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JWT _jwt;
    private readonly string _changeEmailEncryptionKey;
    private readonly int _changeEmailTokenExpiry;
    private readonly string _resendChangeEmailEncryptionKey;
    private readonly int _resendChangeEmailTokenExpiry;
    private readonly IGeneralSettingService _generalSettingService;
    private readonly IDepartmentService departmentService;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger,
        IEmailService emailService,
        IRefreshTokenService refreshTokenService,
        IOptions<JWT> jwt,
        IConfiguration configuration,
        IStringLocalizer<ExceptionLocalizer> localizer,
        IGeneralSettingService generalSettingService,
        IDepartmentService departmentService,
        IPasswordHasher passwordHasher
    )
        : base(unitOfWork, logger, localizer)
    {
        _emailService = emailService;
        _refreshTokenService = refreshTokenService;
        _jwt = jwt.Value;
        _changeEmailEncryptionKey = configuration.GetSection("ChangeEmail:EncryptionKey").Value;
        _changeEmailTokenExpiry = int.Parse(
            configuration.GetSection("ChangeEmail:ExpireInMinutes").Value
        );
        _resendChangeEmailEncryptionKey = configuration
            .GetSection("ResendChangeEmail:EncryptionKey")
            .Value;
        _resendChangeEmailTokenExpiry = int.Parse(
            configuration.GetSection("ResendChangeEmail:ExpireInMinutes").Value
        );
        _generalSettingService = generalSettingService;
        this.departmentService = departmentService;
        _passwordHasher = passwordHasher;
    }

    #region Account Services

    /// <summary>
    /// Verifies user credentials and generates an authentication token.
    /// </summary>
    /// <param name="model">The login request model containing user credentials.</param>
    /// <returns>An authentication model containing the token and user information.</returns>
    public async Task<AuthenticationModel> VerifyUserAndGetToken(LoginRequestModel model)
    {
        return await AuthenticateUser(
            model.Email.Trim(),
            model.Password,
            async (user, currentTimeStamp) =>
            {
                if (user.Status == UserStatus.Pending)
                {
                    user.Status = UserStatus.Active;
                    user.UpdatedBy = user.Id;
                    user.UpdatedOn = currentTimeStamp;
                    if (user.Role == UserRole.Trainee || user.Role == UserRole.Trainer)
                    {
                        await AddUserToDefaultGroup(user.Id, user.Id).ConfigureAwait(false);
                    }

                    _unitOfWork.GetRepository<User>().Update(user);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        );
    }

    /// <summary>
    /// Generates an authentication token using SSO (Single Sign-On) information.
    /// </summary>
    /// <param name="model">The OAuth user response model containing user information.</param>
    /// <returns>An authentication model containing the token and user information.</returns>
    /// <summary>
    public async Task<AuthenticationModel> GenerateTokenUsingSSOAsync(OAuthUserResponseModel model)
    {
        var currentTimeStamp = DateTime.UtcNow;
        var authenticationModel = new AuthenticationModel();

        var user = await _unitOfWork
            .GetRepository<User>()
            .GetFirstOrDefaultAsync(predicate: p => p.Email == model.Email)
            .ConfigureAwait(false);

        if (user == null)
        {
            var userId = Guid.NewGuid();
            user = new User
            {
                Id = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                MobileNumber = model.MobilePhone,
                ImageUrl = model.ProfilePictureUrl,
                Status = UserStatus.Active,
                Role = UserRole.Trainee,
                CreatedBy = userId,
                CreatedOn = currentTimeStamp,
                UpdatedBy = userId,
                UpdatedOn = currentTimeStamp
            };
            await CreateAsync(user).ConfigureAwait(false);
            await AddUserToDefaultGroup(userId, userId).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        if (user.Status == UserStatus.InActive)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = _localizer.GetString("AccountNotActive");
            return authenticationModel;
        }

        return await GenerateAccessAndRefreshToken(authenticationModel, user, currentTimeStamp)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Logs out a user by invalidating their refresh token.
    /// </summary>
    /// <param name="token">The refresh token to invalidate.</param>
    /// <param name="currentUserId">The ID of the current user.</param>
    /// <returns>A boolean indicating whether the logout was successful.</returns>
    public async Task<bool> Logout(string token, Guid currentUserId)
    {
        var userRefreshToken = await GetUserRefreshToken(token).ConfigureAwait(false);
        if (userRefreshToken == null || userRefreshToken.UserId != currentUserId)
        {
            return false;
        }

        await _refreshTokenService.DeleteAsync(userRefreshToken).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Refreshes an authentication token using a refresh token.
    /// </summary>
    /// <param name="token">The refresh token to use for generating a new authentication token.</param>
    /// <returns>An authentication model containing the new token and user information.</returns>
    public async Task<AuthenticationModel> RefreshTokenAsync(string token)
    {
        var authenticationModel = new AuthenticationModel();
        var refreshToken = await GetUserRefreshToken(token);
        if (refreshToken == null || !refreshToken.IsActive)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = "REFRESH_TOKEN_INVALID";
            return authenticationModel;
        }

        var user = await GetAsync(refreshToken.UserId, includeProperties: false);
        if (user == null)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = "REFRESH_TOKEN_INVALID";
            return authenticationModel;
        }

        await _refreshTokenService.DeleteAsync(refreshToken);

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = await GetUniqueRefreshToken().ConfigureAwait(false),
            LoginAt = DateTime.UtcNow,
            IsActive = true,
            UserId = user.Id,
        };

        await _refreshTokenService.CreateAsync(newRefreshToken);

        return await GenerateAccessAndRefreshToken(authenticationModel, user, DateTime.UtcNow)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the active refresh token for a user.
    /// </summary>
    /// <param name="user">The user to retrieve the refresh token for.</param>
    /// <returns>The active refresh token for the user.</returns>
    public async Task<RefreshToken> GetActiveRefreshToken(User user)
    {
        var refreshTokens = await _refreshTokenService.GetByUserId(user.Id).ConfigureAwait(false);
        return refreshTokens.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The user with the specified email address.</returns>
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await GetUserByPredicateAsync(p => p.Email == email, "UserNotFound");
    }

    /// <summary>
    /// Retrieves a list of trainers based on search criteria.
    /// </summary>
    /// <param name="currentUserId">The ID of the current user.</param>
    /// <param name="criteria">The search criteria for trainers.</param>
    /// <returns>A list of trainer response models.</returns>
    public async Task<IList<TrainerResponseModel>> GetTrainerAsync(
        Guid currentUserId,
        TeacherSearchCriteria criteria
    )
    {
        return await ExecuteWithResultAsync(async () =>
        {
            var isValidUser = await IsSuperAdminOrAdminOrTrainer(currentUserId)
                .ConfigureAwait(false);
            if (!isValidUser)
            {
                throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
            }

            var predicate = PredicateBuilder.New<User>(true);
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x =>
                    x.FirstName.ToLower().Trim().Contains(search)
                    || x.LastName.ToLower().Trim().Contains(search)
                    || x.Email.ToLower().Trim().Contains(search)
                );
            }

            predicate = predicate.And(p => p.Role == UserRole.Admin || p.Role == UserRole.Trainer);

            if (
                !string.IsNullOrWhiteSpace(criteria.Identity)
                && criteria.LessonType == TrainingTypeEnum.Course
            )
            {
                var courseTeacher = await _unitOfWork
                    .GetRepository<CourseTeacher>()
                    .GetAllAsync(predicate: p =>
                        p.CourseId.ToString() == criteria.Identity
                        || p.Course.Slug.ToLower() == criteria.Identity.ToLower().Trim()
                    )
                    .ConfigureAwait(false);

                var userIds = courseTeacher.Select(x => x.UserId).ToList();
                predicate = predicate.And(p => !userIds.Contains(p.Id));
            }

            if (
                !string.IsNullOrWhiteSpace(criteria.Identity)
                && criteria.LessonType == TrainingTypeEnum.QuestionPool
            )
            {
                var questionPoolTeachers = await _unitOfWork
                    .GetRepository<QuestionPoolTeacher>()
                    .GetAllAsync(predicate: p =>
                        p.QuestionPoolId.ToString() == criteria.Identity
                        || p.QuestionPool.Slug.ToLower() == criteria.Identity.ToLower().Trim()
                    )
                    .ConfigureAwait(false);

                var userIds = questionPoolTeachers.Select(x => x.UserId).ToList();
                predicate = predicate.And(p => !userIds.Contains(p.Id));
            }

            return await _unitOfWork
                .GetRepository<User>()
                .GetAllAsync(predicate: predicate, selector: s => new TrainerResponseModel(s))
                .ConfigureAwait(false);
        });
    }

    /// <summary>
    /// Imports users from a CSV file.
    /// </summary>
    /// <param name="file">The CSV file containing user information.</param>
    /// <param name="currentUserId">The ID of the current user performing the import.</param>
    /// <returns>A string message indicating the result of the import operation.</returns>
    public async Task<string> ImportUserAsync(IFormFile file, Guid currentUserId)
    {
        try
        {
            MimeTypes.TryGetExtension(file.ContentType, out var extension);
            if (extension != ".csv")
            {
                throw new ArgumentException(_localizer.GetString("CSVFileExtension"));
            }

            var users = new List<UserImportDto>();
            (List<UserImportDto> userList, List<int> SN) checkForValidRows = (
                new List<UserImportDto>(),
                new List<int>()
            );
            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                while (csv.Read())
                {
                    var user = csv.GetRecord<UserImportDto>();
                    if (user == default)
                    {
                        continue;
                    }

                    if (user != null)
                    {
                        checkForValidRows.userList.Add(user);
                        checkForValidRows.SN.Add(checkForValidRows.SN.Count + 1);
                    }
                }
            }

            await CheckBulkImport(checkForValidRows, currentUserId);
            var message = new StringBuilder();
            users = checkForValidRows
                .userList.Where(x =>
                    !string.IsNullOrWhiteSpace(x.FirstName)
                    && !string.IsNullOrWhiteSpace(x.LastName)
                    && !string.IsNullOrWhiteSpace(x.Email)
                )
                .ToList();
            var company = await _unitOfWork
                .GetRepository<GeneralSetting>()
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);
            var stringBuilder = new StringBuilder();
            if (users.Count != default)
            {
                var userEmails = users.ConvertAll(x => x.Email);
                var duplicateUser = await _unitOfWork
                    .GetRepository<User>()
                    .GetAllAsync(
                        predicate: p => userEmails.Contains(p.Email),
                        selector: x => x.Email
                    )
                    .ConfigureAwait(false);
                if (duplicateUser.Count != default)
                {
                    message.AppendLine(
                        $"{_localizer.GetString("AlreadyRegistered")}"
                            + " "
                            + string.Join(",", duplicateUser)
                    );
                }

                var newUsersList = users.Where(x => !duplicateUser.Contains(x.Email)).ToList();
                newUsersList = newUsersList.DistinctBy(x => x.Email).ToList();
                var newUsers = new List<User>();
                var newUserEmails = new List<UserEmailDto>();
                if (newUsersList.Count != default)
                {
                    Guid? departmentId = null;
                    foreach (var user in newUsersList)
                    {
                        if (!string.IsNullOrEmpty(user.Department))
                        {
                            var existDepartment = await departmentService
                                .SearchAsync(
                                    new DepartmentBaseSearchCriteria
                                    {
                                        departmentName = user.Department
                                    },
                                    false
                                )
                                .ConfigureAwait(false);
                            if (existDepartment.Items.Count > 0)
                            {
                                departmentId = existDepartment.Items[0].Id;
                            }
                        }

                        var userEntity = new User()
                        {
                            Id = Guid.NewGuid(),
                            FirstName = user.FirstName,
                            MiddleName = user.MiddleName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Status = UserStatus.Pending,
                            Profession = user.Designation,
                            MobileNumber = user.MobileNumber,
                            Role = (UserRole)Enum.Parse(typeof(UserRole), user.Role, true),
                            CreatedBy = currentUserId,
                            CreatedOn = DateTime.UtcNow,
                            DepartmentId = departmentId,
                        };
                        var password = await GenerateRandomPassword(8).ConfigureAwait(false);
                        userEntity.HashPassword = _passwordHasher.HashPassword(password);

                        var userEmailDto = new UserEmailDto
                        {
                            FullName = userEntity.FirstName,
                            Email = userEntity.Email,
                            Password = password,
                            CompanyName = company.CompanyName,
                            CompanyNumber = company.CompanyContactNumber
                        };
                        newUsers.Add(userEntity);
                        newUserEmails.Add(userEmailDto);
                    }

                    await _unitOfWork
                        .GetRepository<User>()
                        .InsertAsync(newUsers)
                        .ConfigureAwait(false);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    BackgroundJob.Enqueue<IHangfireJobService>(job =>
                        job.SendEmailImportedUserAsync(newUserEmails, null)
                    );
                    message.AppendLine(
                        $"{newUsers.Count}" + " " + _localizer.GetString("UserImported")
                    );
                }
            }
            else
            {
                message.AppendLine(_localizer.GetString("EmptyFile"));
            }

            return message.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(_localizer.GetString("NoUserImported"));
            throw ex is ServiceException
                ? ex
                : new ServiceException(_localizer.GetString("NoUserImported"));
        }
    }

    /// <summary>
    /// Resets a user's password.
    /// </summary>
    /// <param name="user">The user whose password needs to be reset.</param>
    public async Task ResetPasswordAsync(User user)
    {
        await GenerateAndSendToken(
            async (token, tokenExpiry) =>
            {
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiry = tokenExpiry;
                _unitOfWork.GetRepository<User>().Update(user);
                var companyName = await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .GetFirstOrDefaultAsync(selector: x => x.CompanyName)
                    .ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                await _emailService
                    .SendForgetPasswordEmail(user.Email, user.FirstName, token, companyName)
                    .ConfigureAwait(false);
            }
        );
    }

    /// <summary>
    /// Verifies a password reset token.
    /// </summary>
    /// <param name="model">The model containing the email and token to verify.</param>
    /// <returns>A verification token response model.</returns>

    public async Task<VerificationTokenResponseModel> VerifyPasswordResetTokenAsync(
        VerifyResetTokenModel model
    )
    {
        return await VerifyToken(
            model.Email,
            async (user, currentTimeStamp) =>
            {
                if (currentTimeStamp > user.PasswordResetTokenExpiry)
                {
                    throw new ForbiddenException(_localizer.GetString("ResetTokenExpired"));
                }

                if (model.Token != user.PasswordResetToken)
                {
                    throw new ForbiddenException(_localizer.GetString("ResetTokenNotMatched"));
                }

                user.PasswordChangeToken = await BuildResetPasswordJWTToken(user.Email)
                    .ConfigureAwait(false);
                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return new VerificationTokenResponseModel
                {
                    Token = user.PasswordChangeToken,
                    Message = _localizer.GetString("TokenVerifiedSuccessfully")
                };
            }
        );
    }

    /// <summary>
    /// Generates a random password.
    /// </summary>
    /// <param name="length">The length of the password to generate.</param>
    /// <returns>A randomly generated password.</returns>

    public async Task<string> GenerateRandomPassword(int length) =>
        await PasswordGenerator.GenerateStrongPassword(length);

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    /// <param name="model">The model containing the current and new password.</param>
    /// <param name="currentUserId">The ID of the current user.</param>

    public async Task ChangePasswordAsync(ChangePasswordRequestModel model, Guid currentUserId)
    {
        var user = await GetAsync(currentUserId, includeProperties: false).ConfigureAwait(false);

        var currentPasswordMatched = _passwordHasher.VerifyPassword(
            user.HashPassword,
            model.CurrentPassword
        );

        if (!currentPasswordMatched)
        {
            throw new ForbiddenException(_localizer.GetString("CurrentPasswordNotMatched"));
        }

        user.HashPassword = _passwordHasher.HashPassword(model.NewPassword);
        user.UpdatedOn = DateTime.UtcNow;

        _unitOfWork.GetRepository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Initiates a change email request.
    /// </summary>
    /// <param name="model">The model containing the old and new email addresses.</param>
    /// <param name="currentUserId">The ID of the current user.</param>
    /// <returns>A change email response model.</returns>

    public async Task<ChangeEmailResponseModel> ChangeEmailRequestAsync(
        ChangeEmailRequestModel model,
        Guid currentUserId
    )
    {
        var user = await GetUserByEmailAsync(model.OldEmail).ConfigureAwait(false);
        if (user == null)
        {
            throw new ForbiddenException(
                _localizer.GetString("UserNotFoundWithEmail") + " " + model.OldEmail
            );
        }

        var newUser = await _unitOfWork
            .GetRepository<User>()
            .GetFirstOrDefaultAsync(predicate: p => p.Email == model.NewEmail)
            .ConfigureAwait(false);
        if (newUser != null)
        {
            throw new ForbiddenException(
                model.NewEmail + " " + _localizer.GetString("AlreadyExistInAnotherAccount")
            );
        }

        var isUserAuthenticated = _passwordHasher.VerifyPassword(user.HashPassword, model.Password);
        if (!isUserAuthenticated)
        {
            throw new ForbiddenException(_localizer.GetString("PasswordNotMatched"));
        }

        if (user.Id != currentUserId)
        {
            throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
        }

        var companyName = await _unitOfWork
            .GetRepository<GeneralSetting>()
            .GetFirstOrDefaultAsync(selector: x => x.CompanyName)
            .ConfigureAwait(false);
        var changeEmailToken = GenerateResendAndChangeEmailToken(
            model.OldEmail,
            model.NewEmail,
            _changeEmailEncryptionKey,
            _changeEmailTokenExpiry
        );
        var resendToken = GenerateResendAndChangeEmailToken(
            model.OldEmail,
            model.NewEmail,
            _resendChangeEmailEncryptionKey,
            _resendChangeEmailTokenExpiry
        );
        await _emailService
            .SendChangePasswordMailAsync(
                model.NewEmail,
                user.FirstName,
                changeEmailToken,
                _changeEmailTokenExpiry,
                companyName
            )
            .ConfigureAwait(false);

        return new ChangeEmailResponseModel() { ResendToken = resendToken };
    }

    /// <summary>
    /// Resend an email for a pending email change request.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the email resend.</param>
    /// <param name="currentUserId">The ID of the current user.</param>
    public async Task ResendEmailAsync(Guid userId, Guid currentUserId)
    {
        try
        {
            var isSuperAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
            if (!isSuperAdmin)
            {
                throw new ForbiddenException(_localizer.GetString("UnauthorizedToResendEmail"));
            }

            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == userId)
                .ConfigureAwait(false);
            if (user == default)
            {
                throw new ForbiddenException(_localizer.GetString("UserNotFound"));
            }

            if (user.Status != UserStatus.Pending)
            {
                throw new ArgumentException(_localizer.GetString("UserAlreadyActive"));
            }

            var password = await GenerateRandomPassword(8).ConfigureAwait(false);
            var hashPassword = _passwordHasher.HashPassword(password);
            user.HashPassword = hashPassword;
            user.UpdatedBy = currentUserId;
            user.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var company = await _generalSettingService
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);
            BackgroundJob.Enqueue<IHangfireJobService>(job =>
                job.SendUserCreatedPasswordEmail(
                    user.Email,
                    user.FullName,
                    password,
                    company.CompanyName,
                    company.CompanyContactNumber,
                    null
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred on ResendEmailAsync method : {ex.Message}");
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Resend a change email request.
    /// </summary>
    /// <param name="token">The token for the change email request.</param>
    /// <returns>A change email response model.</returns>

    public async Task<ChangeEmailResponseModel> ResendChangeEmailRequestAsync(string token)
    {
        return await ResendEmailRequestAsync(
            token,
            _resendChangeEmailEncryptionKey,
            _changeEmailEncryptionKey,
            _changeEmailTokenExpiry,
            _resendChangeEmailTokenExpiry,
            async (oldEmail, newEmail, changeEmailToken, resendToken, companyName) =>
            {
                await _emailService
                    .SendChangePasswordMailAsync(
                        newEmail,
                        oldEmail,
                        changeEmailToken,
                        _changeEmailTokenExpiry,
                        companyName
                    )
                    .ConfigureAwait(false);
                return new ChangeEmailResponseModel() { ResendToken = resendToken };
            }
        );
    }

    /// <summary>
    /// Verifies a change email request.
    /// </summary>
    /// <param name="token">The token for the change email request.</param>

    public async Task VerifyChangeEmailAsync(string token)
    {
        await VerifyEmailChangeAsync(
            token,
            _changeEmailEncryptionKey,
            async (oldEmail, newEmail, user, currentTimeStamp) =>
            {
                user.Email = newEmail;
                user.UpdatedOn = currentTimeStamp;

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync();
                BackgroundJob.Enqueue<IHangfireJobService>(job =>
                    job.SendEmailChangedMailAsync(newEmail, oldEmail, user.FirstName, null)
                );
            }
        );
    }

    /// <summary>
    /// Generates a token for resending and changing email.
    /// </summary>
    /// <param name="oldEmail">The old email address.</param>
    /// <param name="newEmail">The new email address.</param>
    /// <param name="encryptionKey">The encryption key.</param>
    /// <param name="tokenExpiry">The token expiry time in minutes.</param>
    /// <returns>The generated token.</returns>
    public static string GenerateResendAndChangeEmailToken(
        string oldEmail,
        string newEmail,
        string encryptionKey,
        int tokenExpiry
    )
    {
        var securityKey = Encoding.UTF8.GetBytes(encryptionKey);
        var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>()
            {
                { "oldEmail", oldEmail },
                { "newEmail", newEmail },
            },
            Expires = DateTime.UtcNow.AddMinutes(tokenExpiry),
            SigningCredentials = new SigningCredentials(
                symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }

    /// <summary>
    /// Removes all refresh tokens for a user.
    /// </summary>
    /// <param name="currentUserId">The ID of the current user.</param>

    public async Task RemoveRefreshTokenAsync(Guid currentUserId)
    {
        var userToken = await _unitOfWork
            .GetRepository<RefreshToken>()
            .GetAllAsync(predicate: p => p.UserId == currentUserId)
            .ConfigureAwait(false);
        userToken.ForEach(x => x.IsActive = false);
        _unitOfWork.GetRepository<RefreshToken>().Update(userToken);
    }

    /// <summary>
    /// Retrieves detailed information about a user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve details for.</param>
    /// <returns>A user response model containing detailed user information.</returns>
    public async Task<UserResponseModel> GetDetailAsync(Guid userId)
    {
        try
        {
            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.Id == userId,
                    include: src =>
                        src.Include(x => x.Department)
                            .Include(x => x.UserSkills)
                            .ThenInclude(x => x.Skills)
                )
                .ConfigureAwait(false);

            var userCertificates = await _unitOfWork
                .GetRepository<CourseEnrollment>()
                .GetAllAsync(
                    predicate: p =>
                        p.UserId == userId
                        && p.HasCertificateIssued.HasValue
                        && p.HasCertificateIssued.Value,
                    include: src => src.Include(x => x.Course)
                )
                .ConfigureAwait(false);
            var externalCertificates = await _unitOfWork
                .GetRepository<Certificate>()
                .GetAllAsync(predicate: p =>
                    p.CreatedBy == userId && p.Status == CertificateStatus.Approved
                )
                .ConfigureAwait(false);
            var response = new UserResponseModel(user);
            foreach (var item in userCertificates)
            {
                response.Certificates.Add(
                    new CourseCertificateIssuedResponseModel
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.Name,
                        CourseSlug = item.Course.Slug,
                        Percentage = item.Percentage,
                        HasCertificateIssued = item.HasCertificateIssued,
                        CertificateIssuedDate = item.CertificateIssuedDate,
                        CertificateUrl = item.CertificateUrl,
                    }
                );
            }

            foreach (var external in externalCertificates)
            {
                response.ExternalCertificates.Add(
                    new ExternalCertificateResponseModel
                    {
                        Name = external.Name,
                        StartDate = external.StartDate,
                        EndDate = external.EndDate,
                        ImageUrl = external.ImageUrl,
                        Location = external.Location,
                        Institute = external.Institute,
                        Duration = external.Duration != 0 ? external.Duration.ToString() : null
                    }
                );
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while attempting to fetch user detail information."
            );
            throw ex is ServiceException
                ? ex
                : new ServiceException(_localizer.GetString("ErrorOccurredFetchUserDetails"));
        }
    }

    /// <summary>
    /// Retrieves users eligible for course enrollment.
    /// </summary>
    /// <param name="userId">The ID of the current user.</param>
    /// <param name="courseId">The ID of the course.</param>
    /// <returns>A list of user response models.</returns>

    public async Task<List<UserResponseModel>> GetUserForCourseEnrollment(
        Guid userId,
        string courseId
    )
    {
        try
        {
            var course = await ValidateAndGetCourse(userId, courseId, validateForModify: false)
                .ConfigureAwait(false);
            if (course == null)
            {
                throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
            }

            var users = await _unitOfWork.GetRepository<User>().GetAllAsync().ConfigureAwait(false);
            var user = users.FirstOrDefault(x => x.Id == userId);
            if (user.Role == UserRole.Admin || user.Role == UserRole.SuperAdmin)
            {
                var trimmedUsers = users.Where(x =>
                    x.Id != userId
                    && x.CourseEnrollments != course.CourseEnrollments
                    && x.Role != UserRole.SuperAdmin
                    && x.Role != UserRole.Admin
                    && x.Id != course.CreatedBy
                    && x.CourseTeachers != course.CourseTeachers
                );

                var response = new List<UserResponseModel>();
                foreach (var trimmedUser in trimmedUsers)
                {
                    response.Add(
                        new UserResponseModel
                        {
                            Id = trimmedUser.Id,
                            FullName = trimmedUser.FullName,
                            Address = trimmedUser.Address,
                            Email = trimmedUser.Email,
                            FirstName = trimmedUser.FirstName,
                            LastName = trimmedUser.LastName,
                            MobileNumber = trimmedUser.MobileNumber,
                            Bio = trimmedUser.Bio,
                            Role = trimmedUser.Role,
                            DepartmentId = trimmedUser.DepartmentId,
                            Status = trimmedUser.Status,
                            PublicUrls = trimmedUser.PublicUrls,
                        }
                    );
                }

                return response;
            }
            else
            {
                throw new UnauthorizedAccessException(_localizer.GetString("UnauthorizedUser"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while attempting to fetch user detail information."
            );
            throw ex is ServiceException
                ? ex
                : new ServiceException(_localizer.GetString("ErrorOccurredFetchUserDetails"));
        }
    }

    /// <summary>
    /// Adds a user to the default group.
    /// </summary>
    /// <param name="userId">The ID of the user to add.</param>
    /// <param name="currentUserId">The ID of the current user performing the action.</param>

    public async Task AddUserToDefaultGroup(Guid userId, Guid currentUserId)
    {
        var defaultGroup = await _unitOfWork
            .GetRepository<Domain.Entities.Group>()
            .GetFirstOrDefaultAsync(predicate: x => x.IsDefault == true);

        if (defaultGroup is null)
        {
            throw new ForbiddenException(_localizer.GetString("DefaultGroupNotFound"));
        }

        var existUsers = await _unitOfWork
            .GetRepository<GroupMember>()
            .ExistsAsync(predicate: p => p.GroupId == defaultGroup.Id && p.UserId == userId)
            .ConfigureAwait(false);

        if (existUsers)
        {
            throw new ForbiddenException(_localizer.GetString("GroupContainsUsersCannotAdd"));
        }

        var groupMember = new GroupMember
        {
            UserId = userId,
            GroupId = defaultGroup.Id,
            IsActive = true,
            CreatedBy = currentUserId,
            CreatedOn = DateTime.Now,
        };
        await _unitOfWork
            .GetRepository<GroupMember>()
            .InsertAsync(groupMember)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a user from the default group.
    /// </summary>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <param name="CurrentUserId">The ID of the current user performing the action.</param>

    public async Task RemoveFromDefaultGroup(Guid userId, Guid CurrentUserId)
    {
        var existingMember = await _unitOfWork
            .GetRepository<GroupMember>()
            .GetFirstOrDefaultAsync(predicate: p => p.UserId == userId)
            .ConfigureAwait(false);

        if (existingMember != null)
        {
            _unitOfWork.GetRepository<GroupMember>().Delete(existingMember.Id);
        }

        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }

    #endregion Account Services

    #region Protected Methods

    /// <summary>
    /// Performs pre-creation checks for a user entity.
    /// </summary>
    /// <param name="entity">The user entity to be created.</param>
    protected override async Task CreatePreHookAsync(User entity)
    {
        await CheckDuplicateEmailAsync(entity).ConfigureAwait(false);
        if (entity.MemberId != default)
        {
            await CheckForDuplicateMemberId(entity).ConfigureAwait(false);
        }

        await Task.FromResult(0);
    }

    /// <summary>
    /// Constructs query conditions for user search.
    /// </summary>
    /// <param name="predicate">The initial predicate for the query.</param>
    /// <param name="criteria">The search criteria for the user search.</param>
    /// <returns>The constructed predicate expression.</returns>
    protected override Expression<Func<User, bool>> ConstructQueryConditions(
        Expression<Func<User, bool>> predicate,
        UserSearchCriteria criteria
    )
    {
        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var search = criteria.Search.ToLower().Trim();
            predicate = predicate.And(x =>
                ((x.FirstName.Trim() + " " + x.MiddleName.Trim()).Trim() + " " + x.LastName.Trim())
                    .Trim()
                    .Contains(search)
                || x.Email.ToLower().Trim().Contains(search)
                || x.MobileNumber.ToLower().Trim().Contains(search)
            );
        }

        if (criteria.Role.HasValue)
        {
            predicate = predicate.And(p => p.Role == criteria.Role.Value);
        }

        if (criteria.DepartmentId.HasValue)
        {
            predicate = predicate.And(p => p.DepartmentId == criteria.DepartmentId.Value);
        }

        if (criteria.Status.HasValue)
        {
            predicate = predicate.And(p => p.Status == criteria.Status.Value);
        }

        return predicate;
    }

    /// <summary>
    /// Deletes related entities when a user is deleted.
    /// </summary>
    /// <param name="user">The user entity to be deleted.</param>
    protected override void DeleteChildEntities(User user)
    {
        var courseTeacher = _unitOfWork
            .GetRepository<CourseTeacher>()
            .GetAll(predicate: p => p.UserId == user.Id)
            .ToList();
        var questionPoolTeacher = _unitOfWork
            .GetRepository<QuestionPoolTeacher>()
            .GetAll(predicate: p => p.UserId == user.Id)
            .ToList();
        if (courseTeacher.Count() != default && !string.IsNullOrEmpty(courseTeacher.ToString()))
        {
            foreach (var removeUser in courseTeacher)
            {
                _unitOfWork.GetRepository<CourseTeacher>().Delete(removeUser);
            }
        }

        if (
            questionPoolTeacher.Count() != default
            && string.IsNullOrEmpty(questionPoolTeacher.ToString())
        )
        {
            foreach (var removeUser in questionPoolTeacher)
            {
                _unitOfWork.GetRepository<QuestionPoolTeacher>().Delete(removeUser);
            }
        }
    }

    /// <summary>
    /// Resolves child entities after a user is updated.
    /// </summary>
    /// <param name="user">The user entity to be updated.</param>
    protected override async Task ResolveChildEntitiesAsync(User user)
    {
        var oldUser = await _unitOfWork
            .GetRepository<User>()
            .GetFirstOrDefaultAsync(predicate: p => p.Id == user.Id)
            .ConfigureAwait(false);
        if (user.MemberId != default)
        {
            await CheckForDuplicateMemberId(user).ConfigureAwait(false);
        }

        if (oldUser != null)
        {
            var allowed = userRecordModificationValidity(user, oldUser);
            if (allowed == true)
            {
                var superAdmin = await _unitOfWork
                    .GetRepository<User>()
                    .GetFirstOrDefaultAsync(predicate: p => p.Role == UserRole.SuperAdmin)
                    .ConfigureAwait(false);
                var courses = await _unitOfWork
                    .GetRepository<Course>()
                    .GetAllAsync(predicate: p => p.CreatedBy == oldUser.Id)
                    .ConfigureAwait(false);
                var questionPools = await _unitOfWork
                    .GetRepository<QuestionPool>()
                    .GetAllAsync(predicate: p => p.CreatedBy == oldUser.Id)
                    .ConfigureAwait(false);
                var updateCourse = new List<Course>();
                var updatePool = new List<QuestionPool>();
                var currentDateTime = DateTime.UtcNow;
                if (courses.Count != default)
                {
                    foreach (var course in courses)
                    {
                        course.CreatedBy = superAdmin.Id;
                        course.UpdatedBy = superAdmin.Id;
                        course.UpdatedOn = currentDateTime;
                        updateCourse.Add(course);
                    }

                    _unitOfWork.GetRepository<Course>().Update(updateCourse);
                }

                if (questionPools.Count != default)
                {
                    foreach (var questionPool in questionPools)
                    {
                        questionPool.UpdatedBy = superAdmin.Id;
                        questionPool.UpdatedOn = currentDateTime;
                        questionPool.CreatedBy = superAdmin.Id;
                        updatePool.Add(questionPool);
                    }

                    _unitOfWork.GetRepository<QuestionPool>().Update(updatePool);
                }
            }
        }
    }

    /// <summary>
    /// Sets the default sort option for user search criteria.
    /// </summary>
    /// <param name="criteria">The search criteria for the user search.</param>
    protected override void SetDefaultSortOption(UserSearchCriteria criteria)
    {
        criteria.SortBy = nameof(User.CreatedOn);
        criteria.SortType = SortType.Descending;
    }

    /// <summary>
    /// Includes navigation properties in the user query.
    /// </summary>
    /// <param name="query">The query to include navigation properties.</param>
    /// <returns>The query with included navigation properties.</returns>
    protected override IIncludableQueryable<User, object> IncludeNavigationProperties(
        IQueryable<User> query
    )
    {
        return query
            .Include(x => x.Department)
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skills);
    }
    #endregion Protected Methods

    #region Private Methods

    /// <summary>
    /// Checks the validity of user record modification.
    /// </summary>
    /// <param name="newUser">The new user entity.</param>
    /// <param name="oldUser">The old user entity.</param>
    /// <returns>True if the modification is valid, otherwise false.</returns>
    private static bool userRecordModificationValidity(User newUser, User oldUser)
    {
        if (newUser.Role == UserRole.Admin && oldUser.Role == UserRole.Trainer)
        {
            return false;
        }

        if (newUser.Role == UserRole.Trainer && oldUser.Role == UserRole.Admin)
        {
            return false;
        }

        if (newUser.Role == oldUser.Role)
        {
            return false;
        }

        if (newUser.Role == UserRole.Admin && oldUser.Role == UserRole.Trainee)
        {
            return false;
        }

        if (newUser.Role == UserRole.Trainer && oldUser.Role == UserRole.Trainee)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Authenticates a user based on email and password.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="additionalActions">Additional actions to be performed after authentication.</param>
    /// <returns>The authentication model with token information.</returns>
    private async Task<AuthenticationModel> AuthenticateUser(
        string email,
        string password,
        Func<User, DateTime, Task> additionalActions
    )
    {
        var authenticationModel = new AuthenticationModel();

        var user = await GetUserByEmailAsync(email);
        if (user == null)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = _localizer.GetString("AccountNotRegistered");
            return authenticationModel;
        }

        if (user.Status == UserStatus.InActive)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = _localizer.GetString("AccountNotActive");
            return authenticationModel;
        }

        var isUserAuthenticated = _passwordHasher.VerifyPassword(user.HashPassword, password);
        if (!isUserAuthenticated)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = _localizer.GetString("IncorrectCredentials");
            return authenticationModel;
        }

        var currentTimeStamp = DateTime.UtcNow;

        await additionalActions(user, currentTimeStamp);

        return await GenerateAccessAndRefreshToken(authenticationModel, user, currentTimeStamp)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Generates and sends a token to the user.
    /// </summary>
    /// <param name="sendTokenAction">The action to send the token.</param>
    private static async Task GenerateAndSendToken(Func<string, DateTime, Task> sendTokenAction)
    {
        var generator = new Random();
        var token = generator.Next(0, 1000000).ToString("D6");
        var tokenExpiry = DateTime.UtcNow.AddMinutes(5);
        await sendTokenAction(token, tokenExpiry);
    }

    /// <summary>
    /// Verifies the token for a user.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="verifyAction">The action to verify the token.</param>
    /// <returns>The verification token response model.</returns>
    private async Task<VerificationTokenResponseModel> VerifyToken(
        string email,
        Func<User, DateTime, Task<VerificationTokenResponseModel>> verifyAction
    )
    {
        var currentTimeStamp = DateTime.UtcNow;
        var user = await GetUserByEmailAsync(email).ConfigureAwait(false);
        if (user == null)
        {
            throw new EntityNotFoundException(_localizer.GetString("UserNotFound"));
        }

        return await verifyAction(user, currentTimeStamp);
    }

    /// <summary>
    /// Resend an email request for changing email.
    /// </summary>
    /// <param name="token">The token for resending the email.</param>
    /// <param name="resendEncryptionKey">The encryption key for resending the email.</param>
    /// <param name="changeEncryptionKey">The encryption key for changing the email.</param>
    /// <param name="changeTokenExpiry">The expiry time for the change email token.</param>
    /// <param name="resendTokenExpiry">The expiry time for the resend email token.</param>
    /// <param name="sendEmailAction">The action to send the email.</param>
    private async Task<ChangeEmailResponseModel> ResendEmailRequestAsync(
        string token,
        string resendEncryptionKey,
        string changeEncryptionKey,
        int changeTokenExpiry,
        int resendTokenExpiry,
        Func<string, string, string, string, string, Task<ChangeEmailResponseModel>> sendEmailAction
    )
    {
        var currentTimeStamp = DateTime.UtcNow;
        var (oldEmail, newEmail) = VerifyResendAndEmailChangeToken(
            token,
            currentTimeStamp,
            resendEncryptionKey
        );
        if (string.IsNullOrWhiteSpace(oldEmail) || string.IsNullOrWhiteSpace(newEmail))
        {
            throw new ForbiddenException(_localizer.GetString("EmailShouldNotEmpty"));
        }

        var user = await GetUserByEmailAsync(oldEmail).ConfigureAwait(false);
        if (user == null)
        {
            throw new ForbiddenException(
                _localizer.GetString("UserNotFoundWithEmail") + " " + newEmail
            );
        }

        var changeEmailToken = GenerateResendAndChangeEmailToken(
            oldEmail,
            newEmail,
            changeEncryptionKey,
            changeTokenExpiry
        );
        var resendToken = GenerateResendAndChangeEmailToken(
            oldEmail,
            newEmail,
            resendEncryptionKey,
            resendTokenExpiry
        );
        var companyName = await _unitOfWork
            .GetRepository<GeneralSetting>()
            .GetFirstOrDefaultAsync(selector: x => x.CompanyName)
            .ConfigureAwait(false);

        return await sendEmailAction(
            oldEmail,
            newEmail,
            changeEmailToken,
            resendToken,
            companyName
        );
    }

    /// <summary>
    /// Verifies the email change request.
    /// </summary>
    /// <param name="token">The token for email change.</param>
    /// <param name="encryptionKey">The encryption key for email change.</param>
    /// <param name="verifyAction">The action to verify the email change.</param>
    private async Task VerifyEmailChangeAsync(
        string token,
        string encryptionKey,
        Func<string, string, User, DateTime, Task> verifyAction
    )
    {
        var currentTimeStamp = DateTime.UtcNow;
        var (oldEmail, newEmail) = VerifyResendAndEmailChangeToken(
            token,
            currentTimeStamp,
            encryptionKey
        );
        if (string.IsNullOrWhiteSpace(oldEmail) || string.IsNullOrWhiteSpace(newEmail))
        {
            throw new ForbiddenException(_localizer.GetString("EmailShouldNotEmpty"));
        }

        var user = await GetUserByEmailAsync(oldEmail).ConfigureAwait(false);
        if (user == null)
        {
            throw new ForbiddenException(
                _localizer.GetString("UserNotFoundWithEmail") + " " + newEmail
            );
        }

        await verifyAction(oldEmail, newEmail, user, currentTimeStamp);
    }

    /// <summary>
    /// Retrieves a user based on the provided predicate.
    /// </summary>
    /// <param name="predicate">The expression to filter the user.</param>
    /// <param name="notFoundMessage">The message to display if the user is not found.</param>
    /// <returns>The user entity.</returns>
    private async Task<User> GetUserByPredicateAsync(
        Expression<Func<User, bool>> predicate,
        string notFoundMessage
    )
    {
        try
        {
            var user = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: predicate)
                .ConfigureAwait(false);
            if (user == default)
            {
                throw new EntityNotFoundException(_localizer.GetString(notFoundMessage));
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while attempting to fetch user.");
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    /// <summary>
    /// Generates access and refresh tokens for a user.
    /// </summary>
    /// <param name="authenticationModel">The authentication model to store token information.</param>
    /// <param name="user">The user entity.</param>
    /// <param name="currentTimeStamp">The current timestamp.</param>
    /// <returns>The authentication model with token information.</returns>
    private async Task<AuthenticationModel> GenerateAccessAndRefreshToken(
        AuthenticationModel authenticationModel,
        User user,
        DateTime currentTimeStamp
    )
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = await GetUniqueRefreshToken().ConfigureAwait(false),
            LoginAt = currentTimeStamp,
            UserId = user.Id,
            IsActive = true,
        };

        authenticationModel.IsAuthenticated = true;
        var jwtSecurityToken = await CreateJwtToken(user);
        authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authenticationModel.ExpirationDuration = Convert.ToInt32(_jwt.DurationInMinutes);
        authenticationModel.Email = user.Email;
        authenticationModel.UserId = user.Id;
        authenticationModel.Role = user.Role;
        authenticationModel.RefreshToken = refreshToken.Token;
        authenticationModel.UserId = user.Id;

        await _refreshTokenService.CreateAsync(refreshToken).ConfigureAwait(false);
        return authenticationModel;
    }

    /// <summary>
    /// Creates a JWT token for a user.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <returns>The JWT security token.</returns>
    private async Task<JwtSecurityToken> CreateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.FirstName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id.ToString()),
            new Claim("role", user.Role.ToString()),
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(
            symmetricSecurityKey,
            SecurityAlgorithms.HmacSha256
        );
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials
        );

        await Task.CompletedTask;
        return jwtSecurityToken;
    }

    /// <summary>
    /// Retrieves the refresh token by its value.
    /// </summary>
    /// <param name="token">The token value.</param>
    /// <returns>The refresh token entity.</returns>
    private async Task<RefreshToken> GetUserRefreshToken(string token)
    {
        return await _refreshTokenService.GetByValue(token).ConfigureAwait(false);
    }

    /// <summary>
    /// Generates a unique refresh token.
    /// </summary>
    /// <returns>The unique refresh token.</returns>
    private async Task<string> GetUniqueRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshToken = await _refreshTokenService.GetByValue(token).ConfigureAwait(false);
        if (refreshToken == null)
        {
            return token;
        }

        return await GetUniqueRefreshToken().ConfigureAwait(false);
    }

    /// <summary>
    /// Builds a JWT token for resetting the password.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="allowedMinutes">The allowed minutes for the token to be valid.</param>
    /// <returns>The JWT token.</returns>
    private async Task<string> BuildResetPasswordJWTToken(string email, double allowedMinutes = 5)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(
            symmetricSecurityKey,
            SecurityAlgorithms.HmacSha256
        );
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Email, email), };

        var token = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(allowedMinutes),
            signingCredentials: signingCredentials
        );

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    /// <summary>
    /// Verifies the resend and email change token.
    /// </summary>
    /// <param name="token">The token for resending and changing email.</param>
    /// <param name="currentTimeStamp">The current timestamp.</param>
    /// <param name="encryptionKey">The encryption key for the token.</param>
    /// <returns>A tuple containing the old and new email addresses.</returns>
    private (string, string) VerifyResendAndEmailChangeToken(
        string token,
        DateTime currentTimeStamp,
        string encryptionKey
    )
    {
        try
        {
            var securityKey = Encoding.UTF8.GetBytes(encryptionKey);
            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(securityKey)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out var security
            );
            var oldEmail = principal.Claims?.FirstOrDefault(x => x.Type == "oldEmail")?.Value;
            var newEmail = principal.Claims?.FirstOrDefault(x => x.Type == "newEmail")?.Value;
            var tokenExpiry = security.ValidTo;
            if (tokenExpiry < currentTimeStamp)
            {
                throw new ForbiddenException(_localizer.GetString("TokenExpired"));
            }

            return (oldEmail, newEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while attempting to verify change email token."
            );
            if (ex is SecurityTokenInvalidSignatureException)
            {
                throw new ForbiddenException(_localizer.GetString("TokenSignatureNotFormatted"));
            }

            if (ex is SecurityTokenExpiredException)
            {
                throw new ForbiddenException(_localizer.GetString("TokenExpired"));
            }

            throw ex is ServiceException
                ? ex
                : new ServiceException(_localizer.GetString("ErrorOccurredVerifyChangeEmailToken"));
        }
    }

    /// <summary>
    /// Checks for duplicate email addresses.
    /// </summary>
    /// <param name="entity">The user entity.</param>
    private async Task CheckDuplicateEmailAsync(User entity)
    {
        var checkDuplicateEmail = await _unitOfWork
            .GetRepository<User>()
            .ExistsAsync(predicate: p =>
                p.Id != entity.Id && p.Email.ToLower().Equals(entity.Email.ToLower())
            )
            .ConfigureAwait(false);
        if (checkDuplicateEmail)
        {
            _logger.LogWarning("Duplicate user email : {email} is found.", entity.Email);
            throw new ServiceException(_localizer.GetString("DuplicateEmailFound"));
        }
    }

    /// <summary>
    /// Checks for duplicate member IDs.
    /// </summary>
    /// <param name="entity">The user entity.</param>
    private async Task CheckForDuplicateMemberId(User entity)
    {
        var checkDuplicateMemberId = await _unitOfWork
            .GetRepository<User>()
            .ExistsAsync(predicate: p =>
                p.Id != entity.Id && p.MemberId.ToLower().Equals(entity.MemberId.ToLower())
            )
            .ConfigureAwait(false);
        if (checkDuplicateMemberId)
        {
            _logger.LogWarning("Duplicate MemberId : {UserId} is found.", entity.Id);
            throw new ServiceException(_localizer.GetString("DuplicateMemberIdFound"));
        }
    }

    /// <summary>
    /// Checks the bulk import data for validity.
    /// </summary>
    /// <param name="checkForValidRows">The data to check.</param>
    /// <param name="currentUserId">The current user ID.</param>
    private async Task CheckBulkImport(
        (List<UserImportDto> userList, List<int> SN) checkForValidRows,
        Guid currentUserId
    )
    {
        try
        {
            var checkUser = await _unitOfWork
                .GetRepository<User>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == currentUserId)
                .ConfigureAwait(false);

            if (checkForValidRows.userList.Count == default)
            {
                throw new ForbiddenException(_localizer.GetString("CSVNullError"));
            }

            if (checkForValidRows.userList.Any(x => string.IsNullOrWhiteSpace(x.FirstName)))
            {
                var selectedSNs = checkForValidRows
                    .userList.Select((user, index) => new { User = user, Index = index })
                    .Where(x => string.IsNullOrWhiteSpace(x.User.FirstName))
                    .Select(x => checkForValidRows.SN.ElementAtOrDefault(x.Index) + 1)
                    .ToList();
                if (selectedSNs.Any())
                {
                    throw new ForbiddenException(
                        _localizer.GetString("IncorrectFirstName")
                            + " "
                            + string.Join(", ", selectedSNs)
                            + " "
                            + _localizer.GetString("TryAgain")
                    );
                }
            }

            if (checkForValidRows.userList.Any(x => string.IsNullOrWhiteSpace(x.LastName)))
            {
                var selectedSNs = checkForValidRows
                    .userList.Select((user, index) => new { User = user, Index = index })
                    .Where(x => string.IsNullOrWhiteSpace(x.User.LastName))
                    .Select(x => checkForValidRows.SN.ElementAtOrDefault(x.Index) + 1)
                    .ToList();

                if (selectedSNs.Any())
                {
                    throw new ForbiddenException(
                        _localizer.GetString("IncorrectLastName")
                            + " "
                            + string.Join(",", selectedSNs)
                            + " "
                            + _localizer.GetString("TryAgain")
                    );
                }
            }

            if (checkForValidRows.userList.Any(x => string.IsNullOrWhiteSpace(x.Email)))
            {
                var selectedSNs = checkForValidRows
                    .userList.Select((user, index) => new { User = user, Index = index })
                    .Where(x => string.IsNullOrWhiteSpace(x.User.Email))
                    .Select(x => checkForValidRows.SN.ElementAtOrDefault(x.Index) + 1)
                    .ToList();
                throw new ForbiddenException(
                    _localizer.GetString("IncorrectEmail")
                        + " "
                        + string.Join(", ", selectedSNs)
                        + " "
                        + _localizer.GetString("TryAgain")
                );
            }

            if (!checkForValidRows.userList.Any(x => string.IsNullOrWhiteSpace(x.Email)))
            {
                var emailPattern =
                    @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
                    + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
                    + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                var invalidEmailRows = checkForValidRows
                    .userList.Select((user, index) => (user.Email, index))
                    .Where(entry => !Regex.IsMatch(entry.Email, emailPattern))
                    .Select(entry => entry.Item2 + 2)
                    .ToList();
                if (invalidEmailRows.Any())
                {
                    throw new ForbiddenException(
                        _localizer.GetString("IncorrectEmailFormat")
                            + " "
                            + string.Join(", ", invalidEmailRows)
                            + " "
                            + _localizer.GetString("TryAgain")
                    );
                }
            }

            if (!checkForValidRows.userList.Any(x => string.IsNullOrWhiteSpace(x.MobileNumber)))
            {
                var mobileNumberPattern = @"^[+\d]+$";

                var invalidMobileNumberRows = checkForValidRows
                    .userList.Select((user, index) => (user.MobileNumber, index))
                    .Where(entry => !Regex.IsMatch(entry.MobileNumber, mobileNumberPattern))
                    .Select(entry => entry.Item2 + 2)
                    .ToList();
                if (invalidMobileNumberRows.Any())
                {
                    throw new ForbiddenException(
                        _localizer.GetString("IncorrectMobileNumberFormat")
                            + " "
                            + string.Join(", ", invalidMobileNumberRows)
                            + " "
                            + _localizer.GetString("TryAgain")
                    );
                }
            }

            if (
                checkForValidRows.userList.Any(x =>
                    string.IsNullOrWhiteSpace(x.Role) || !Enum.TryParse<UserRole>(x.Role, out _)
                )
            )
            {
                var selectedSN = checkForValidRows
                    .userList.Select((user, index) => new { User = user, Index = index })
                    .Where(x => string.IsNullOrWhiteSpace(x.User.Role))
                    .Select(x => checkForValidRows.SN.ElementAtOrDefault(x.Index) + 1)
                    .ToList();

                if (selectedSN.Any())
                {
                    throw new ForbiddenException(
                        _localizer.GetString("IncorrectRole")
                            + " "
                            + string.Join(", ", selectedSN)
                            + " "
                            + _localizer.GetString("TryAgain")
                    );
                }

                var selectedSNs = checkForValidRows
                    .userList.Select((user, index) => new { User = user, Index = index })
                    .Where(x =>
                        !Enum.GetNames(typeof(UserRole))
                            .Any(enumValue =>
                                string.Equals(
                                    enumValue,
                                    x.User.Role,
                                    StringComparison.OrdinalIgnoreCase
                                )
                            )
                    )
                    .Join(
                        checkForValidRows.SN.Select((sn, index) => new { SN = sn, Index = index }),
                        x => x.Index,
                        sn => sn.Index,
                        (x, sn) => sn.SN + 1
                    )
                    .ToList();

                if (selectedSNs.Any())
                {
                    throw new ForbiddenException(
                        _localizer.GetString("IncorrectRoleFormat")
                            + " "
                            + string.Join(", ", selectedSNs)
                            + " "
                            + _localizer.GetString("TryAgain")
                    );
                }

                var selectedIndices = Enum.GetValues(typeof(UserRole))
                    .Cast<UserRole>()
                    .Select((role, index) => new { Role = role, Index = index })
                    .Where(x =>
                        string.Equals(
                            x.Role.ToString(),
                            UserRole.Admin.ToString(),
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .Select(x => x.Index + 1)
                    .ToList();
                if (selectedIndices.Any() && checkUser.Role != UserRole.SuperAdmin)
                {
                    throw new ForbiddenException(_localizer.GetString("AdminCannotAddAdmin"));
                }
            }
        }
        catch (ForbiddenException ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while attempting to fetch user detail information."
            );
            throw ex is ServiceException ? ex : new ServiceException(ex.Message);
        }
    }

    #endregion Private Methods
}
