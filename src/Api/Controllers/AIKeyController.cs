// <copyright file="GroupController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
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

        public AIKeyController(IAiKeyService aiKeyService)
        {
            this.aiKeyService = aiKeyService;
        }

        /// <summary>
        /// Search the groups.
        /// </summary>
        /// <param name="searchCriteria">The group search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        public async Task<AiKeyResponseModel> GetAiKey()
        {
            IsSuperAdmin(CurrentUser.Role);

            var model = await aiKeyService
                .GetFirstOrDefaultAsync(CurrentUser.Id, false)
                .ConfigureAwait(false);
            return new AiKeyResponseModel(model);
        }

        /// <summary>
        /// update group.
        /// </summary>
        /// <param name="groupId"> the group id.</param>
        /// <param name="model"> the  instance of <see cref="AiKeyRequestModel" /> .</param>
        /// <returns> the instance of <see cref="AiKeyResponseModel" /> .</returns>
        [HttpPut]
        public async Task<AiKeyResponseModel> UpdateGroup(AiKeyRequestModel model)
        {
            var currentTimeStamp = DateTime.UtcNow;

            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            var existing = await aiKeyService
                .GetFirstOrDefaultAsync(CurrentUser.Id, false)
                .ConfigureAwait(false);
            if (existing == null)
            {
                var entity = new AIKey()
                {
                    Id = Guid.NewGuid(),
                    Key = model.Key == null ? "" : model.Key,
                    IsActive = true,
                    CreatedBy = CurrentUser.Id,
                    CreatedOn = currentTimeStamp,
                };
                var response = await aiKeyService.CreateAsync(entity, false).ConfigureAwait(false);
                return new AiKeyResponseModel(response);
            }
            else
            {
                existing.Id = existing.Id;
                existing.Key = model.Key == null ? "" : model.Key;
                existing.IsActive = model.IsActive;
                existing.UpdatedBy = CurrentUser.Id;
                existing.UpdatedOn = currentTimeStamp;

                var savedEntity = await aiKeyService
                    .UpdateAsync(existing, false)
                    .ConfigureAwait(false);
                return new AiKeyResponseModel(savedEntity);
            }
        }
    }
}
