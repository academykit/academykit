using System.Text.Json;
using AcademyKit.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AcademyKit.Api.Controllers
{
    public class IframelyController : BaseApiController
    {
        private readonly string IFRAMELY_API_BASE_URL;
        private readonly string IFRAMELY_API_KEY;

        public IframelyController(IConfiguration configuration)
        {
            IFRAMELY_API_BASE_URL = configuration.GetSection("IFRAMELY:API_BASE_URL").Value;
            IFRAMELY_API_KEY = configuration.GetSection("IFRAMELY:API_KEY").Value;
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
                var client = new RestClient(IFRAMELY_API_BASE_URL);
                var request = new RestRequest("/api/oembed");
                request.AddQueryParameter("url", url);
                request.AddQueryParameter("key", IFRAMELY_API_KEY);
                request.AddQueryParameter("iframe", 1);
                request.AddQueryParameter("omit_script", 1);
                request.AddHeader("Content-Type", "application/json");
                var response = await client.GetAsync(request).ConfigureAwait(false);
                var jObject = JObject.Parse(response.Content);
                var jsonString = JsonConvert.SerializeObject(jObject);
                var json = JsonDocument.Parse(jsonString);
                return Ok(json);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }
    }
}
