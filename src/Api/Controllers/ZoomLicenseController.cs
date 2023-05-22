namespace Lingtren.Api.Controllers
{
    using Application.Common.Models.RequestModels;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class ZoomLicenseController : BaseApiController
    {
        private readonly IZoomLicenseService _zoomLicenseService;
        private readonly IValidator<ZoomLicenseRequestModel> _validator;
        private readonly IValidator<LiveClassLicenseRequestModel> _validator1; 
        public ZoomLicenseController(
            IZoomLicenseService zoomLicenseService,
            IValidator<LiveClassLicenseRequestModel> validator1,
            IValidator<ZoomLicenseRequestModel> validator)
        {
            _zoomLicenseService = zoomLicenseService;
            _validator = validator;
            _validator1 = validator1;
        }

        /// <summary>
        /// get zoomLicense api
        /// </summary>
        /// <returns> the list of <see cref="ZoomLicenseResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<ZoomLicenseResponseModel>> SearchAsync([FromQuery] ZoomLicenseBaseSearchCriteria searchCriteria)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            var searchResult = await _zoomLicenseService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<ZoomLicenseResponseModel>
            {
                Items = new List<ZoomLicenseResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new ZoomLicenseResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="id"> the user id </param>
        /// <returns> the instance of <see cref="ZoomLicenseResponseModel" /> .</returns>
        [HttpGet("{id}")]
        public async Task<ZoomLicenseResponseModel> Get(Guid id)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            var model = await _zoomLicenseService.GetAsync(id).ConfigureAwait(false);
            return new ZoomLicenseResponseModel(model);
        }

        /// <summary>
        /// create zoomLicense api
        /// </summary>
        /// <param name="model"> the instance of <see cref="ZoomLicenseRequestModel" />. </param>
        /// <returns> the instance of <see cref="ZoomLicenseRequestModel" /> .</returns>
        [HttpPost]
        public async Task<ZoomLicenseResponseModel> CreateAsync(ZoomLicenseRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new ZoomLicense
            {
                Id = Guid.NewGuid(),
                LicenseEmail = model.LicenseEmail,
                HostId = model.HostId,
                Capacity = model.Capacity,
                IsActive = true,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id
            };
            var response = await _zoomLicenseService.CreateAsync(entity).ConfigureAwait(false);
            return new ZoomLicenseResponseModel(response);
        }

        /// <summary>
        /// update zoomLicense api
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="model"> the instance of <see cref="ZoomLicenseRequestModel" />. </param>
        /// <returns> the instance of <see cref="ZoomLicenseResponseModel" /> .</returns>
        [HttpPut("{id}")]
        public async Task<ZoomLicenseResponseModel> UpdateAsync(Guid id, ZoomLicenseRequestModel model)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _zoomLicenseService.GetAsync(id, CurrentUser.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.LicenseEmail = model.LicenseEmail;
            existing.HostId = model.HostId;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _zoomLicenseService.UpdateAsync(existing).ConfigureAwait(false);
            return new ZoomLicenseResponseModel(savedEntity);
        }

        /// <summary>
        /// delete zoomLicense api
        /// </summary>
        /// <param name="id"> the zoom id  </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await _zoomLicenseService.DeleteAsync(id.ToString(), CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "ZoomLicense removed successfully." });
        }

        /// <summary>
        /// change zoomLicense status api
        /// </summary>
        /// <param name="id">the zoomLicense id</param>
        /// <param name="enabled">the boolean</param>
        /// <returns>the instance of <see cref="ZoomLicenseResponseModel"/></returns>
        [HttpPatch("{id}/status")]
        public async Task<ZoomLicenseResponseModel> ChangeStatus(Guid id, [FromQuery] bool enabled)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            var existing = await _zoomLicenseService.GetAsync(id, CurrentUser.Id).ConfigureAwait(false);

            existing.Id = existing.Id;
            existing.IsActive = enabled;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = DateTime.UtcNow;

            var savedEntity = await _zoomLicenseService.UpdateAsync(existing).ConfigureAwait(false);
            return new ZoomLicenseResponseModel(savedEntity);
        }

        /// <summary>
        /// Gets Active LicenseId 
        /// </summary>
        /// <param name="zoomLicenseIdRequestModel">the instance of <see cref="LiveClassLicenseRequestModel"></param>
        /// <returns>the instance of <see cref="ZoomLicenseResponseModel"/></returns>
        /// <exception cref="ForbiddenException"></exception>
        [HttpGet("Active")]
        public async Task<List<ZoomLicenseResponseModel>> Active([FromQuery]LiveClassLicenseRequestModel model)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await _validator1.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var zoomLicenses = await _zoomLicenseService.GetActiveLicensesAsync(model).ConfigureAwait(false);
            return zoomLicenses.ToList();
        }
    }
}
