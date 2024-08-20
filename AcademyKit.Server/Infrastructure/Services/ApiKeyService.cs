namespace AcademyKit.Infrastructure.Services
{
    using System.Linq.Expressions;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class ApiKeyService : BaseGenericService<ApiKey, BaseSearchCriteria>, IApiKeyService
    {
        public ApiKeyService(
            IUnitOfWork unitOfWork,
            ILogger<ApiKeyService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        protected override Expression<Func<ApiKey, bool>> ConstructQueryConditions(
            Expression<Func<ApiKey, bool>> predicate,
            BaseSearchCriteria criteria
        )
        {
            if (criteria.CurrentUserId != Guid.Empty)
            {
                predicate = predicate.And(key => key.UserId == criteria.CurrentUserId);
            }
            return predicate;
        }

        protected override Expression<Func<ApiKey, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }

        public async Task<ApiKey> GetByKeyFirstOrDefaultAsync(string key)
        {
            return await ExecuteWithResult(async () =>
                {
                    var entity = await _unitOfWork
                        .GetRepository<ApiKey>()
                        .GetFirstOrDefaultAsync(predicate: (p) => p.Key == key)
                        .ConfigureAwait(false);
                    return entity;
                })
                .ConfigureAwait(false);
        }
    }
}
