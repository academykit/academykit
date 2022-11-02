namespace Lingtren.Infrastructure.Services
{
    using Domain.Entities;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Configurations;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.Data;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using System.Text;

    public class UserService : BaseService, IUserService
    {
        private readonly JWT _jwt;

        public UserService(IUnitOfWork unitOfWork,
            ILogger<UserService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IOptions<JWT> jwt) : base(unitOfWork, logger, localizer)
        {
            _jwt = jwt.Value;
        }

        #region Private Methods
        private Expression<Func<User, bool>> ConstructQueryConditions(Expression<Func<User, bool>> predicate, UserSearchCriteria criteria)
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
        private void SetDefaultSortOption(UserSearchCriteria criteria)
        {
            criteria.SortBy = nameof(User.CreatedOn);
            criteria.SortType = SortType.Descending;
        }
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
        private async Task<IEnumerable<RefreshToken>> GetUserRefreshTokenList(string username)
        {
            //var tokens = await _userRefreshTokenRepository.GetByUserNameAsync(username);
            //return tokens;
            return null;
        }
        private async Task<RefreshToken> GetUserRefreshToken(string token)
        {
            // return await _userRefreshTokenRepository.GetByTokenValueAsync(token);
            return null;

        }
        private async Task<User> GetUserFromRefreshToken(string token)
        {
            var userRefreshToken = await GetUserRefreshToken(token);
            //if (userRefreshToken != null)
            //{
            //    var user = await _userRepository.GetByIdAsync(userRefreshToken.UserId);
            //    return user;
            //}
            return null;
        }
        private async Task<RefreshToken> CreateRefreshToken()
        {
            var currentTimeStamp = DateTime.UtcNow;
            //return new RefreshToken
            //{
            //    Token = await GetUniqueRefreshToken().ConfigureAwait(false),
            //    Expires = currentTimeStamp.AddMinutes(_refreshTokenDuration),
            //    CreatedOn = currentTimeStamp,
            //};
            return null;
        }
        private async Task<string> GetUniqueRefreshToken()
        {
            //var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            //var refreshToken = await _userRefreshTokenRepository.GetByTokenValueAsync(token).ConfigureAwait(false);
            //if (refreshToken == null)
            //{
            //    return token;
            //}
            //return await GetUniqueRefreshToken().ConfigureAwait(false);
            return null;
        }


        #endregion Private Methods

        /// <summary>
        /// Handle to search users
        /// </summary>
        /// <param name="searchCriteria"> the instance of <see cref="UserSearchCriteria" /> </param>
        /// <returns> the instance of <see cref="SearchResult{User}" /> .</returns>
        public async Task<SearchResult<User>> SearchUserAsync(UserSearchCriteria searchCriteria)
        {
            try
            {
                var predicate = PredicateBuilder.New<User>(true);
                predicate = ConstructQueryConditions(predicate, searchCriteria);
                var query = _unitOfWork.GetRepository<User>().GetAll(predicate: predicate, include: null);

                if (searchCriteria.SortBy == null)
                {
                    SetDefaultSortOption(searchCriteria);
                }
                query = searchCriteria.SortType == SortType.Ascending
                    ? query.OrderBy(searchCriteria.SortBy)
                    : query.OrderByDescending(searchCriteria.SortBy);

                return await query.ToPagedListAsync(searchCriteria.Page, searchCriteria.Size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to search user.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to search user.");
            }
        }

        /// <summary>
        /// Handle to get user detail by id
        /// </summary>
        /// <param name="id">the user id</param>
        /// <returns>the instance of <see cref="User"/></returns>
        public async Task<User> GetUserAsync(Guid id)
        {
            try
            {
                return await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: p => p.Id == id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to fetch single user.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to fetch single user.");
            }
        }

        /// <summary>
        /// Handle to create user
        /// </summary>
        /// <param name="entity">the instance of <see cref="User"/></param>
        /// <returns>the instance of <see cref="User"/></returns>
        public async Task<User> CreateUserAsync(User entity)
        {
            try
            {
                await _unitOfWork.GetRepository<User>().InsertAsync(entity).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to create user.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to create user");
            }
        }

        /// <summary>
        /// Handle to update user information
        /// </summary>
        /// <param name="entity">the instance of <see cref="User"</param>
        /// <returns>the instance of <see cref="User"/></returns>
        public async Task<User> UpdateUserAsync(User entity)
        {
            try
            {
                _unitOfWork.GetRepository<User>().Update(entity);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to update user.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to update user");
            }
        }

    }
}

