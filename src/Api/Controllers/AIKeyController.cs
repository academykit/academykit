// <copyright file="GroupController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class AIKeyController : BaseApiController
    {
        private readonly IAiKeyService aiKeyService;
        private readonly IValidator<AiKeyRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public AIKeyController(
            IAiKeyService aiKeyService,
            IValidator<AiKeyRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.aiKeyService = aiKeyService;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// Search the groups.
        /// </summary>
        /// <param name="searchCriteria">The group search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet("AiKey")]
        public async Task<AiKeyResponseModel> GetAiKey()
        {
            IsSuperAdmin(CurrentUser.Role);

            var model = await aiKeyService
                .GetFirstOrDefaultAsync(CurrentUser.Id, false)
                .ConfigureAwait(false);
            return new AiKeyResponseModel(model);
        }

        /// <summary>
        /// Ai Key create api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="AiKeyRequestModel" /> .</param>
        /// <returns> the instance of <see cref="AiKeyResponseModel" /> .</returns>
        [HttpPost]
        public async Task<AiKeyResponseModel> Create(AiKeyRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            var currentTimeStamp = DateTime.UtcNow;
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);

            var entity = new AIKey()
            {
                Id = Guid.NewGuid(),
                Key = model.Key,
                IsActive = true,
                CreatedBy = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
            };
            var response = await aiKeyService.CreateAsync(entity, false).ConfigureAwait(false);
            return new AiKeyResponseModel(response);
        }

        /// <summary>
        /// update group.
        /// </summary>
        /// <param name="groupId"> the group id.</param>
        /// <param name="model"> the  instance of <see cref="AiKeyRequestModel" /> .</param>
        /// <returns> the instance of <see cref="AiKeyResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<AiKeyResponseModel> UpdateGroup(string identity, AiKeyRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var existing = await aiKeyService
                .GetByIdOrSlugAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Key = model.Key;
            existing.IsActive = model.IsActive;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await aiKeyService.UpdateAsync(existing, false).ConfigureAwait(false);
            return new AiKeyResponseModel(savedEntity);
        }

        /// <summary>
        /// delete department api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await aiKeyService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AiKeyRemoved")
                }
            );
        }
    }
}
