namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Common;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// This abstract class is a base for all <see cref="IGenericService{T,S}"/> service implementations. It
    /// provides CRUD, search and export functionality.
    /// </summary>
    ///
    /// <typeparam name="T">The type of the managed entities.</typeparam>
    /// <typeparam name="S">The type of the entities search criteria.</typeparam>
    public abstract class BaseGenericService<T, S> : BaseService, IGenericService<T, S>
        where T : IdentifiableEntity
        where S : BaseSearchCriteria
    {
        /// <summary>
        /// The cached name of the entity type.
        /// </summary>
        protected readonly string _entityName = typeof(T).Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGenericService{T,S}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="logger">The logger</param>
        /// <param name="localizer">The localization</param>
        protected BaseGenericService(IUnitOfWork unitOfWork, ILogger logger)
            : base(unitOfWork, logger)
        {
        }

        /// <summary>
        /// Creates given entity.
        /// </summary>
        ///
        /// <param name="entity">The entity to create.</param>
        /// <returns>The created entity.</returns>
        public async Task<T> CreateAsync(T entity, bool includeProperties = true)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                CommonHelper.ValidateArgumentNotNull(entity, nameof(entity));

                entity.Id = Guid.NewGuid();

                await CreatePreHookAsync(entity).ConfigureAwait(false);

                // get existing child entities from DB, otherwise new entities will be created in database
                await ResolveChildEntitiesAsync(entity).ConfigureAwait(false);

                var repo = _unitOfWork.GetRepository<T>();

                await repo.InsertAsync(entity).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                // load entity again to return all fields populated, because child entities may contain just Ids
                T entityRetrievedFromDb = await Get(entity.Id, includeProperties).ConfigureAwait(false);

                await CreatePostHookAsync(entityRetrievedFromDb).ConfigureAwait(false);

                return entityRetrievedFromDb;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates given entity.
        /// </summary>
        ///
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public async Task<T> UpdateAsync(T entity, bool includeProperties = true)
        {
            return await ExecuteWithResult(async () =>
            {
                CommonHelper.ValidateArgumentNotNull(entity, nameof(entity));

                T existing = await Get(entity.Id, includeProperties).ConfigureAwait(false);

                // get existing child entities from DB, otherwise new entities will be created in database
                await ResolveChildEntitiesAsync(entity).ConfigureAwait(false);

                // delete one-to-many children, so that they will be re-created
                DeleteChildEntities(existing);

                // copy fields to existing entity (attach approach doesn't work for child entities)
                await UpdateEntityFieldsAsync(existing, entity).ConfigureAwait(false);
                // _unitOfWork.DbContext.Entry(existing).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                // _unitOfWork.GetRepository<T>().Update(existing);

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                // load entity again to return all fields populated, because child entities may contain just Ids
                return await Get(entity.Id, includeProperties).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <returns>The retrieved entity.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is not positive.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public async Task<T> GetAsync(Guid id, Guid? currentUserId = null, bool includeProperties = true)
        {
            return await ExecuteWithResult(async () =>
            {
                T entity = await Get(id, includeProperties).ConfigureAwait(false);
                await PopulateRetrievedEntity(entity).ConfigureAwait(false);
                await CheckGetPermissionsAsync(entity, currentUserId).ConfigureAwait(false);
                return entity;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves entity with the given slug or id.
        /// </summary>
        ///
        /// <param name="identity">The slug of the entity to retrieve.</param>
        /// <returns>The retrieved entity.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="slug"/> is empty or null.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public async Task<T> GetByIdOrSlugAsync(string identity, Guid? currentUserId = null, bool includeProperties = true)
        {
            return await ExecuteWithResult(async () =>
            {
                CommonHelper.ValidateArgumentNotNullOrEmpty(identity, nameof(identity));
                T entity = await Get(PredicateForIdOrSlug(identity), includeProperties).ConfigureAwait(false);
                await PopulateRetrievedEntity(entity).ConfigureAwait(false);
                await CheckGetPermissionsAsync(entity, currentUserId).ConfigureAwait(false);
                return entity;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <returns>The retrieved entity.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is not positive.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public async Task<T> GetFirstOrDefaultAsync(Guid? currentUserId = null, bool includeProperties = true)
        {
            return await ExecuteWithResult(async () =>
            {
                T entity = await _unitOfWork.GetRepository<T>().GetFirstOrDefaultAsync(
                    include: includeProperties ? IncludeNavigationProperties : null).ConfigureAwait(false);
                await PopulateRetrievedEntity(entity).ConfigureAwait(false);
                await CheckGetPermissionsAsync(entity, currentUserId).ConfigureAwait(false);
                return entity;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes entity with the given Id or Slug.
        /// </summary>
        ///
        /// <param name="identity">The id or slug of the entity to delete.</param>
        /// <param name="currentUserId"> The current user Id.</param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="identity"/> is not positive.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public virtual async Task DeleteAsync(string identity, Guid currentUserId)
        {
            await ExecuteAsync(async () =>
            {
                T entity = await GetByIdOrSlugAsync(identity, currentUserId, false).ConfigureAwait(false);
                await CheckDeletePermissionsAsync(entity, currentUserId).ConfigureAwait(false);
                _unitOfWork.GetRepository<T>().Delete(entity);
                _unitOfWork.SaveChanges();
            });
        }

        /// <summary>
        /// Retrieves entities matching given search criteria.
        /// </summary>
        ///
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The matched entities.</returns>
        ///
        /// <exception cref="ArgumentNullException">If the <paramref name="criteria"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the <paramref name="criteria"/> is incorrect,
        /// e.g. PageNumber is negative, or PageNumber is positive and PageSize is not positive.</exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public async Task<SearchResult<T>> SearchAsync(S criteria, bool includeProperties = true)
        {
            return await ExecuteWithResultAsync<SearchResult<T>>(async () =>
            {
                CommonHelper.CheckSearchCriteria(criteria);
                await SearchPreHookAsync(criteria).ConfigureAwait(false);

                var predicate = PredicateBuilder.New<T>(true);
                // construct query conditions
                predicate = ConstructQueryConditions(predicate, criteria);

                // execute query and set result properties
                var query = _unitOfWork.GetRepository<T>().GetAll(predicate: predicate, include: includeProperties ? IncludeNavigationProperties : null);

                // construct SortBy property selector expression

                if (criteria.SortType == SortType.Popular ||
                    criteria.SortType == SortType.Trending)
                {
                    query = ApplySortByPopularOrTrending(query, criteria.SortType, criteria.SortBy);
                }
                else
                {
                    if (criteria.SortBy == null)
                    {
                        SetDefaultSortOption(criteria);
                    }
                    query = criteria.SortType == SortType.Ascending
                        ? query.OrderBy(criteria.SortBy)
                        : query.OrderByDescending(criteria.SortBy);
                }

                var result = query.ToPagedList(criteria.Page, criteria.Size);

                return result;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected virtual Expression<Func<T, bool>> PredicateForIdOrSlug(string identity)
        {
            throw new ServiceException($"The {_entityName} does not support get by slug or slug");
        }

        /// <summary>
        /// Check if entity could be accessed by current user
        /// </summary>
        /// <param name="entityToReturn">The entity being returned</param>
        protected virtual async Task CheckGetPermissionsAsync(T entityToReturn, Guid? CurrentUserId = null)
        {
            await Task.FromResult(0);
        }

        /// <summary>
        /// Check if entity could be deleted
        /// </summary>
        /// <param name="entityToDelete">The entity being deleted</param>
        protected virtual async Task CheckDeletePermissionsAsync(T entityToDelete, Guid CurrentUserId)
        {
            await Task.FromResult(0);
        }

        /// <summary>
        /// This pre hook of search method should be overridden by child classes to validate additional conditions to the search query.
        /// Should throw exception if validation fails.
        /// </summary>
        /// <param name="criteria">The search criteria</param>
        protected virtual async Task SearchPreHookAsync(S criteria)
        {
            // by default do nothing
            await Task.FromResult(0);
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        protected virtual void SetDefaultSortOption(S criteria)
        {
            // if not overridden, use Id ASC by default
            criteria.SortBy = nameof(IdentifiableEntity.Id);
            criteria.SortType = SortType.Ascending;
        }

        /// <summary>
        /// Applies order by on a given popularity or trending.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="sortType">The sort type.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <exception cref="ArgumentException">
        /// If method is executed and is not overridden in child service.
        /// </exception>
        /// <remarks>
        /// It should be overridden in child services to apply order by popularity or trending properties.
        /// </remarks>
        protected virtual IQueryable<T> ApplySortByPopularOrTrending(IQueryable<T> query, SortType sortType, string sortColumn)
        {
            // if method is not overridden in child class, consider it doesn't support sort by Health
            throw new ArgumentException(
                $"SortType={sortType} is not supported on {typeof(T).Name}.{sortColumn}.");
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected virtual async Task CreatePreHookAsync(T entity)
        {
            // do nothing by default
            await Task.FromResult(0);
        }

        /// <summary>
        /// This is called after entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates after entity is saved.
        /// </remarks>
        protected virtual async Task CreatePostHookAsync(T entityRetrived)
        {
            // do nothing by default
            await Task.FromResult(0);
        }

        /// <summary>
        /// Populates the retrieved entity.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to populate extra properties.
        /// </remarks>
        protected virtual async Task PopulateRetrievedEntity(T entity)
        {
            // do nothing by default
            await Task.FromResult(0);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected virtual Expression<Func<T, bool>> ConstructQueryConditions(Expression<Func<T, bool>> predicate, S criteria)
        {
            return predicate;
        }

        /// <summary>
        /// Deletes the child entities from the database context.
        /// </summary>
        ///
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        ///
        /// <param name="entity">The entity to delete children for.</param>
        protected virtual void DeleteChildEntities(T entity)
        {
            // do nothing by default
        }

        /// <summary>
        /// Updates the child entities by loading them from the database context.
        /// </summary>
        ///
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        ///
        /// <param name="context">The database context.</param>
        protected virtual async Task ResolveChildEntitiesAsync(T entity)
        {
            // do nothing by default
            await Task.FromResult(0);
        }

        /// <summary>
        /// Updates the <paramref name="existing"/> entity according to <paramref name="newEntity"/> entity.
        /// </summary>
        /// <remarks>Override in child services to update navigation properties.</remarks>
        /// <param name="existing">The existing entity.</param>
        /// <param name="newEntity">The new entity.</param>
        protected virtual async Task UpdateEntityFieldsAsync(T existing, T newEntity)
        {
            _unitOfWork.DbContext.Entry(existing).CurrentValues.SetValues(newEntity);
            _unitOfWork.GetRepository<T>().Update(existing);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected virtual IIncludableQueryable<T, object> IncludeNavigationProperties(IQueryable<T> query)
        {
            return null;
        }

        /// <summary>
        /// Retrieves entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <param name="full">Determines whether navigation properties should also be loaded.</param>
        /// <returns>The retrieved entity.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is not positive.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        private async Task<T> Get(Guid id, bool full)
        {
            CommonHelper.ValidateArgumentGuid(id, nameof(id));
            T entity = await _unitOfWork.GetRepository<T>().GetFirstOrDefaultAsync(predicate: e => e.Id == id,
            include: full ? IncludeNavigationProperties : null).ConfigureAwait(false);
            CommonHelper.CheckFoundEntity(entity, id);
            await PopulateRetrievedEntity(entity).ConfigureAwait(false);
            return entity;
        }

        /// <summary>
        /// Retrieves entity with the given predicate.
        /// </summary>
        ///
        /// <param name="predicate">The predicate of the entity to retrieve.</param>
        /// <param name="full">Determines whether navigation properties should also be loaded.</param>
        /// <returns>The retrieved entity.</returns>
        ///
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given slug doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        protected virtual async Task<T> Get(Expression<Func<T, bool>> predicate, bool full)
        {
            T entity = await _unitOfWork.GetRepository<T>().GetFirstOrDefaultAsync(predicate: predicate,
            include: full ? IncludeNavigationProperties : null).ConfigureAwait(false);
            CommonHelper.CheckFoundEntity(entity);
            return entity;
        }
    }
}
