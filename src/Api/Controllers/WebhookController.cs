namespace AcademyKit.Api.Controllers
{
    using System;
    using System.Text;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Infrastructure.Helpers;

    using Hangfire;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class WebhookController : BaseApiController
    {
        private readonly IZoomSettingService zoomSettingService;
        private readonly IWebhookService webhookService;
        private readonly ILogger<WebhookController> logger;

        public WebhookController(
            IZoomSettingService zoomSettingService,
            IWebhookService webhookService,
            ILogger<WebhookController> logger
        )
        {
            this.zoomSettingService = zoomSettingService;
            this.webhookService = webhookService;
            this.logger = logger;
        }

        /// <summary>
        /// zoom recording webhook api.
        /// </summary>
        /// <returns>the task complete. </returns>
        [HttpPost("zoomrecording")]
        [AllowAnonymous]
        public async Task<IActionResult> ZoomRecording()
        {
            logger.LogInformation("Zoom Recording API");
            var zoomSetting = await zoomSettingService.GetFirstOrDefaultAsync();
            using (var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomRecordPayloadDto>(reader);
                if (model.Event.Equals("endpoint.url_validation"))
                {
                    var plainToken = model.Payload.PlainToken;
                    var encoding = new ASCIIEncoding();
                    var keyBytes = encoding.GetBytes(zoomSetting.WebHookSecret);
                    var messageBytes = encoding.GetBytes(plainToken);
                    var cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
                    var bytes = cryptographer.ComputeHash(messageBytes);
                    var hash = BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
                    var response = new WebHookResponseModel
                    {
                        PlainToken = plainToken,
                        EncryptedToken = hash,
                    };
                    return Ok(response);
                }

                BackgroundJob.Enqueue<IWebhookService>(job =>
                    job.UploadZoomRecordingAsync(model, null)
                );
            }

            return Ok();
        }

        /// <summary>
        /// join meeting event api.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPost("joinmeeting")]
        [AllowAnonymous]
        public async Task<IActionResult> JoinMeeting()
        {
            logger.LogInformation("Join Meeting API");
            var zoomSetting = await zoomSettingService.GetFirstOrDefaultAsync();
            using (var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomPayLoadDto>(reader);
                logger.LogInformation(model.Event.SanitizeForLogger());
                if (model.Event.Equals("endpoint.url_validation"))
                {
                    var plainToken = model.Payload.PlainToken;
                    var encoding = new ASCIIEncoding();
                    var keyBytes = encoding.GetBytes(zoomSetting.WebHookSecret);
                    var messageBytes = encoding.GetBytes(plainToken);
                    var cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
                    var bytes = cryptographer.ComputeHash(messageBytes);
                    var hash = BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
                    var response = new WebHookResponseModel
                    {
                        PlainToken = plainToken,
                        EncryptedToken = hash,
                    };
                    return Ok(response);
                }

                await webhookService.ParticipantJoinMeetingAsync(model).ConfigureAwait(false);
                return Ok();
            }
        }

        /// <summary>
        /// left meeting event api.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPost("leftmeeting")]
        [AllowAnonymous]
        public async Task<IActionResult> LeftMeeting()
        {
            logger.LogInformation("Left Meeting API");
            var zoomSetting = await zoomSettingService.GetFirstOrDefaultAsync();
            using (var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomPayLoadDto>(reader);
                if (model.Event.Equals("endpoint.url_validation"))
                {
                    var plainToken = model.Payload.PlainToken;
                    var encoding = new ASCIIEncoding();
                    var keyBytes = encoding.GetBytes(zoomSetting.WebHookSecret);
                    var messageBytes = encoding.GetBytes(plainToken);
                    var cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
                    var bytes = cryptographer.ComputeHash(messageBytes);
                    var hash = BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
                    var response = new WebHookResponseModel
                    {
                        PlainToken = plainToken,
                        EncryptedToken = hash,
                    };
                    return Ok(response);
                }

                await webhookService.ParticipantLeaveMeetingAsync(model).ConfigureAwait(false);
                return Ok();
            }
        }
    }
}
