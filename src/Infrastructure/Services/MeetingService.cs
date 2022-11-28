namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;

    public class MeetingService : BaseGenericService<Meeting, BaseSearchCriteria>, IMeetingService
    {
        public MeetingService(
            IUnitOfWork unitOfWork,
            ILogger<MeetingService> logger) : base(unitOfWork, logger)
        {

        }
    }
}
