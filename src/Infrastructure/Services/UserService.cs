namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class UserService : BaseService, IUserService
    {

        public UserService(IUnitOfWork unitOfWork,
            ILogger<UserService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {
        }
    }
}

