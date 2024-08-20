using System.Text.Json;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net.Http.Headers;

namespace AcademyKit.Api.Controllers
{
    public class LemonSqueezyController : BaseApiController
    {
        private readonly string LEMON_SQUEEZY_STORE_ID;
        private readonly string LEMON_SQUEEZY_API_KEY;
        private readonly string LEMON_SQUEEZY_BASE_URL;
        private readonly ILogger<LemonSqueezyController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public LemonSqueezyController(IConfiguration configuration, IUnitOfWork unitOfWork, ILogger<LemonSqueezyController> logger)
        {
            LEMON_SQUEEZY_STORE_ID = configuration.GetSection("LEMON_SQUEEZY:STORE_ID").Value;
            LEMON_SQUEEZY_API_KEY = configuration.GetSection("LEMON_SQUEEZY:API_KEY").Value;
            LEMON_SQUEEZY_BASE_URL = configuration.GetSection("LEMON_SQUEEZY:BASE_URL").Value;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("activate")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateLicenseAsync([FromBody] string licenseKey)
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
                request.AddQueryParameter("instance_name", CurrentUser.Id);
                request.AddHeader("Accept", "application/json");
                var response = await license.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    return StatusCode((int)response.StatusCode, JsonConvert.DeserializeObject<ResponseModel>(response.Content));
                }

                var jObject = JObject.Parse(response.Content);
                var licenseResponse = System.Text.Json.JsonSerializer.Deserialize<LicenseResponse>(response.Content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    AllowTrailingCommas = true,
                    // ReadCommentHandling = JsonCommentHandling.Allow
                });
                var jsonString = JsonConvert.SerializeObject(jObject);
                var json = JsonDocument.Parse(jsonString);
                // Deserialize the response content to LicenseResponse object
                var licenseResponsess = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                // _logger.LogInformation(licenseResponse.LicenseKey.Key);

                // Map to the License entity
                var data = new License
                {
                    Id = new Guid(),
                    status = Domain.Enums.LicenseStatusType.Active,
                    licenseKey = licenseKey,
                    licenseKeyId = licenseResponsess.LicenseKey.Id,
                    CreatedBy = new Guid(),
                    customerEmail = licenseResponsess.Meta.CustomerEmail,
                    customerName = licenseResponsess.Meta.CustomerName,
                    CreatedOn = DateTime.UtcNow,

                };

                // Save to the database
                await _unitOfWork.GetRepository<License>().InsertAsync(data).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                return Ok(data);

            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
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
                if (!response.IsSuccessful)
                {
                    return StatusCode((int)response.StatusCode, JsonConvert.DeserializeObject<ResponseModel>(response.Content));
                }

                // Deserialize the response content to LicenseResponse object
                var licenseResponse = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                // var jObject = JObject.Parse(response.Content);
                // var jsonString = JsonConvert.SerializeObject(jObject);
                // var json = JsonDocument.Parse(jsonString);

                return Ok(licenseResponse);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }
        public class TestResponse
        {
            public IList<TestResponse> ChildrenTokens { get; set; }
            public string Name { get; set; }
            public ValueResponse Value { get; set; }
        }
        public class ValueResponse
        {
            public string Value { get; set; }
        }

        public class LicenseResponse
        {
            public bool Valid { get; set; }
            public string Error { get; set; }
            public LicenseKey LicenseKey { get; set; }
            public Instance Instance { get; set; }
            public Meta Meta { get; set; }
        }

        public class LicenseKey
        {
            public int Id { get; set; }
            public string Status { get; set; }
            public string Key { get; set; }
            public int ActivationLimit { get; set; }
            public int ActivationUsage { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        public class Instance
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class Meta
        {
            public int StoreId { get; set; }
            public int OrderId { get; set; }
            public int OrderItemId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int VariantId { get; set; }
            public string VariantName { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
        }
    }
}
