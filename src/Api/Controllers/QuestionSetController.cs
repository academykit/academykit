namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class QuestionSetController : BaseApiController
    {
        private readonly IQuestionSetService _questionSetService;
        public QuestionSetController(IQuestionSetService questionSetService)
        {
            _questionSetService = questionSetService;
        }

        /// <summary>
        /// update question pool
        /// </summary>
        /// <param name="identity"> the question id or  slug</param>
        /// <returns> the instance of <see cref="QuestionSetResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        [AllowAnonymous]
        public async Task<QuestionSetResponseModel> Get(string identity)
        {
            var model = await _questionSetService.GetByIdOrSlugAsync(identity, CurrentUser?.Id).ConfigureAwait(false);
            return new QuestionSetResponseModel(model);
        }

        [HttpPost("{identity}/AddQuestion")]
        public async Task<IActionResult> AddQuestion(string identity, QuestionSetAddQuestionRequestModel model)
        {
            if (model.QuestionPoolQuestionIds.Count == 0)
            {
                throw new ArgumentException("At least one question is required");
            }
            await _questionSetService.AddQuestionsAsync(identity, model, CurrentUser.Id);
            return Ok();
        }

    }
}
