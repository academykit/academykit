namespace Lingtren.Api.Controllers
{
    using System;
    using Hangfire;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class WebhookController : BaseApiController
    {
        private readonly IZoomSettingService _zoomSettingService;
        private readonly IWebhookService _webhookService;
        public WebhookController(IZoomSettingService zoomSettingService,
        IWebhookService webhookService)
        {
           _zoomSettingService = zoomSettingService;
           _webhookService = webhookService;
        }

        /// <summary>
        /// zoom recording webhook api
        /// </summary>
        /// <returns>the task complete </returns>
        [HttpPost("zoomrecording")]
        [AllowAnonymous]
        public async Task<IActionResult> ZoomRecording()
        {
            var header = Request.Headers;
            var authorizationKey = header["authorization"];
            var zoomSetting = await _zoomSettingService.GetFirstOrDefaultAsync();
            if(!authorizationKey.Equals(zoomSetting.WebHookVerificationKey))
            {
                throw new ForbiddenException($"Requested AuthorizationKey {authorizationKey} not matched.");
            }

            using(var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomRecordPayloadDto>(reader);
                BackgroundJob.Enqueue<IWebhookService>(job => job.UploadZoomRecordingAsync(model,null));
            }
            return Ok();
        }

         /// <summary>
        /// join meeting event api
        /// </summary>
        [HttpPost("joinmeeting")]
        [AllowAnonymous]
        public async Task<IActionResult> JoinMeeting()
        {
            var headers = Request.Headers;
            var authorizationKey = headers["authorization"];
             var zoomSetting = await _zoomSettingService.GetFirstOrDefaultAsync();
            if(!authorizationKey.Equals(zoomSetting.WebHookVerificationKey))
            {
                throw new ForbiddenException($"Requested AuthorizationKey {authorizationKey} not matched.");
            }
            using (var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomPayLoadDto>(reader);
                await _webhookService.ParticipantJoinMeetingAsync(model).ConfigureAwait(false);
            }
            return Ok();
        }

        /// <summary>
        /// left meeting event api
        /// </summary>
        [HttpPost("leftmeeting")]
        [AllowAnonymous]
        public async Task<IActionResult> LeftMeeting()
        {
            var headers = Request.Headers;
            var authorizationKey = headers["authorization"];
             var zoomSetting = await _zoomSettingService.GetFirstOrDefaultAsync();
            if(!authorizationKey.Equals(zoomSetting.WebHookVerificationKey))
            {
                throw new ForbiddenException($"Requested AuthorizationKey {authorizationKey} not matched.");
            }
            using (var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomPayLoadDto>(reader);
                await _webhookService.ParticipantLeaveMeetingAsync(model).ConfigureAwait(false);
            }
            return Ok();
        }
    }
}