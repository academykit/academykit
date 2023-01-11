namespace Lingtren.Infrastructure.Services
{
    using Domain.Entities;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Configurations;
    using LinqKit;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    public class UserService : BaseGenericService<User, UserSearchCriteria>, IUserService
    {
        private readonly IEmailService _emailService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JWT _jwt;
        private readonly string _changeEmailEncryptionKey;
        private readonly int _changeEmailTokenExpiry;
        private readonly string _resendChangeEmailEncryptionKey;
        private readonly int _resendChangeEmailTokenExpiry;

        public UserService(IUnitOfWork unitOfWork,
            ILogger<UserService> logger,
            IEmailService emailService,
            IRefreshTokenService refreshTokenService,
            IOptions<JWT> jwt,
            IConfiguration configuration) : base(unitOfWork, logger)
        {
            _emailService = emailService;
            _refreshTokenService = refreshTokenService;
            _jwt = jwt.Value;
            _changeEmailEncryptionKey = configuration.GetSection("ChangeEmail:EncryptionKey").Value;
            _changeEmailTokenExpiry = int.Parse(configuration.GetSection("ChangeEmail:ExpireInMinutes").Value);
            _resendChangeEmailEncryptionKey = configuration.GetSection("ResendChangeEmail:EncryptionKey").Value;
            _resendChangeEmailTokenExpiry = int.Parse(configuration.GetSection("ResendChangeEmail:ExpireInMinutes").Value);
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

            var user = await GetUserByEmailAsync(email: model.Email);
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Account not registered.";
                return authenticationModel;
            }

            if (!user.IsActive)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Inactive user account";
                return authenticationModel;
            }

            var isUserAuthenticated = VerifyPassword(user.HashPassword, model.Password);
            if (!isUserAuthenticated)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Incorrect User Credentials.";
                return authenticationModel;
            }
            var currentTimeStamp = DateTime.UtcNow;

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
                authenticationModel.Message = "Token did not match any users.";
                return authenticationModel;
            }
            var refreshToken = await GetUserRefreshToken(token);
            if (refreshToken == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Token not found.";
                return authenticationModel;
            }

            if (!refreshToken.IsActive)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Token Not Active.";
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
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to fetch user by email.");
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
                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                await _emailService.SendForgetPasswordEmail(user.Email, user.FirstName, token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to verify reset token.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to verify reset token.");
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
                    throw new EntityNotFoundException("User not found.");
                }
                if (currentTimeStamp > user.PasswordResetTokenExpiry)
                {
                    _logger.LogWarning("Password reset token expired for the user with id : {id}.", user.Id);
                    throw new ForbiddenException("Password reset token expired.");
                }
                if (model.Token != user.PasswordResetToken)
                {
                    _logger.LogWarning("User not found with email: {email}.", model.Email);
                    throw new ForbiddenException("Reset token not matched.");
                }
                user.PasswordChangeToken = await BuildResetPasswordJWTToken(user.Email).ConfigureAwait(false);
                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return user.PasswordChangeToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to verify reset token.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to verify reset token.");
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
                throw new ForbiddenException("Current Password does not matched.");
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
        public async Task<ChangeEmailResponseModel> ChangeEmailRequestAsync(ChangeEmailRequestModel model)
        {
            var user = await GetUserByEmailAsync(model.OldEmail).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with email : {email} not found.", model.OldEmail);
                throw new ForbiddenException($"User not found with email : {model.OldEmail}.");
            }
            var newUser = await GetUserByEmailAsync(model.NewEmail).ConfigureAwait(false);
            if (newUser != null)
            {
                _logger.LogWarning("User with new email : {email} found in the system.", model.NewEmail);
                throw new ForbiddenException($"This new email address {model.NewEmail} already exist in another account.");
            }
            var isUserAuthenticated = VerifyPassword(user.HashPassword, model.Password);
            if (!isUserAuthenticated)
            {
                _logger.LogWarning("User with id : {userId} password not matched for email change.", user.Id);
                throw new ForbiddenException("User password not matched.");
            }
            var changeEmailToken = GenerateResendAndChangeEmailToken(model.OldEmail, model.NewEmail, _changeEmailEncryptionKey, _changeEmailTokenExpiry);
            var resendToken = GenerateResendAndChangeEmailToken(model.OldEmail, model.NewEmail, _resendChangeEmailEncryptionKey, _resendChangeEmailTokenExpiry);
            await _emailService.SendChangePasswordMailAsync(model.NewEmail, user.FirstName, changeEmailToken, _changeEmailTokenExpiry).ConfigureAwait(false);
            return new ChangeEmailResponseModel() { ResendToken = resendToken };
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
                throw new ForbiddenException("Old email or new email cannot be null or empty.");
            }
            var user = await GetUserByEmailAsync(oldEmail).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with email : {email} not found.", oldEmail);
                throw new ForbiddenException($"User not found with email : {newEmail}.");
            }
            var changeEmailToken = GenerateResendAndChangeEmailToken(oldEmail, newEmail, _changeEmailEncryptionKey, _changeEmailTokenExpiry);
            var resendToken = GenerateResendAndChangeEmailToken(oldEmail, newEmail, _resendChangeEmailEncryptionKey, _resendChangeEmailTokenExpiry);
            await _emailService.SendChangePasswordMailAsync(newEmail, user.FirstName, changeEmailToken, _changeEmailTokenExpiry).ConfigureAwait(false);
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
                throw new ForbiddenException("Old email or new email cannot be null or empty.");
            }
            var user = await GetUserByEmailAsync(oldEmail).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with email : {email} not found.", oldEmail);
                throw new ForbiddenException($"User not found with email : {newEmail}.");
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
                    throw new ForbiddenException("Change email token has already expired. Please resend change email request.");
                }
                return (oldEmail, newEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to verify change email token.");
                if (ex is SecurityTokenInvalidSignatureException)
                {
                    throw new ForbiddenException("Token signature not properly formatted.");
                }
                if (ex is SecurityTokenExpiredException)
                {
                    throw new ForbiddenException("Token expired.");
                }
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to verify change email token.");
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
            if (criteria.IsActive != null)
            {
                predicate.And(p => p.IsActive == criteria.IsActive);
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
                predicate: p => p.Id != entity.Id && p.Email.ToLower() == entity.Email.ToLower()).ConfigureAwait(false);
            if (checkDuplicateEmail)
            {
                _logger.LogWarning("Duplicate user email : {email} is found.", entity.Email);
                throw new ServiceException("Duplicate email is found.");
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
            var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                predicate: p => p.Id == userId,
                include: src => src.Include(x => x.Department)
                ).ConfigureAwait(false);

            var userCertificates = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(
                predicate: p => p.UserId == userId && p.HasCertificateIssued.HasValue && p.HasCertificateIssued.Value,
                include: src => src.Include(x => x.Course)
                ).ConfigureAwait(false);

            var response = new UserResponseModel(user);
            foreach (var item in userCertificates)
            {
                response.Certificates.Add(new CourseCertificateIssuedResponseModel
                {
                    CourseId = item.CourseId,
                    CourseName = item.Course.Name,
                    CourseSlug = item.Course.Slug,
                    HasCertificateIssued = item.HasCertificateIssued,
                    CertificateIssuedDate = item.CertificateIssuedDate,
                    CertificateUrl = item.CertificateUrl,
                });
            }
            return response;
        }
    }
}