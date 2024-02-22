namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class AiKeyService : BaseGenericService<AIKey, BaseSearchCriteria>, IAiKeyService
    {
        public AiKeyService(
            IUnitOfWork unitOfWork,
            ILogger<AiKeyService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }
    }
}
