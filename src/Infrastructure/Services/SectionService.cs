namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;

    public class SectionService : BaseGenericService<Section, BaseSearchCriteria>, ISectionService
    {
        public SectionService(IUnitOfWork unitOfWork, ILogger<SectionService> logger) : base(unitOfWork, logger)
        {
        }
    }
}