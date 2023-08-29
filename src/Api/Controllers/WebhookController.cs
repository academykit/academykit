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
            var zoomSetting = await _zoomSettingService.GetFirstOrDefaultAsync();
            using (var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomRecordPayloadDto>(reader);
                if (model.Event.Equals("endpoint.url_validation"))
                {
                    var plainToken = model.Payload.PlainToken;
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] keyBytes = encoding.GetBytes(zoomSetting.WebHookSecret);
                    byte[] messageBytes = encoding.GetBytes(plainToken);
                    System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
                    byte[] bytes = cryptographer.ComputeHash(messageBytes);
                    var hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                    var response = new WebHookResponseModel
                    {
                        PlainToken = plainToken,
                        EncryptedToken = hash
                    };
                    return Ok(response);
                }
                BackgroundJob.Enqueue<IWebhookService>(job => job.UploadZoomRecordingAsync(model, null));
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
            var zoomSetting = await _zoomSettingService.GetFirstOrDefaultAsync();
            using(var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomPayLoadDto>(reader);
                _logger.LogInformation(model.Event);
                if (model.Event.Equals("endpoint.url_validation"))
                {
                    var plainToken = model.Payload.PlainToken;
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] keyBytes = encoding.GetBytes(zoomSetting.WebHookSecret);
                    byte[] messageBytes = encoding.GetBytes(plainToken);
                    System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
                    byte[] bytes = cryptographer.ComputeHash(messageBytes);
                    var hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                    var response = new WebHookResponseModel
                    {
                        PlainToken = plainToken,
                        EncryptedToken = hash
                    };
                    return Ok(response);
                }
                await _webhookService.ParticipantJoinMeetingAsync(model).ConfigureAwait(false);
                return Ok();
            }
        }

        /// <summary>
        /// left meeting event api
        /// </summary>
        [HttpPost("leftmeeting")]
        [AllowAnonymous]
        public async Task<IActionResult> LeftMeeting()
        {
            var zoomSetting = await _zoomSettingService.GetFirstOrDefaultAsync();
            using (var stream = new StreamReader(Request.Body))
            {
                var reader = await stream.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ZoomPayLoadDto>(reader);
                _logger.LogInformation(model.ToString());
                _logger.LogInformation(model.Payload.ToString());
                _logger.LogInformation(model.Event);
                if (model.Event.Equals("endpoint.url_validation"))
                {
                    var plainToken = model.Payload.PlainToken;
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] keyBytes = encoding.GetBytes(zoomSetting.WebHookSecret);
                    byte[] messageBytes = encoding.GetBytes(plainToken);
                    System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
                    byte[] bytes = cryptographer.ComputeHash(messageBytes);
                    var hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                    var response = new WebHookResponseModel
                    {
                        PlainToken = plainToken,
                        EncryptedToken = hash
                    };
                    return Ok(response);
                }
                await _webhookService.ParticipantLeaveMeetingAsync(model).ConfigureAwait(false);
                return Ok();
            }
        }
    }
}
