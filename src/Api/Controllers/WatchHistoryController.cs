namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Helpers;
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
        /// <returns>the instance of <see cref="WatchHistoryResponseModel" /> .</returns>
        [HttpPost]
        public async Task<WatchHistoryResponseModel> AddHistory(WatchHistoryRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var response = await _watchHistoryService.CreateAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="model">the instance of <see cref="WatchHistoryRequestModel"/></param>
        /// <returns></returns>
        [HttpPatch("pass/{userId}")]
        public async Task<CommonResponseModel> Pass(Guid userId, WatchHistoryRequestModel model)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(model.CourseIdentity, nameof(model.CourseIdentity));
            CommonHelper.ValidateArgumentNotNullOrEmpty(model.LessonIdentity, nameof(model.LessonIdentity));

            await _watchHistoryService.PassAsync(userId, model, CurrentUser.Id).ConfigureAwait(false);
            return new CommonResponseModel { Success = true, Message = "Watch history updated successfully." };
        }
    }
}
