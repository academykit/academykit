namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Domain.Entities;
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Get refresh token by user id
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>the list of <see cref="RefreshToken"/> </returns>
        Task<IList<RefreshToken>> GetByUserId(Guid userId);

        /// <summary>
        /// Handle to fetch refresh token by value
        /// </summary>
        /// <param name="token">the refresh token</param>
        /// <returns>the instance of <see cref="RefreshToken"/></returns>
        Task<RefreshToken> GetByValue(string token);

        /// <summary>
        /// Handle to create refresh token
        /// </summary>
        /// <param name="token">the instance of <see cref="RefreshToken"/> </param>
        /// <returns></returns>
        Task CreateAsync(RefreshToken token);

        /// <summary>
        /// Handle to update refresh token
        /// </summary>
        /// <param name="token">the instance of <see cref="RefreshToken"/></param>
        /// <returns></returns>
        Task UpdateAsync(RefreshToken token);
    }
}
