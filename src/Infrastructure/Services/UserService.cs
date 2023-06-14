using CsvHelper;
using Lingtren.Domain.Entities;
using Hangfire;
using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.Common.Models.ResponseModels;
using Lingtren.Domain.Enums;
using Lingtren.Infrastructure.Common;
using Lingtren.Infrastructure.Configurations;
using Lingtren.Infrastructure.Localization;
using LinqKit;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Lingtren.Infrastructure.Services
{
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

        public UserService(IUnitOfWork unitOfWork,
            ILogger<UserService> logger,
            IEmailService emailService,
            IRefreshTokenService refreshTokenService,
            IOptions<JWT> jwt,
            IConfiguration configuration,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IGeneralSettingService generalSettingService) : base(unitOfWork, logger, localizer)
        {
            _emailService = emailService;
            _refreshTokenService = refreshTokenService;
            _jwt = jwt.Value;
            _changeEmailEncryptionKey = configuration.GetSection("ChangeEmail:EncryptionKey").Value;
            _changeEmailTokenExpiry = int.Parse(configuration.GetSection("ChangeEmail:ExpireInMinutes").Value);
            _resendChangeEmailEncryptionKey = configuration.GetSection("ResendChangeEmail:EncryptionKey").Value;
            _resendChangeEmailTokenExpiry = int.Parse(configuration.GetSection("ResendChangeEmail:ExpireInMinutes").Value);
            _generalSettingService = generalSettingService;
        }

        #region Account Services

        /// <summary>
        /// Handle to verified user during login and generate token
        /// </summary>
        /// <param name="tokenRequestDto">the instance of <see cref="TokenRequestDto"/></param>
        /// <returns>the instance of <see cref="AuthenticationModel"/></returns>
        public async Task<AuthenticationModel> VerifyUserAndGetToken(LoginRequestModel model)
        {
            var authenticationModel = new AuthenticationModel();

            var user = await GetUserByEmailAsync(email: model.Email.Trim());
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

            var isUserAuthenticated = VerifyPassword(user.HashPassword, model.Password);
            if (!isUserAuthenticated)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = _localizer.GetString("IncorrectCredentials");
                return authenticationModel;
            }

            var currentTimeStamp = DateTime.UtcNow;

            if (user.Status == UserStatus.Pending)
            {
                user.Status = UserStatus.Active;
                user.UpdatedBy = user.Id;
                user.UpdatedOn = currentTimeStamp;

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = await GetUniqueRefreshToken().ConfigureAwait(false),
                LoginAt = currentTimeStamp,
                UserId = user.Id,
                IsActive = true,
            };

            //Create Token 
            authenticationModel.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
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
        /// Handle to logout user and set refresh token false
        /// </summary>
        /// <param name="token">the refresh token</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<bool> Logout(string token, Guid currentUserId)
        {
            var user = await GetUserFromRefreshToken(token);
            // return false if no user found with token
            if (user == null)
            {
                return false;
            }
            if (user.Id != currentUserId)
            {
                return false;
            }
            var refreshToken = await GetUserRefreshToken(token);
            // return false if token is not active
            if (!refreshToken.IsActive)
            {
                return false;
            }
            await _refreshTokenService.DeleteAsync(refreshToken);
            return true;
        }

        /// <summary>
        /// Handle to generate new jwt token from refresh token
        /// </summary>
        /// <param name="token">the refresh token</param>
        /// <returns></returns>
        public async Task<AuthenticationModel> RefreshTokenAsync(string token)
        {
            var authenticationModel = new AuthenticationModel();

            var user = await GetUserFromRefreshToken(token);
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = _localizer.GetString("TokenNotMatched");
                return authenticationModel;
            }
            var refreshToken = await GetUserRefreshToken(token);
            if (refreshToken == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = _localizer.GetString("TokenNotFound");
                return authenticationModel;
            }

            if (!refreshToken.IsActive)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = _localizer.GetString("TokenNotActive");
                return authenticationModel;
            }
            refreshToken.IsActive = false;

            //Revoke Current Refresh Token
            await _refreshTokenService.UpdateAsync(refreshToken);

            //Generate new Refresh Token and save to Database
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = await GetUniqueRefreshToken().ConfigureAwait(false),
                LoginAt = DateTime.UtcNow,
                IsActive = true,
                UserId = user.Id,
            };

            await _refreshTokenService.CreateAsync(newRefreshToken);

            //Generates new jwt
            authenticationModel.UserId = user.Id;
            authenticationModel.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
            authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authenticationModel.ExpirationDuration = Convert.ToInt32(_jwt.DurationInMinutes);
            authenticationModel.Email = user.Email;
            authenticationModel.Role = user.Role;
            authenticationModel.RefreshToken = newRefreshToken.Token;
            return authenticationModel;
        }

        /// <summary>
        /// Get single active refresh token of the user
        /// </summary>
        /// <param name="user">the instance of <see cref="User"/></param>
        /// <returns>the instance of <see cref="RefreshToken"/></returns>
        public async Task<RefreshToken?> GetActiveRefreshToken(User user)
        {
            var refreshTokens = await _refreshTokenService.GetByUserId(user.Id).ConfigureAwait(false);
            return refreshTokens.FirstOrDefault();
        }

        /// <summary>
        /// Handle to get user detail by id
        /// </summary>
        /// <param name="id">the user id</param>
        /// <returns>the instance of <see cref="User"/></returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Email == email).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to fetch user by email.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("UserNotFound"));
            }
        }

        /// <summary>
        /// Handle to hash password
        /// </summary>
        /// <param name="password">the password</param>
        /// <param name="salt"></param>
        /// <param name="needsOnlyHash"></param>
        /// <returns></returns>
        public string HashPassword(string password, byte[]? salt = null, bool needsOnlyHash = false)
        {
            if (salt == null || salt.Length != 16)
            {
                // generate a 128-bit salt using a secure PRNG
                salt = new byte[128 / 8];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (needsOnlyHash) return hashed;
            // password will be concatenated with salt using ':'
            return $"{hashed}:{Convert.ToBase64String(salt)}";
        }

        /// <summary>
        /// Handle to get trainer 
        /// </summary>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="TrainerResponseModel"/></returns>
        public async Task<IList<TrainerResponseModel>> GetTrainerAsync(Guid currentUserId, string search)
        {
            return await ExecuteWithResultAsync(async () =>
          {
              var isValidUser = await IsSuperAdminOrAdminOrTrainer(currentUserId).ConfigureAwait(false);
              if (!isValidUser)
              {
                  throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
              }
              var predicate = PredicateBuilder.New<User>(true);
              if (!string.IsNullOrWhiteSpace(search))
              {
                  search = search.ToLower().Trim();
                  predicate = predicate.And(x => x.FirstName.ToLower().Trim().Contains(search)
                  || x.LastName.ToLower().Trim().Contains(search)
                  || x.Email.ToLower().Trim().Contains(search));
              }
              predicate = predicate.And(p => p.Role == UserRole.Admin || p.Role == UserRole.Trainer);
              return await _unitOfWork.GetRepository<User>().GetAllAsync(predicate: predicate,
                    selector: s => new TrainerResponseModel(s)).ConfigureAwait(false);
          });
        }


        /// <summary>
        /// Handle to import the user
        /// </summary>
        /// <param name="file"> the instance of <see cref="IFormFile" /> .</param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
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
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    while (csv.Read())
                    {
                        var user = csv.GetRecord<UserImportDto>();
                        users.Add(user);
                    }
                }
                var message = new StringBuilder();
                var inValidUsers = users.Where(x => string.IsNullOrWhiteSpace(x.FirstName) || string.IsNullOrWhiteSpace(x.LastName) || string.IsNullOrWhiteSpace(x.Email)).ToList();
                foreach(var user in inValidUsers)
                {
                    message.AppendLine(_localizer.GetString("DuplicateEmailDetected"));
                }
                users = users.Where(x => !string.IsNullOrWhiteSpace(x.FirstName) && !string.IsNullOrWhiteSpace(x.LastName) && !string.IsNullOrWhiteSpace(x.Email)).ToList();

                var company = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync().ConfigureAwait(false);
                var stringBuilder = new StringBuilder();
                if (users.Count != default)
                {
                    var userEmails = users.ConvertAll(x => x.Email);
                    var duplicateUser = await _unitOfWork.GetRepository<User>().GetAllAsync(predicate: p => userEmails.Contains(p.Email), selector: x => x.Email).ConfigureAwait(false);
                    foreach (var entity in duplicateUser)
                    {
                        message.AppendLine(string.Join(",", entity) + " " + _localizer.GetString("AlreadyRegistered"));
                    }
                    var newUsersList = users.Where(x => !duplicateUser.Contains(x.Email)).ToList();
                    newUsersList = newUsersList.DistinctBy(x => x.Email).ToList();
                    var newUsers = new List<User>();
                    var newUserEmails = new List<UserEmailDto>();
                    if (newUsersList.Count != default)
                    {
                        foreach (var user in newUsersList)
                        {
                            var userEntity = new User()
                            {
                                Id = Guid.NewGuid(),
                                FirstName = user.FirstName,
                                MiddleName = user.MiddleName,
                                LastName = user.LastName,
                                Email = user.Email,
                                Status = UserStatus.Pending,
                                Profession = user.Profession,
                                MobileNumber = user.MobileNumber,
                                Role = Enum.Parse<UserRole>(user.Role),
                                CreatedBy = currentUserId,
                                CreatedOn = DateTime.UtcNow
                            };
                            var password = await GenerateRandomPassword(8).ConfigureAwait(false);
                            userEntity.HashPassword = HashPassword(password);

                            var userEmailDto = new UserEmailDto
                            {
                                FullName = userEntity.FullName,
                                Email = userEntity.Email,
                                Password = password,
                                CompanyName = company.CompanyName
                            };
                            newUsers.Add(userEntity);
                            newUserEmails.Add(userEmailDto);
                        }
                        await _unitOfWork.GetRepository<User>().InsertAsync(newUsers).ConfigureAwait(false);
                        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                        BackgroundJob.Enqueue<IHangfireJobService>(job => job.SendEmailImportedUserAsync(newUserEmails, null));
                        message.AppendLine(_localizer.GetString("UserImported"));
                    }
                }
                else
                {
                    message.AppendLine(_localizer.GetString("NoUserImported"));
                }
                return message.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(_localizer.GetString("NoUserImported"));
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("NoUserImported"));
            }
        }

        /// <summary>
        /// Handle to verify password
        /// </summary>
        /// <param name="hashedPasswordWithSalt">the hashed password</param>
        /// <param name="password">the password</param>
        /// <returns></returns>
        public bool VerifyPassword(string hashedPasswordWithSalt, string password)
        {
            // retrieve both salt and password from 'hashedPasswordWithSalt'
            var passwordAndHash = hashedPasswordWithSalt.Split(':');
            if (passwordAndHash == null || passwordAndHash.Length != 2)
                return false;
            var salt = Convert.FromBase64String(passwordAndHash[1]);
            if (salt == null)
                return false;
            // hash the given password
            var hashOfPasswordToCheck = HashPassword(password, salt, true);
            // compare both hashes
            return string.Compare(passwordAndHash[0], hashOfPasswordToCheck) == 0;
        }

        /// <summary>
        /// Handle to reset password
        /// </summary>
        /// <param name="model">the instance of <see cref="VerifyResetTokenModel"/></param>
        /// <returns>the password change token</returns>
        public async Task ResetPasswordAsync(User user)
        {
            try
            {
                var generator = new Random();
                var token = generator.Next(0, 1000000).ToString("D6");
                var tokenExpiry = DateTime.UtcNow.AddMinutes(5);
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiry = tokenExpiry;
                user.Status = UserStatus.Active;
                _unitOfWork.GetRepository<User>().Update(user);
                var companyName = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync(selector: x => x.CompanyName).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                await _emailService.SendForgetPasswordEmail(user.Email, user.FirstName, token, companyName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to verify reset token.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredVerifyResetToken"));
            }
        }

        /// <summary>
        /// Handle to verify reset token
        /// </summary>
        /// <param name="model">the instance of <see cref="VerifyResetTokenModel"/></param>
        /// <returns>the password change token</returns>
        public async Task<string> VerifyPasswordResetTokenAsync(VerifyResetTokenModel model)
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                var user = await GetUserByEmailAsync(model.Email).ConfigureAwait(false);
                if (user == null)
                {
                    _logger.LogWarning("User not found with email: {email}.", model.Email);
                    throw new EntityNotFoundException(_localizer.GetString("UserNotFound"));
                }
                if (currentTimeStamp > user.PasswordResetTokenExpiry)
                {
                    _logger.LogWarning("Password reset token expired for the user with id : {id}.", user.Id);
                    throw new ForbiddenException(_localizer.GetString("ResetTokenExpired"));
                }
                if (model.Token != user.PasswordResetToken)
                {
                    _logger.LogWarning("User not found with email: {email}.", model.Email);
                    throw new ForbiddenException(_localizer.GetString("ResetTokenNotMatched"));
                }
                user.PasswordChangeToken = await BuildResetPasswordJWTToken(user.Email).ConfigureAwait(false);
                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return user.PasswordChangeToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to verify reset token.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredVerifyResetToken"));
            }
        }

        /// <summary>
        /// Handle to generate random password
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<string> GenerateRandomPassword(int length)
        {
            Random random = new();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var password = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return await Task.FromResult(password.ToLower());
        }

        /// <summary>
        /// Handle to change user password
        /// </summary>
        /// <param name="model">the instance of <see cref="ChangePasswordRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        public async Task ChangePasswordAsync(ChangePasswordRequestModel model, Guid currentUserId)
        {
            var user = await GetAsync(currentUserId, includeProperties: false).ConfigureAwait(false);
            var currentPasswordMatched = VerifyPassword(user.HashPassword, model.CurrentPassword);
            if (!currentPasswordMatched)
            {
                _logger.LogWarning("User with userId : {id} current password does not matched while changing password.", currentUserId);
                throw new ForbiddenException(_localizer.GetString("CurrentPasswordNotMatched"));
            }
            user.HashPassword = HashPassword(model.NewPassword);
            user.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.GetRepository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Handle to change user email
        /// </summary>
        /// <param name="model">the instance of <see cref="ChangeEmailRequestModel"/></param>
        /// <returns>the instance of <see cref="ChangeEmailResponseModel"/></returns>
        public async Task<ChangeEmailResponseModel> ChangeEmailRequestAsync(ChangeEmailRequestModel model, Guid currentUserId)
        {
            var user = await GetUserByEmailAsync(model.OldEmail).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with email : {email} not found.", model.OldEmail);
                throw new ForbiddenException(_localizer.GetString("UserNotFoundWithEmail") + " " + model.OldEmail);
            }
            var newUser = await GetUserByEmailAsync(model.NewEmail).ConfigureAwait(false);
            if (newUser != null)
            {
                _logger.LogWarning("User with new email : {email} found in the system.", model.NewEmail);
                throw new ForbiddenException(model.NewEmail + " " + _localizer.GetString("AlreadyExistInAnotherAccount"));
            }
            var isUserAuthenticated = VerifyPassword(user.HashPassword, model.Password);
            if (!isUserAuthenticated)
            {
                _logger.LogWarning("User with id : {userId} password not matched for email change.", user.Id);
                throw new ForbiddenException(_localizer.GetString("PasswordNotMatched"));
            }
            if (user.Id != currentUserId)
            {
                _logger.LogWarning("User with email: {email} is invalid user request to change email of current user with id: {currentUserId}", user.Email, currentUserId);
                throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
            }
            var companyName = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync(selector: x => x.CompanyName).ConfigureAwait(false);
            var changeEmailToken = GenerateResendAndChangeEmailToken(model.OldEmail, model.NewEmail, _changeEmailEncryptionKey, _changeEmailTokenExpiry);
            var resendToken = GenerateResendAndChangeEmailToken(model.OldEmail, model.NewEmail, _resendChangeEmailEncryptionKey, _resendChangeEmailTokenExpiry);
            await _emailService.SendChangePasswordMailAsync(model.NewEmail, user.FirstName, changeEmailToken, _changeEmailTokenExpiry, companyName).ConfigureAwait(false);
            return new ChangeEmailResponseModel() { ResendToken = resendToken };
        }

        /// <summary>
        /// Handle to resend email async
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task ResendEmailAsync(Guid userId, Guid currentUserId)
        {
            try
            {
                var isSuperAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!isSuperAdmin)
                {
                    throw new ForbiddenException("You are not authorized to resend email.");
                }

                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Id == userId).ConfigureAwait(false);
                if (user == default)
                {
                    throw new ForbiddenException("User not found.");
                }

                if (user.Status != UserStatus.Pending)
                {
                    throw new ArgumentException("User is already active.");
                }
                var password = await GenerateRandomPassword(8).ConfigureAwait(false);
                var hashPassword = HashPassword(password);
                user.HashPassword = hashPassword;
                user.UpdatedBy = currentUserId;
                user.UpdatedOn = DateTime.UtcNow;
                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                var company = await _generalSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
                BackgroundJob.Enqueue<IHangfireJobService>(job => job.SendUserCreatedPasswordEmail(user.Email, user.FullName, password, company.CompanyName, null));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured on ResendEmailAsync method : {ex.Message}");
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to resend change user email
        /// </summary>
        /// <param name="token">the resend token</param>
        /// <returns>the instance of <see cref="ChangeEmailResponseModel"/></returns>
        public async Task<ChangeEmailResponseModel> ResendChangeEmailRequestAsync(string token)
        {
            var currentTimeStamp = DateTime.UtcNow;
            var (oldEmail, newEmail) = VerifyResendAndEmailChangeToken(token, currentTimeStamp, _resendChangeEmailEncryptionKey);
            if (string.IsNullOrWhiteSpace(oldEmail) || string.IsNullOrWhiteSpace(newEmail))
            {
                _logger.LogWarning("Old email or new email is null or empty for resend change email.");
                throw new ForbiddenException(_localizer.GetString("EmailShouldNotEmpty"));
            }
            var user = await GetUserByEmailAsync(oldEmail).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with email : {email} not found.", oldEmail);
                throw new ForbiddenException(_localizer.GetString("UserNotFoundWithEmail") + " " + newEmail);
            }
            var changeEmailToken = GenerateResendAndChangeEmailToken(oldEmail, newEmail, _changeEmailEncryptionKey, _changeEmailTokenExpiry);
            var resendToken = GenerateResendAndChangeEmailToken(oldEmail, newEmail, _resendChangeEmailEncryptionKey, _resendChangeEmailTokenExpiry);
            var companyName = await _unitOfWork.GetRepository<GeneralSetting>().GetFirstOrDefaultAsync(selector: x => x.CompanyName).ConfigureAwait(false);
            await _emailService.SendChangePasswordMailAsync(newEmail, user.FirstName, changeEmailToken, _changeEmailTokenExpiry, companyName).ConfigureAwait(false);
            return new ChangeEmailResponseModel() { ResendToken = resendToken };
        }

        /// <summary>
        /// Handle to verify user email change
        /// </summary>
        /// <param name="token">the token</param>
        /// <returns></returns>
        public async Task VerifyChangeEmailAsync(string token)
        {
            var currentTimeStamp = DateTime.UtcNow;
            var (oldEmail, newEmail) = VerifyResendAndEmailChangeToken(token, currentTimeStamp, _changeEmailEncryptionKey);
            if (string.IsNullOrWhiteSpace(oldEmail) || string.IsNullOrWhiteSpace(newEmail))
            {
                _logger.LogWarning("Old email or new email is null or empty for verify change email.");
                throw new ForbiddenException(_localizer.GetString("EmailShouldNotEmpty"));
            }
            var user = await GetUserByEmailAsync(oldEmail).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with email : {email} not found.", oldEmail);
                throw new ForbiddenException(_localizer.GetString("UserNotFoundWithEmail") + " " + newEmail);
            }
            user.Email = newEmail;
            user.UpdatedOn = currentTimeStamp;

            _unitOfWork.GetRepository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Handle to generate change email jwt token
        /// </summary>
        /// <param name="oldEmail">the old email</param>
        /// <param name="newEmail">the new email for change requested</param>
        /// <returns>the jwt token</returns>
        public static string GenerateResendAndChangeEmailToken(string oldEmail, string newEmail, string encryptionKey, int tokenExpiry)
        {
            byte[] securityKey = Encoding.UTF8.GetBytes(encryptionKey);
            var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>()
                {
                    {"oldEmail",oldEmail },
                    {"newEmail",newEmail},
                },
                Expires = DateTime.UtcNow.AddMinutes(tokenExpiry),
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        /// <summary>
        /// Verify email change token
        /// </summary>
        /// <param name="token">the email change token</param>
        /// <param name="currentTimeStamp">the current time stamp</param>
        /// <returns>the old email and new email</returns>
        private (string?, string?) VerifyResendAndEmailChangeToken(string token, DateTime currentTimeStamp, string encryptionKey)
        {
            try
            {
                byte[] securityKey = Encoding.UTF8.GetBytes(encryptionKey);
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
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var security);
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
                _logger.LogError(ex, "An error occurred while attempting to verify change email token.");
                if (ex is SecurityTokenInvalidSignatureException)
                {
                    throw new ForbiddenException(_localizer.GetString("TokenSignatureNotFormatted"));
                }
                if (ex is SecurityTokenExpiredException)
                {
                    throw new ForbiddenException(_localizer.GetString("TokenExpired"));
                }
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredVerifyChnageEmailToken"));
            }
        }
        #endregion Account Services

        #region Protected Methods

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(User entity)
        {
            await CheckDuplicateEmailAsync(entity).ConfigureAwait(false);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Construct query condition according to search criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected override Expression<Func<User, bool>> ConstructQueryConditions(Expression<Func<User, bool>> predicate, UserSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x =>
                    ((x.FirstName.Trim() + " " + x.MiddleName.Trim()).Trim() + " " + x.LastName.Trim()).Trim().Contains(search)
                 || x.Email.ToLower().Trim().Contains(search)
                 || x.MobileNumber.ToLower().Trim().Contains(search));
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
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(UserSearchCriteria criteria)
        {
            criteria.SortBy = nameof(User.CreatedOn);
            criteria.SortType = SortType.Descending;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<User, object> IncludeNavigationProperties(IQueryable<User> query)
        {
            return query.Include(x => x.Department);
        }
        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Handle to create jwt token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            await Task.CompletedTask;
            return jwtSecurityToken;
        }
        private async Task<RefreshToken> GetUserRefreshToken(string token)
        {
            return await _refreshTokenService.GetByValue(token).ConfigureAwait(false);
        }
        private async Task<User?> GetUserFromRefreshToken(string token)
        {
            var userRefreshToken = await GetUserRefreshToken(token).ConfigureAwait(false);
            if (userRefreshToken != null)
            {
                var user = await GetAsync(userRefreshToken.UserId, includeProperties: false);
                return user;
            }
            return null;
        }

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
        /// Handle to create verify reset password token
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="allowedMinutes"></param>
        /// <returns></returns>
        private async Task<string> BuildResetPasswordJWTToken(string email, double allowedMinutes = 60)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
            };

            var token = new JwtSecurityToken(_jwt.Issuer,
              _jwt.Audience,
              claims: claims,
              expires: DateTime.UtcNow.AddMinutes(allowedMinutes),
              signingCredentials: signingCredentials);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        /// <summary>
        /// Check duplicate name
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        private async Task CheckDuplicateEmailAsync(User entity)
        {
            var checkDuplicateEmail = await _unitOfWork.GetRepository<User>().ExistsAsync(
                predicate: p => p.Id != entity.Id && p.Email.ToLower().Equals(entity.Email.ToLower())).ConfigureAwait(false);
            if (checkDuplicateEmail)
            {
                _logger.LogWarning("Duplicate user email : {email} is found.", entity.Email);
                throw new ServiceException(_localizer.GetString("DuplicateEmailFound"));
            }
        }

        #endregion Private Methods

        /// <summary>
        /// Handle to fetch users detail
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>the instance of <see cref="UserResponseModel"/></returns>
        public async Task<UserResponseModel> GetDetailAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id == userId,
                    include: src => src.Include(x => x.Department)
                    ).ConfigureAwait(false);

                var userCertificates = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(
                    predicate: p => p.UserId == userId && p.HasCertificateIssued.HasValue && p.HasCertificateIssued.Value,
                    include: src => src.Include(x => x.Course)
                    ).ConfigureAwait(false);
                var externalCertificates = await _unitOfWork.GetRepository<Certificate>().GetAllAsync(predicate: p => p.CreatedBy == userId &&
                                         p.Status == CertificateStatus.Approved).ConfigureAwait(false);
                var response = new UserResponseModel(user);
                foreach (var item in userCertificates)
                {
                    response.Certificates.Add(new CourseCertificateIssuedResponseModel
                    {
                        CourseId = item.CourseId,
                        CourseName = item.Course.Name,
                        CourseSlug = item.Course.Slug,
                        Percentage = item.Percentage,
                        HasCertificateIssued = item.HasCertificateIssued,
                        CertificateIssuedDate = item.CertificateIssuedDate,
                        CertificateUrl = item.CertificateUrl,
                    });
                }

                foreach (var external in externalCertificates)
                {
                    response.ExternalCertificates.Add(new ExternalCertificateResponseModel
                    {
                        Name = external.Name,
                        StartDate = external.StartDate,
                        EndDate = external.EndDate,
                        ImageUrl = external.ImageUrl,
                        Location = external.Location,
                        Institute = external.Institute,
                        Duration = external.Duration != 0 ? external.Duration.ToString() : null
                    });
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to fetch user detail information.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFetchUserDetails"));
            }
        }

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <param name="CourseID">the current course id </param>
        /// <returns> the instance of <see cref="UserResponseModel" /> .</returns>
        public async Task<List<UserResponseModel>> GetUserForCourseEnrollment(Guid userId, string courseId)
        {
            try
            {
                var course = await ValidateAndGetCourse(userId, courseId, validateForModify: false).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("Training with identity : {identity} not found for user with id : {currentUserId}.", courseId, userId);
                    throw new EntityNotFoundException(_localizer.GetString("TrainingNotFound"));
                }
                var users = await _unitOfWork.GetRepository<User>().GetAllAsync().ConfigureAwait(false);
                var user = users.FirstOrDefault(x => x.Id == userId);
                if (user.Role == UserRole.Admin || user.Role == UserRole.SuperAdmin)
                {
                    var trimedUsers = users.Where(x => x.Id != userId && x.CourseEnrollments != course.CourseEnrollments && x.Role != UserRole.SuperAdmin && x.Role != UserRole.Admin &&
                    x.Id != course.CreatedBy && x.CourseTeachers != course.CourseTeachers);

                    var response = new List<UserResponseModel>();
                    foreach (var trimedUser in trimedUsers)
                    {
                        response.Add(new UserResponseModel
                        {
                            Id = trimedUser.Id,
                            FullName = trimedUser.FullName,
                            Address = trimedUser.Address,
                            Email = trimedUser.Email,
                            FirstName = trimedUser.FirstName,
                            LastName = trimedUser.LastName,
                            MobileNumber = trimedUser.MobileNumber,
                            Bio = trimedUser.Bio,
                            Role = trimedUser.Role,
                            DepartmentId = trimedUser.DepartmentId,
                            Status = trimedUser.Status,
                            PublicUrls = trimedUser.PublicUrls,
                        });
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
                _logger.LogError(ex, "An error occurred while attempting to fetch user detail information.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("ErrorOccurredFetchUserDetails"));
            }
        }
    }
}