namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Domain.Entities;
    public interface IRefreshTokenService
    {
        Task<IList<RefreshToken>> GetByUserId(Guid userId);
        Task<RefreshToken> GetByValue(string token);
        Task CreateAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
    }
}
