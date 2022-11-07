namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;

    public class CourseService : BaseGenericService<Course, BaseSearchCriteria>, ICourseService
    {
        public CourseService(IUnitOfWork unitOfWork, ILogger<CourseService> logger) : base(unitOfWork, logger)
        {
        }
    }
}