namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class ZoomLicenseController : BaseApiController
    {
        private readonly IZoomLicenseService zoomLicenseService;
        private readonly IValidator<ZoomLicenseRequestModel> validator;
        private readonly IValidator<LiveClassLicenseRequestModel> validator1;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public ZoomLicenseController(
            IZoomLicenseService zoomLicenseService,
            IValidator<LiveClassLicenseRequestModel> validator1,
            IValidator<ZoomLicenseRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.zoomLicenseService = zoomLicenseService;
            this.validator = validator;
            this.validator1 = validator1;
            this.localizer = localizer;
        }

        /// <summary>
        /// get zoomLicense api.
        /// </summary>
        /// <returns> the list of <see cref="ZoomLicenseResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<ZoomLicenseResponseModel>> SearchAsync(
            [FromQuery] ZoomLicenseBaseSearchCriteria searchCriteria
        )
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            var searchResult = await zoomLicenseService
                .SearchAsync(searchCriteria)
                .ConfigureAwait(false);

            var response = new SearchResult<ZoomLicenseResponseModel>
            {
                Items = new List<ZoomLicenseResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p => response.Items.Add(new ZoomLicenseResponseModel(p)));
            return response;
        }

        /// <summary>
        /// get user by id.
        /// </summary>
        /// <param name="id"> the user id. </param>
        /// <returns> the instance of <see cref="ZoomLicenseResponseModel" /> .</returns>
        [HttpGet("{id}")]
        public async Task<ZoomLicenseResponseModel> Get(Guid id)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            var model = await zoomLicenseService.GetAsync(id).ConfigureAwait(false);
            return new ZoomLicenseResponseModel(model);
        }

        /// <summary>
        /// create zoomLicense api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="ZoomLicenseRequestModel" />. </param>
        /// <returns> the instance of <see cref="ZoomLicenseRequestModel" /> .</returns>
        [HttpPost]
        public async Task<ZoomLicenseResponseModel> CreateAsync(ZoomLicenseRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var response = await zoomLicenseService
                .CreateZoomLicenseAsync(model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new ZoomLicenseResponseModel(response);
        }

        ///
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns><summary>
        /// <summary>
        /// update zoomLicense api
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="model"> the instance of <see cref="ZoomLicenseRequestModel" />. </param>
        /// <returns> the instance of <see cref="ZoomLicenseResponseModel" /> .</returns>
        [HttpPut("{id}")]
        public async Task<ZoomLicenseResponseModel> UpdateAsync(
            Guid id,
            ZoomLicenseRequestModel model
        )
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var response = await zoomLicenseService
                .UpdateZoomLicenseAsync(id, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new ZoomLicenseResponseModel(response);
        }

        /// <summary>
        /// delete zoomLicense api.
        /// </summary>
        /// <param name="id"> the zoom id.  </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await zoomLicenseService
                .DeleteAsync(id.ToString(), CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("ZoomLicense")
                }
            );
        }

        /// <summary>
        /// change zoomLicense status api.
        /// </summary>
        /// <param name="id">the zoomLicense id.</param>
        /// <param name="enabled">the boolean.</param>
        /// <returns>the instance of <see cref="ZoomLicenseResponseModel"/>.</returns>
        [HttpPatch("{id}/status")]
        public async Task<ZoomLicenseResponseModel> ChangeStatus(Guid id, [FromQuery] bool enabled)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            var existing = await zoomLicenseService
                .GetAsync(id, CurrentUser.Id)
                .ConfigureAwait(false);

            existing.Id = existing.Id;
            existing.IsActive = enabled;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = DateTime.UtcNow;

            var savedEntity = await zoomLicenseService.UpdateAsync(existing).ConfigureAwait(false);
            return new ZoomLicenseResponseModel(savedEntity);
        }

        /// <summary>
        /// Gets Active LicenseId.
        /// </summary>
        ///
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns><param name="zoomLicenseIdRequestModel">the instance of <see cref="LiveClassLicenseRequestModel"></param>.
        /// <returns>the instance of <see cref="ZoomLicenseResponseModel"/></returns>
        /// <exception cref="ForbiddenException"></exception>
        [HttpGet("Active")]
        public async Task<List<ZoomLicenseResponseModel>> Active(
            [FromQuery] LiveClassLicenseRequestModel model
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await validator1
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var zoomLicenses = await zoomLicenseService
                .GetActiveLicensesAsync(model)
                .ConfigureAwait(false);
            return zoomLicenses.ToList();
        }
    }
}
