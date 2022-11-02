namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;

    public class RefreshTokenService : BaseService, IRefreshTokenService
    {
        public RefreshTokenService(IUnitOfWork unitOfWork,
            ILogger<RefreshTokenService> logger) : base(unitOfWork, logger)
        {
        }
        public async Task<IList<RefreshToken>> GetByUserId(Guid userId)
        {
            try
            {
                return await _unitOfWork.GetRepository<RefreshToken>().GetAllAsync(predicate: p => p.UserId == userId && p.IsActive == true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to fetch refresh token by user id.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to fetch refresh token by user id.");
            }
        }
        public async Task<RefreshToken> GetByValue(string token)
        {
            try
            {
                return await _unitOfWork.GetRepository<RefreshToken>().GetFirstOrDefaultAsync(predicate: p => p.Token == token && p.IsActive == true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to fetch refresh token by value.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to fetch refresh token by value.");
            }
        }

        public async Task CreateAsync(RefreshToken token)
        {
            try
            {
                await _unitOfWork.GetRepository<RefreshToken>().InsertAsync(token).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to update refresh token.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to update refresh token.");
            }
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            try
            {
                _unitOfWork.GetRepository<RefreshToken>().Update(token);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to update refresh token.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to update refresh token.");
            }
        }
    }
}
