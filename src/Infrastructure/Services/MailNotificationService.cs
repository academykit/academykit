using System.Data;
using System.Linq.Expressions;
using Humanizer;
using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Domain.Entities;
using Lingtren.Infrastructure.Common;
using Lingtren.Infrastructure.Localization;
using LinqKit;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Lingtren.Infrastructure.Services
{
    public class MailNotificationService
        : BaseGenericService<MailNotification, MailNotificationBaseSearchCriteria>,
            IMailNotificationService
    {
        public MailNotificationService(
            IUnitOfWork unitOfWork,
            ILogger<MailNotificationService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        #region Protected Methods
        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(MailNotification entity)
        {
            await CheckDuplicateMailNameAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the <paramref name="existing"/> entity according to <paramref name="newEntity"/> entity.
        /// </summary>
        /// <remarks>Override in child services to update navigation properties.</remarks>
        /// <param name="existing">The existing entity.</param>
        /// <param name="newEntity">The new entity.</param>
        protected override async Task UpdateEntityFieldsAsync(
            MailNotification existing,
            MailNotification newEntity
        )
        {
            await CheckDuplicateMailNameAsync(newEntity).ConfigureAwait(false);
            _unitOfWork.GetRepository<MailNotification>().Update(newEntity);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<MailNotification, bool>> PredicateForIdOrSlug(
            string identity
        )
        {
            return p => p.Id.ToString() == identity;
        }

        #endregion Protected Methods

        #region Private Methods
        /// <summary>
        /// Check duplicate name
        /// /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        private async Task CheckDuplicateMailNameAsync(MailNotification entity)
        {
            var duplicate = await _unitOfWork
                .GetRepository<MailNotification>()
                .GetFirstOrDefaultAsync(
                    selector: x => new { Name = x.Name, Subject = x.Subject },
                    predicate: p =>
                        p.Id != entity.Id && (p.Name == entity.Name || p.Subject == entity.Subject)
                );
            if (duplicate != null)
            {
                var duplicateProperties = "";
                var duplicateValues = "";

                if (duplicate.Subject == entity.Subject)
                {
                    duplicateProperties = nameof(entity.Subject).Humanize();
                    duplicateValues = entity.Subject;
                }

                if (duplicate.Name == entity.Name)
                {
                    if (!string.IsNullOrEmpty(duplicateProperties))
                    {
                        duplicateProperties += " and ";
                        duplicateValues += " and ";
                    }

                    duplicateProperties += nameof(entity.Name);
                    duplicateValues += entity.Name;
                }

                _logger.LogWarning(
                    "Duplicate Mail Notification ({properties}) with ({values}) are found for the Mail Notification.",
                    duplicateProperties,
                    duplicateValues
                );

                _logger.LogInformation(_localizer.GetString("DuplicateValuesError"));

                throw new ServiceException(
                    string.Format(_localizer.GetString("DuplicateValuesError"), duplicateProperties)
                );
            }
        }

        #endregion Private Methods

        /// <summary>
        /// /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<MailNotification, bool>> ConstructQueryConditions(
            Expression<Func<MailNotification, bool>> predicate,
            MailNotificationBaseSearchCriteria criteria
        )
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
            }

            if (criteria.IsActive.HasValue)
            {
                predicate = predicate.And(p => p.IsActive == criteria.IsActive);
            }

            if (criteria.MailType.HasValue)
            {
                predicate = predicate.And(p =>
                    p.MailType == criteria.MailType && p.IsActive == true
                );
            }

            return predicate;
        }
    }
}
