using System.Linq.Expressions;
using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Localization;
using LinqKit;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace AcademyKit.Infrastructure.Services
{
    public class LogsService : BaseService, ILogsService
    {
        public LogsService(
            IUnitOfWork unitOfWork,
            ILogger<LogsService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        /// <summary>
        /// Handle to get server logs async
        /// </summary>
        /// <param name="criteria"> the instance of <see cref="LogBaseSearchCriteria"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="ServerLogsResponseModel"/></returns>
        public async Task<SearchResult<ServerLogsResponseModel>> GetServerLogsAsync(
            LogBaseSearchCriteria criteria,
            Guid currentUserId
        )
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var hasAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                Expression<Func<Logs, bool>> predicate = PredicateBuilder.New<Logs>(true);
                if (!string.IsNullOrWhiteSpace(criteria.Search))
                {
                    var search = criteria.Search.ToLower().Trim();
                    predicate = predicate.And(x =>
                        x.Message.Contains(search)
                        || x.Exception.Contains(search)
                        || x.Logger.Contains(search)
                    );
                }

                if (criteria.StartDate.HasValue && !criteria.EndDate.HasValue)
                {
                    predicate = predicate.And(x => x.Logged.Date == criteria.StartDate.Value.Date);
                }

                if (criteria.StartDate.HasValue && criteria.EndDate.HasValue)
                {
                    //predicate = predicate.And(x => x.Logged.Date >= criteria.StartDate.Value.Date || x.Logged.Date =< criteria.EndDate.Value);
                }

                if (criteria.Severity.HasValue)
                {
                    var serverity = criteria.Severity.Value.ToString();
                    predicate = predicate.And(x => x.Level == serverity);
                }

                var response = await _unitOfWork
                    .GetRepository<Logs>()
                    .GetPagedListAsync(
                        criteria,
                        predicate: predicate,
                        selector: s => new ServerLogsResponseModel(s)
                    )
                    .ConfigureAwait(false);
                return response;
            });
        }

        /// <summary>
        /// Handle to get log details async
        /// </summary>
        /// <param name="logId"> the log id </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="ServerLogsResponseModel"/></returns>
        public async Task<ServerLogsResponseModel> GetLogDetailAsync(Guid logId, Guid currentUserId)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var hasAccess = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!hasAccess)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                return new ServerLogsResponseModel();
            });
        }
    }
}
