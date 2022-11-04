namespace Lingtren.Infrastructure.Services
{
    using Domain.Entities;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Configurations;
    using LinqKit;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    public class UserService : BaseGenericService<User, UserSearchCriteria>, IUserService
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JWT _jwt;

        public UserService(IUnitOfWork unitOfWork,
            ILogger<UserService> logger,
            IRefreshTokenService refreshTokenService,
            IOptions<JWT> jwt) : base(unitOfWork, logger)
        {
            _refreshTokenService = refreshTokenService;
            _jwt = jwt.Value;
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
            await _refreshTokenService.UpdateAsync(refreshToken);
            return true;
        }
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

        ///// <summary>
        ///// Handle to get user detail by id
        ///// </summary>
        ///// <param name="id">the user id</param>
        ///// <returns>the instance of <see cref="User"/></returns>
        //public async Task<User> GetUserAsync(Guid id)
        //{
        //    try
        //    {
        //        return await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Id == id).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while attempting to fetch single user.");
        //        throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to fetch single user.");
        //    }
        //}

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
        /// Handle to verify reset token
        /// </summary>
        /// <param name="model">the instance of <see cref="VerifyResetTokenModel"/></param>
        /// <returns>the password change token</returns>
        public async Task<string> VerifyPasswordResetTokenAsync(VerifyResetTokenModel model)
        {
            try
            {
                var user = await GetUserByEmailAsync(model.Email).ConfigureAwait(false);
                if (user == null)
                {
                    _logger.LogWarning("User not found with email: {email}.", model.Email);
                    throw new EntityNotFoundException("User Not Found.");
                }
                if (model.Token != user.PasswordResetToken)
                {
                    _logger.LogWarning("User not found with email: {email}.", model.Email);
                    throw new ForbiddenException("Reset Token Not Matched.");
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

        #endregion Account Services
        #region Protected Methods
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
                predicate = predicate.And(x => x.FirstName.ToLower().Trim().Contains(search)
                 || x.LastName.ToLower().Trim().Contains(search)
                 || x.MiddleName.ToLower().Trim().Contains(search)
                 || x.Email.ToLower().Trim().Contains(search));
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
              expires: DateTime.Now.AddMinutes(allowedMinutes),
              claims: claims,
              signingCredentials: signingCredentials);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        #endregion Private Methods

    }
}

