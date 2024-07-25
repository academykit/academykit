// <copyright file="DepartmentController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class SkillsController : BaseApiController
    {
        private readonly ISkillService skillsServices;
        private readonly IValidator<SkillsRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public SkillsController(
            ISkillService skillsServices,
            IValidator<SkillsRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.skillsServices = skillsServices;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// get department api.
        /// </summary>
        /// <returns> the list of <see cref="SkillsResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<SkillsResponseModel>> SearchAsync(
            [FromQuery] SkillsBaseSearchCriteria searchCriteria
        )
        {
            var searchResult = await skillsServices
                .SearchAsync(searchCriteria, true)
                .ConfigureAwait(false);
            var response = new SearchResult<SkillsResponseModel>
            {
                Items = new List<SkillsResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p => response.Items.Add(new SkillsResponseModel(p)));
            return response;
        }

        /// <summary>
        /// create department api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="SkillsResponseModel" />. </param>
        /// <returns> the instance of <see cref="SkillsRequestModel" /> .</returns>
        [HttpPost]
        public async Task<SkillsResponseModel> CreateAsync(SkillsRequestModel model)
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new Skills
            {
                Id = Guid.NewGuid(),
                IsActive = model.IsActive,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                SkillName = model.SkillName,
                Description = model.Description,
            };
            var response = await skillsServices.CreateAsync(entity, false).ConfigureAwait(false);
            return new SkillsResponseModel(response);
        }

        /// <summary>
        /// update department api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="DepartmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="DepartmentResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<SkillsResponseModel> UpdateAsync(Guid identity, SkillsRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);

            var existing = await skillsServices
                .GetAsync(identity, CurrentUser.Id, false)
                .ConfigureAwait(false);

            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.SkillName = model.SkillName;
            existing.Description = model.Description;
            existing.IsActive = model.IsActive;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await skillsServices
                .UpdateAsync(existing, false)
                .ConfigureAwait(false);

            return new SkillsResponseModel(savedEntity);
        }

        /// <summary>
        /// delete skills api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await skillsServices.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("SkillRemoved")
                }
            );
        }
    }
}
