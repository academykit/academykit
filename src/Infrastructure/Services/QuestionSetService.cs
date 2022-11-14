namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Microsoft.Extensions.Logging;

    public class QuestionSetService : BaseGenericService<QuestionSet, BaseSearchCriteria>, IQuestionSetService
    {
        public QuestionSetService(
            IUnitOfWork unitOfWork,
            ILogger<QuestionSetService> logger) : base(unitOfWork, logger)
        {

        }
    }
}
