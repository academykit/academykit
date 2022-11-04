namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;

    public class DepartmentService : BaseGenericService<Department, BaseSearchCriteria>, IDepartmentService
    {
        public DepartmentService(
            IUnitOfWork unitOfWork,
            ILogger<DepartmentService> logger) : base(unitOfWork, logger)
        {

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
    }
}
