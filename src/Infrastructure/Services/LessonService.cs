using System.Runtime.CompilerServices;
namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Domain.Entities;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;

    public class LessonService : BaseGenericService<Lesson, BaseSearchCriteria>, ILessonService
    {
        public LessonService(IUnitOfWork unitOfWork, ILogger<LessonService> logger) : base(unitOfWork, logger)
        {
        }
    }
}