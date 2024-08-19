namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Api.Common;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Helpers;
    using AcademyKit.Infrastructure.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class ApiKeyController : BaseApiController
    {
        private readonly IApiKeyService apiKeyService;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public ApiKeyController(
            IApiKeyService apiKeyService,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.apiKeyService = apiKeyService;
            this.localizer = localizer;
        }

        /// <summary>
        /// Search API Keys of current user.
        /// </summary>
        /// <returns>The search result.</returns>
        [HttpGet]
        public async Task<SearchResult<ApiKey>> GetKeys(
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            IsSuperAdmin(CurrentUser.Role);

            var model = await apiKeyService
                .SearchAsync(
                    new BaseSearchCriteria
                    {
                        Page = searchCriteria.Page,
                        Size = searchCriteria.Size,
                        CurrentUserId = CurrentUser.Id,
                    },
                    false
                )
                .ConfigureAwait(false);
            return model;
        }

        /// <summary>
        /// Create API key.
        /// </summary>
        /// <param name="payload">Request data<see cref="ApiKeyRequestModel"/> to create API key</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPost]
        public async Task<ApiKey> CreateApiKey(ApiKeyRequestModel payload)
        {
            IsSuperAdmin(CurrentUser.Role);
            CommonHelper.ValidateArgumentNotNullOrEmpty(payload.Name, nameof(payload.Name));
            var currentTimeStamp = DateTime.UtcNow;
            string key;
            do
            {
                key = await PasswordGenerator.GenerateStrongPassword(64);
            } while (await apiKeyService.GetByKeyFirstOrDefaultAsync(key) != null);
            var model = new ApiKey
            {
                Id = Guid.NewGuid(),
                Name = payload.Name,
                Key = key,
                UserId = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
            };
            await apiKeyService.CreateAsync(model, false).ConfigureAwait(false);
            return model;
        }

        /// <summary>
        /// Deletes the API key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await apiKeyService.DeleteAsync(id.ToString(), CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel
                {
                    Success = true,
                    Message = localizer.GetString("ApiKeyRemovedSuccessfully")
                }
            );
        }
    }
}
