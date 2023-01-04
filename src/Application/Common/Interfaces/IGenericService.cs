namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Common;

    /// <summary>
    /// This service interface defines methods to manage <typeparamref name="T"/> entities.
    /// </summary>
    ///
    /// <typeparam name="T">The type of the managed entities.</typeparam>
    /// <typeparam name="S">The type of the entities search criteria.</typeparam>
    public interface IGenericService<T, S>
        where T : IdentifiableEntity
        where S : BaseSearchCriteria
    {
        /// <summary>
        /// Creates given entity.
        /// </summary>
        ///
        /// <param name="entity">The entity to create.</param>
        /// <returns>The created entity.</returns>
        Task<T> CreateAsync(T entity, bool includeProperties = true);

        /// <summary>
        /// Updates given entity.
        /// </summary>
        ///
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync(T entity, bool includeProperties = true);

        /// <summary>
        /// Retrieves entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <param name="currentUserId">The id of the current user.</param>
        /// <returns>The retrieved entity.</returns>
        Task<T> GetAsync(Guid id, Guid? currentUserId = null, bool includeAllProperties = true);

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
        Task<T> GetFirstOrDefaultAsync(Guid? currentUserId = null, bool includeProperties = true);

        /// <summary>
        /// Retrieves entity with the given id or slug.
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
        Task<T> GetByIdOrSlugAsync(string identity, Guid? currentUserId = null, bool includeProperties = true);

        /// <summary>
        /// Deletes entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to delete.</param>
        /// <param name="currentUserId">The id of the current user.</param>
        Task DeleteAsync(string identity, Guid currentUserId);

        /// <summary>
        /// Retrieves entities matching given search criteria.
        /// </summary>
        ///
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The matched entities.</returns>
        Task<SearchResult<T>> SearchAsync(S criteria, bool includeAllProperties = true);
    }
}