namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;

    public class QuestionSetController : BaseApiController
    {
        private readonly IQuestionSetService _questionSetService;
        public QuestionSetController(IQuestionSetService questionSetService)
        {
            _questionSetService = questionSetService;
        }
    }
}
