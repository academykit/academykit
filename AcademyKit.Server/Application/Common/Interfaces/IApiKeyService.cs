namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Domain.Entities;

    public interface IApiKeyService : IGenericService<ApiKey, BaseSearchCriteria>
    {
        public Task<ApiKey> GetByKeyFirstOrDefaultAsync(string key);
    }
}
