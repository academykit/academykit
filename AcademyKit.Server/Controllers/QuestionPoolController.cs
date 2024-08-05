﻿namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Api.Common;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Localization;
    using FluentValidation;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class QuestionPoolController : BaseApiController
    {
        private readonly IQuestionPoolService questionPoolService;
        private readonly IValidator<QuestionPoolRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public QuestionPoolController(
            IQuestionPoolService questionPoolService,
            IValidator<QuestionPoolRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.questionPoolService = questionPoolService;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// get QuestionPool api.
        /// </summary>
        /// <returns> the list of <see cref="QuestionPoolResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<QuestionPoolResponseModel>> SearchAsync(
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            searchCriteria.CurrentUserId = CurrentUser.Id;
            var searchResult = await questionPoolService
                .SearchAsync(searchCriteria)
                .ConfigureAwait(false);

            var response = new SearchResult<QuestionPoolResponseModel>
            {
                Items = new List<QuestionPoolResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p => response.Items.Add(new QuestionPoolResponseModel(p)));
            return response;
        }

        /// <summary>
        /// create QuestionPool api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="QuestionPoolRequestModel" />. </param>
        /// <returns> the instance of <see cref="QuestionPoolRequestModel" /> .</returns>
        [HttpPost]
        public async Task<QuestionPoolResponseModel> CreateAsync(QuestionPoolRequestModel model)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new QuestionPool
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                QuestionPoolTeachers = new List<QuestionPoolTeacher>(),
            };
            entity.QuestionPoolTeachers.Add(
                new QuestionPoolTeacher()
                {
                    Id = Guid.NewGuid(),
                    QuestionPoolId = entity.Id,
                    UserId = CurrentUser.Id,
                    Role = PoolRole.Creator,
                    CreatedOn = currentTimeStamp,
                    CreatedBy = CurrentUser.Id,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = CurrentUser.Id,
                }
            );
            var response = await questionPoolService.CreateAsync(entity).ConfigureAwait(false);
            return new QuestionPoolResponseModel(response);
        }

        /// <summary>
        /// get QuestionPool by id or slug.
        /// </summary>
        /// <param name="identity"> the QuestionPool id or slug.</param>
        /// <returns> the instance of <see cref="QuestionPoolResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<QuestionPoolResponseModel> Get(string identity)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            var model = await questionPoolService
                .GetByIdOrSlugAsync(identity)
                .ConfigureAwait(false);
            return new QuestionPoolResponseModel(model);
        }

        /// <summary>
        /// update QuestionPool api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="QuestionPoolRequestModel" />. </param>
        /// <returns> the instance of <see cref="QuestionPoolResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<QuestionPoolResponseModel> UpdateAsync(
            string identity,
            QuestionPoolRequestModel model
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var existing = await questionPoolService
                .GetByIdOrSlugAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await questionPoolService.UpdateAsync(existing).ConfigureAwait(false);
            return new QuestionPoolResponseModel(savedEntity);
        }

        /// <summary>
        /// delete QuestionPool api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await questionPoolService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("QuestionpoolRemoved")
                }
            );
        }

        /// <summary>
        /// reorder questionpool question.
        /// </summary>
        /// <param name="ids">Question ids.</param>
        /// <returns>task completed.</returns>
        [HttpPost("reorder")]
        public async Task<IActionResult> Reorder([FromQuery] string identity, IList<Guid> ids)
        {
            await questionPoolService.QuestionPoolQuestionReorderAsync(
                CurrentUser.Id,
                identity,
                ids
            );
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("QuestionpoolUpdatedSuccesfully")
                }
            );
        }
    }
}
