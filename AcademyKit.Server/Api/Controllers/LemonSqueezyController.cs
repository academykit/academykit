using System.Text.Json;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AcademyKit.Api.Controllers
{
    public class LemonSqueezyController : BaseApiController
    {
        private readonly string LEMON_SQUEEZY_STORE_ID;
        private readonly string LEMON_SQUEEZY_API_KEY;
        private readonly string LEMON_SQUEEZY_BASE_URL;

        public LemonSqueezyController(IConfiguration configuration)
        {
            LEMON_SQUEEZY_STORE_ID = configuration.GetSection("LEMON_SQUEEZY:STORE_ID").Value;
            LEMON_SQUEEZY_API_KEY = configuration.GetSection("LEMON_SQUEEZY:API_KEY").Value;
            LEMON_SQUEEZY_BASE_URL = configuration.GetSection("LEMON_SQUEEZY:BASE_URL").Value;
        }

        [HttpGet("validate")]
        [AllowAnonymous]
        public async Task<ActionResult> ValidateLicenseAsync([FromQuery] string licenseKey)
        {
            if (string.IsNullOrEmpty(licenseKey))
            {
                throw new ArgumentException("License Key is required.");
            }

            try
            {
                var license = new RestClient(LEMON_SQUEEZY_BASE_URL);
                var request = new RestRequest("/v1/licenses/validate");
                request.AddQueryParameter("license_key", licenseKey);
                request.AddHeader("Accept", "application/json");
                var response = await license.PostAsync(request).ConfigureAwait(false);
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

        [HttpGet("activate")]
        [AllowAnonymous]
        public async Task<ActionResult> ActivateLicenseAsync([FromQuery] string licenseKey)
        {
            if (string.IsNullOrEmpty(licenseKey))
            {
                throw new ArgumentException("License Key is required.");
            }

            try
            {
                var license = new RestClient(LEMON_SQUEEZY_BASE_URL);
                var request = new RestRequest("/v1/licenses/activate");
                request.AddQueryParameter("license_key", licenseKey);
                request.AddQueryParameter("instance_name", "User");
                request.AddHeader("Accept", "application/json");
                var response = await license.PostAsync(request).ConfigureAwait(false);
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
