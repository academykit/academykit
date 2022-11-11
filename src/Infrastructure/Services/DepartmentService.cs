namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;

    public class DepartmentService : BaseGenericService<Department, DepartmentBaseSearchCriteria>, IDepartmentService
    {
        public DepartmentService(
            IUnitOfWork unitOfWork,
            ILogger<DepartmentService> logger) : base(unitOfWork, logger)
        {
        }
        #region Protected Methods
        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Department entity)
        {
            await CheckDuplicateDepartmentNameAsync(entity).ConfigureAwait(false);
            entity.Slug = CommonHelper.GetEntityTitleSlug<Department>(_unitOfWork, (slug) => q => q.Slug == slug, entity.Name);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Updates the <paramref name="existing"/> entity according to <paramref name="newEntity"/> entity.
        /// </summary>
        /// <remarks>Override in child services to update navigation properties.</remarks>
        /// <param name="existing">The existing entity.</param>
        /// <param name="newEntity">The new entity.</param>
        protected override async Task UpdateEntityFieldsAsync(Department existing, Department newEntity)
        {
            await CheckDuplicateDepartmentNameAsync(newEntity).ConfigureAwait(false);
            _unitOfWork.GetRepository<Department>().Update(newEntity);
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Department, bool>> ConstructQueryConditions(Expression<Func<Department, bool>> predicate, DepartmentBaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
            }
            if (criteria.IsActive != null)
            {
                predicate.And(p => p.IsActive == criteria.IsActive);
            }
            return predicate;
        }

        /// <summary>
        /// Sets the default sort column and order to given criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        protected override void SetDefaultSortOption(DepartmentBaseSearchCriteria criteria)
        {
            criteria.SortBy = nameof(Department.CreatedOn);
            criteria.SortType = SortType.Descending;
        }


        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Department, object> IncludeNavigationProperties(IQueryable<Department> query)
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Department, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }
        #endregion Protected Methods

        #region Private Methods
        /// <summary>
        /// Check duplicate name
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        private async Task CheckDuplicateDepartmentNameAsync(Department entity)
        {
            var departmentExist = await _unitOfWork.GetRepository<Department>().ExistsAsync(
                predicate: p => p.Id != entity.Id && p.Name.ToLower() == entity.Name.ToLower()).ConfigureAwait(false);
            if (departmentExist)
            {
                _logger.LogWarning("Duplicate department name : {name} is found for the department with id : {id}", entity.Name, entity.Id);
                throw new ServiceException("Duplicate department name is found");
            }
        }
        #endregion Private Methods
    }
}
