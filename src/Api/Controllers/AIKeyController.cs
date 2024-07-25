namespace Lingtren.Api.Controllers
{
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Microsoft.AspNetCore.Mvc;

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
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

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

            IsSuperAdmin(CurrentUser.Role);
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
                    AiModel = model.AiModel
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
                existing.AiModel = model.AiModel;

                var savedEntity = await aiKeyService
                    .UpdateAsync(existing, false)
                    .ConfigureAwait(false);
                return new AiKeyResponseModel(savedEntity);
            }
        }
    }
}
