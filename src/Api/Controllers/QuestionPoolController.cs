﻿namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class QuestionPoolController : BaseApiController
    {
        private readonly IQuestionPoolService _questionPoolService;
        private readonly IValidator<QuestionPoolRequestModel> _validator;
        public QuestionPoolController(
            IQuestionPoolService questionPoolService,
            IValidator<QuestionPoolRequestModel> validator)
        {
            _questionPoolService = questionPoolService;
            _validator = validator;
        }
        /// <summary>
        /// get QuestionPool api
        /// </summary>
        /// <returns> the list of <see cref="QuestionPoolResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<QuestionPoolResponseModel>> SearchAsync([FromQuery] BaseSearchCriteria searchCriteria)
        {
            var searchResult = await _questionPoolService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<QuestionPoolResponseModel>
            {
                Items = new List<QuestionPoolResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new QuestionPoolResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// create QuestionPool api
        /// </summary>
        /// <param name="model"> the instance of <see cref="QuestionPoolRequestModel" />. </param>
        /// <returns> the instance of <see cref="QuestionPoolRequestModel" /> .</returns>
        [HttpPost]
        public async Task<QuestionPoolResponseModel> CreateAsync(QuestionPoolRequestModel model)
        {
            IsAdmin(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new QuestionPool
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id
            };
            var response = await _questionPoolService.CreateAsync(entity).ConfigureAwait(false);
            return new QuestionPoolResponseModel(response);
        }

        /// <summary>
        /// get QuestionPool by id or slug
        /// </summary>
        /// <param name="identity"> the QuestionPool id or slug</param>
        /// <returns> the instance of <see cref="QuestionPoolResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<QuestionPoolResponseModel> Get(string identity)
        {
            var model = await _questionPoolService.GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            return new QuestionPoolResponseModel(model);
        }

        /// <summary>
        /// update QuestionPool api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="QuestionPoolRequestModel" />. </param>
        /// <returns> the instance of <see cref="QuestionPoolResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<QuestionPoolResponseModel> UpdateAsync(string identity, QuestionPoolRequestModel model)
        {
            IsAdmin(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _questionPoolService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _questionPoolService.UpdateAsync(existing).ConfigureAwait(false);
            return new QuestionPoolResponseModel(savedEntity);
        }

        /// <summary>
        /// delete QuestionPool api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeletAsync(string identity)
        {
            IsAdmin(CurrentUser.Role);

            await _questionPoolService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "QuestionPool removed successfully." });
        }
    }
}