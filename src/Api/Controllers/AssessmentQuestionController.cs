namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class AssessmentQuestionController : BaseApiController
    {
        private readonly IAssessmentQuestionService assessmentQuestionService;
        private readonly IAssessmentService assessmentService;
        private readonly IValidator<AssessmentQuestionRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public AssessmentQuestionController(
            IAssessmentQuestionService assessmentQuestionService,
            IValidator<AssessmentQuestionRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IAssessmentService assessmentService
        )
        {
            this.assessmentQuestionService = assessmentQuestionService;
            this.validator = validator;
            this.localizer = localizer;
            this.assessmentService = assessmentService;
        }

        /// <summary>
        /// Search the questions.
        /// </summary>
        /// <param name="searchCriteria">The question search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        public async Task<SearchResult<AssessmentQuestionResponseModel>> SearchAsync(
            string identity,
            [FromQuery] AssessmentQuestionBaseSearchCriteria searchCriteria
        )
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(identity, nameof(identity));
            searchCriteria.CurrentUserId = CurrentUser.Id;
            var existingAssessment = await assessmentService
                .GetByIdOrSlugAsync(identity, CurrentUser.Id, false)
                .ConfigureAwait(false);
            searchCriteria.AssessmentIdentity = existingAssessment.Id;
            searchCriteria.SortBy = "Order";
            var searchResult = await assessmentQuestionService
                .SearchAsync(searchCriteria, true)
                .ConfigureAwait(false);

            var response = new SearchResult<AssessmentQuestionResponseModel>
            {
                Items = new List<AssessmentQuestionResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                response.Items.Add(new AssessmentQuestionResponseModel(p))
            );

            return response;
        }

        /// <summary>
        /// create Feedback api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="AssessmentQuestionRequestModel" />. </param>
        /// <returns> the instance of <see cref="AssessmentQuestionResponseModel" /> .</returns>
        [HttpPost("{identity}")]
        public async Task<AssessmentQuestionResponseModel> CreateAsync(
            string identity,
            AssessmentQuestionRequestModel model
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            var existingAssessment = await assessmentService
                .GetByIdOrSlugAsync(identity)
                .ConfigureAwait(false);

            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new AssessmentQuestion
            {
                Id = Guid.NewGuid(),
                Name = model.QuestionName,
                AssessmentId = existingAssessment.Id,
                Type = model.Type,
                IsActive = true,
                Hints = model.Hints,
                Description = model.Description,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                AssessmentOptions = new List<AssessmentOptions>(),
            };
            if (
                model.Type == AssessmentTypeEnum.SingleChoice
                || model.Type == AssessmentTypeEnum.MultipleChoice
            )
            {
                foreach (
                    var item in model.assessmentQuestionOptions.Select(
                        (answer, i) => new { i, answer }
                    )
                )
                {
                    entity.AssessmentOptions.Add(
                        new AssessmentOptions
                        {
                            Id = Guid.NewGuid(),
                            AssessmentQuestionId = entity.Id,
                            Order = item.i + 1,
                            Option = item.answer.Option,
                            IsCorrect = item.answer.IsCorrect,
                            CreatedBy = CurrentUser.Id,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = CurrentUser.Id,
                            UpdatedOn = currentTimeStamp,
                        }
                    );
                }
            }

            var response = await assessmentQuestionService
                .CreateAsync(entity)
                .ConfigureAwait(false);
            return new AssessmentQuestionResponseModel(response);
        }

        /// <summary>
        /// get Feedback by id or slug.
        /// </summary>
        /// <param name="identity"> the Feedback id or slug.</param>
        /// <returns> the instance of <see cref="AssessmentQuestionResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<AssessmentQuestionResponseModel> Get(string identity)
        {
            var model = await assessmentQuestionService
                .GetByIdOrSlugAsync(identity)
                .ConfigureAwait(false);
            return new AssessmentQuestionResponseModel(model);
        }

        /// <summary>
        /// update Feedback api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="AssessmentQuestionRequestModel" />. </param>
        /// <returns> the instance of <see cref="AssessmentQuestionResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<AssessmentQuestionResponseModel> UpdateAsync(
            string identity,
            AssessmentQuestionRequestModel model
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var savedEntity = await assessmentQuestionService
                .UpdateAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new AssessmentQuestionResponseModel(savedEntity);
        }

        /// <summary>
        /// delete Feedback api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await assessmentQuestionService
                .DeleteAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AssessmentRemoved")
                }
            );
        }

        /// <summary>
        /// delete Feedback api.
        /// </summary>
        /// <param name="identity"> id or slug of Assessment. </param>
        /// <returns> the task complete. </returns>
        [HttpGet("{identity}/ExamQuestion")]
        public async Task<AssessmentExamResponseModel> GetExamQuestion(string identity)
        {
            var existingAssessment = await assessmentService
                .GetByIdOrSlugAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
            var GetExamQuestion = await assessmentQuestionService
                .GetExamQuestion(existingAssessment, CurrentUser.Id)
                .ConfigureAwait(false);
            return GetExamQuestion;
        }
    }
}
