namespace Lingtren.Api.Controllers
{
    using System;
    using Lingtren.Application.Common.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class WebhookController : BaseApiController
    {
        private readonly ILessonService _lessonService;
        private readonly IZoomLicenseService _zoomLicenseService;
        public WebhookController(ILessonService lessonService,
        IZoomLicenseService zoomLicenseService)
        {
           _lessonService = lessonService; 
           _zoomLicenseService = zoomLicenseService;
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

            return Ok();
        }

    }
}