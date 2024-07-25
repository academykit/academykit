namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class WatchHistoryController : BaseApiController
    {
        private readonly IWatchHistoryService watchHistoryService;
        private readonly IValidator<WatchHistoryRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public WatchHistoryController(
            IWatchHistoryService watchHistoryService,
            IValidator<WatchHistoryRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.watchHistoryService = watchHistoryService;
            this.validator = validator;
            this.localizer = localizer;
        }

        /// <summary>
        /// add watch history api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="WatchHistoryRequestModel" /> .</param>
        /// <returns>the instance of <see cref="WatchHistoryResponseModel" /> .</returns>
        [HttpPost]
        public async Task<WatchHistoryResponseModel> AddHistory(WatchHistoryRequestModel model)
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var response = await watchHistoryService
                .CreateAsync(model, CurrentUser.Id)
                .ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// api to pass student in current lesson.
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <param name="model">the instance of <see cref="WatchHistoryRequestModel"/>.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPatch("pass/{userId}")]
        public async Task<CommonResponseModel> Pass(Guid userId, WatchHistoryRequestModel model)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(
                model.CourseIdentity,
                nameof(model.CourseIdentity)
            );
            CommonHelper.ValidateArgumentNotNullOrEmpty(
                model.LessonIdentity,
                nameof(model.LessonIdentity)
            );

            await watchHistoryService
                .PassAsync(userId, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new CommonResponseModel
            {
                Success = true,
                Message = localizer.GetString("WatchHistory")
            };
        }
    }
}
