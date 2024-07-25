namespace Lingtren.Api.Controllers
{
    using ClosedXML.Excel;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/QuestionPool/{identity}/Question")]
    public class QuestionController : BaseApiController
    {
        private readonly IQuestionPoolService questionPoolService;
        private readonly IQuestionService questionService;
        private readonly IValidator<QuestionRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public QuestionController(
            IQuestionPoolService questionPoolService,
            IQuestionService questionService,
            IValidator<QuestionRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.questionPoolService = questionPoolService;
            this.questionService = questionService;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// Search the questions.
        /// </summary>
        /// <param name="searchCriteria">The question search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        public async Task<SearchResult<QuestionResponseModel>> SearchAsync(
            string identity,
            [FromQuery] QuestionBaseSearchCriteria searchCriteria
        )
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(identity, nameof(identity));
            searchCriteria.CurrentUserId = CurrentUser.Id;
            searchCriteria.PoolIdentity = identity;
            var searchResult = await questionService
                .SearchAsync(searchCriteria)
                .ConfigureAwait(false);

            var response = new SearchResult<QuestionResponseModel>
            {
                Items = new List<QuestionResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
            {
                var questionPoolQuestion = questionPoolService
                    .GetQuestionPoolQuestion(identity, p.Id)
                    .Result;
                response.Items.Add(
                    new QuestionResponseModel(p, questionPoolQuestionId: questionPoolQuestion?.Id)
                );
            });
            return response;
        }

        /// <summary>
        /// create question api.
        /// </summary>
        /// <param name="identity">the question id or slug.</param>
        /// <param name="model"> the instance of <see cref="QuestionRequestModel" />. </param>
        /// <returns> the instance of <see cref="QuestionResponseModel" /> .</returns>
        [HttpPost]
        public async Task<QuestionResponseModel> CreateAsync(
            string identity,
            QuestionRequestModel model
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            var questionPool =
                await questionPoolService
                    .GetByIdOrSlugAsync(identity, currentUserId: CurrentUser.Id)
                    .ConfigureAwait(false)
                ?? throw new EntityNotFoundException(localizer.GetString("QuestionPoolNotFound"));
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var response = await questionService
                .AddAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            var questionPoolQuestion = questionPoolService
                .GetQuestionPoolQuestion(identity, response.Id)
                .Result;

            return new QuestionResponseModel(
                response,
                questionPoolQuestionId: questionPoolQuestion?.Id
            );
        }

        /// <summary>
        /// get question api.
        /// </summary>
        /// <param name="identity"> the question id or slug.</param>
        /// <returns> the instance of <see cref="QuestionResponseModel" /> .</returns>
        [HttpGet("{id}")]
        public async Task<QuestionResponseModel> Get(string identity, Guid id)
        {
            var questionPool =
                await questionPoolService
                    .GetByIdOrSlugAsync(identity, currentUserId: CurrentUser.Id)
                    .ConfigureAwait(false)
                ?? throw new EntityNotFoundException(localizer.GetString("QuestionPoolNotFound"));
            var showCorrectAndHints = false;
            if (
                questionPool.CreatedBy == CurrentUser.Id
                || questionPool.QuestionPoolTeachers.Any(x => x.UserId == CurrentUser.Id)
            )
            {
                showCorrectAndHints = true;
            }

            var model = await questionService
                .GetByIdOrSlugAsync(id.ToString(), CurrentUser?.Id)
                .ConfigureAwait(false);
            return new QuestionResponseModel(
                model,
                showCorrectAnswer: showCorrectAndHints,
                showHints: showCorrectAndHints
            );
        }

        /// <summary>
        /// update question api.
        /// </summary>
        /// <param name="identity">the question id or slug. </param>
        /// <param name="model"> the instance of <see cref="QuestionRequestModel" />. </param>
        /// <returns> the instance of <see cref="QuestionResponseModel" /> .</returns>
        [HttpPut("{id}")]
        public async Task<QuestionResponseModel> UpdateAsync(
            string identity,
            Guid id,
            QuestionRequestModel model
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var savedEntity = await questionService
                .UpdateAsync(identity, id, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new QuestionResponseModel(savedEntity);
        }

        /// <summary>
        /// delete question api.
        /// </summary>
        /// <param name="identity">the question id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string identity, Guid id)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            _ =
                await questionPoolService
                    .GetByIdOrSlugAsync(identity, currentUserId: CurrentUser.Id)
                    .ConfigureAwait(false)
                ?? throw new EntityNotFoundException(localizer.GetString("QuestionPoolNotFound"));
            await questionService
                .DeleteQuestionAsync(poolIdentity: identity, questionId: id, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("QuestionRemovedSuccessfully")
                }
            );
        }
    }
}
