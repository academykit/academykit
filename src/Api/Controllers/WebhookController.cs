namespace Lingtren.Api.Controllers
{
    using System;
    using System.Text;
    using Hangfire;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class WebhookController : BaseApiController
    {
        private readonly IZoomSettingService _zoomSettingService;
        private readonly IWebhookService _webhookService;
        private readonly ILogger<WebhookController> _logger;
        public WebhookController(IZoomSettingService zoomSettingService,
        IWebhookService webhookService, ILogger<WebhookController> logger)
        {
            _zoomSettingService = zoomSettingService;
            _webhookService = webhookService;
            _logger = logger;
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
            _logger.LogError("Join even api called");
            var zoomSetting = await _zoomSettingService.GetFirstOrDefaultAsync();
            using (var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                _logger.LogError(reader.ToString());
                var model = JsonConvert.DeserializeObject<ZoomPayLoadDto>(reader);
                var plainToken = model.Payload.PlainToken;
                 ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] keyBytes = encoding.GetBytes(zoomSetting.WebHookSecret);
                byte[] messageBytes = encoding.GetBytes(plainToken);
                System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
                byte[] bytes = cryptographer.ComputeHash(messageBytes);
                var hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                _logger.LogError(model.Event);
               // await _webhookService.ParticipantJoinMeetingAsync(model).ConfigureAwait(false);
                var response = new WebHookResponseModel{
                    PlainToken = plainToken,
                    EncryptedToken = hash
                };
                return Ok(response);
            }
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
            if (!authorizationKey.Equals(zoomSetting.WebHookVerificationKey))
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