using AcademyKit.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AcademyKit.Api.Controllers
{
    public class IframelyController : BaseApiController
    {
        private readonly ILogger<IframelyController> logger;
        private readonly string IFRAMELY_API_BASE_URL;
        private readonly string IFRAMELY_API_KEY;

        public IframelyController(IConfiguration configuration, ILogger<IframelyController> logger)
        {
            IFRAMELY_API_BASE_URL = configuration.GetSection("IFRAMELY:API_BASE_URL").Value;
            IFRAMELY_API_KEY = configuration.GetSection("IFRAMELY:API_KEY").Value;
            this.logger = logger;
        }

        [HttpGet("oembed")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOEmbedContentAsync([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL parameter is required.");
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var requestUrl =
                        $"{IFRAMELY_API_BASE_URL}/api/oembed?url={System.Web.HttpUtility.UrlEncode(url)}&key={IFRAMELY_API_KEY}";

                    // Send the request to the iFramely API
                    var response = await httpClient.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode(); // Throw if not a success code.

                    // Read the response content
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Optionally, parse the JSON response if you need to process it
                    var jsonResponse = JObject.Parse(responseBody);
                    logger.LogDebug("response from iframely", responseBody);
                    // Return JSON response to the client
                    return Ok(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }
    }
}
