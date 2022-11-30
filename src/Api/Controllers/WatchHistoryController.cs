namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;
    public class WatchHistoryController : BaseApiController
    {
        private readonly IWatchHistoryService _watchHistoryService;
        private readonly IValidator<WatchHistoryRequestModel> _validator;
        public WatchHistoryController(
            IWatchHistoryService watchHistoryService,
            IValidator<WatchHistoryRequestModel> validator)
        {
            _watchHistoryService = watchHistoryService;
            _validator = validator;
        }
        /// <summary>
        /// add watch history api
        /// </summary>
        /// <param name="model"> the instance of <see cref="WatchHistoryRequestModel" /> .</param>
        /// <returns>the instance of <see cref="HistoryResponseModel" /> .</returns>
        [HttpPost]
        public async Task<WatchHistoryResponseModel> AddHistory(WatchHistoryRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var response = await _watchHistoryService.CreateAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return response;
        }
    }
}
