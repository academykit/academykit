namespace Lingtren.Infrastructure.Services
{
    using System;
    using System.Threading.Tasks;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Domain.Common;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public abstract class BaseService
    {
        /// <summary>
        /// The unit of work.
        /// </summary>
        protected readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="logger">The logger</param>
        protected BaseService(IUnitOfWork unitOfWork, ILogger logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        protected TResult ExecuteWithResult<TResult>(Func<TResult> delegateFunc)
        {
            try
            {
                return delegateFunc();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        protected async Task<TResult> ExecuteWithResultAsync<TResult>(Func<Task<TResult>> delegateFunc)
        {
            try
            {
                return await delegateFunc().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        protected async Task Execute(Func<Task> delegateFunc)
        {
            try
            {
                await delegateFunc().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        protected async Task ExecuteAsync(Func<Task> delegateFunc)
        {
            try
            {
                await delegateFunc().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Check that entity is not <c>null</c> and tries to retrieve its updated value from the database context.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="context">The database context.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="argumentName">The name of the argument being validated.</param>
        /// <param name="required">Determines whether entity should not be null.</param>
        /// <returns>The updated entity from the database context.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="entity"/> is null and <paramref name="required"/> is True.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If entity with given Id or Name (for lookup entity with Id=0) was not found.
        /// </exception>
        /// <remarks>All other exceptions will be propagated to caller method.</remarks>
        protected TEntity ResolveChildEntity<TEntity>(
            TEntity entity, string argumentName, bool required = false)
            where TEntity : IdentifiableEntity
        {
            if (entity == null)
            {
                if (!required)
                {
                    return null;
                }

                argumentName = typeof(TEntity).Name + "." + argumentName;
                throw new ArgumentException($"{argumentName} cannot be null.", argumentName);
            }

            TEntity child = _unitOfWork.GetRepository<TEntity>().GetFirstOrDefault(predicate: e => e.Id == entity.Id);

            if (child == null)
            {
                throw new ServiceException(
                    $"Child entity {typeof(TEntity).Name} with Id={entity.Id} was not found.");
            }

            return child;
        }
    }
}
