using System.ComponentModel.DataAnnotations;
using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace AcademyKit.Api.Controllers
{
    public class LemonSqueezyController : BaseApiController
    {
        private readonly string LEMON_SQUEEZY_BASE_URL;
        private readonly string LEMON_SQUEEZY_CHECKOUT_KEY;
        private readonly string LEMON_SQUEEZY_CHECKOUT_URL;

        private readonly IUnitOfWork _unitOfWork;

        public LemonSqueezyController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            LEMON_SQUEEZY_BASE_URL = configuration.GetSection("LEMON_SQUEEZY:BASE_URL").Value;
            LEMON_SQUEEZY_CHECKOUT_KEY = configuration
                .GetSection("LEMON_SQUEEZY:CHECKOUT_KEY")
                .Value;
            LEMON_SQUEEZY_CHECKOUT_URL = configuration
                .GetSection("LEMON_SQUEEZY:CHECKOUT_URL")
                .Value;
            _unitOfWork = unitOfWork;
        }

        private async Task<RestResponse> SendLemonSqueezyRequest(string endpoint, string licenseKey, Guid? instanceName = null)
        {
            var client = new RestClient(LEMON_SQUEEZY_BASE_URL);
            var request = new RestRequest(endpoint);
            request.AddQueryParameter("license_key", licenseKey);
            if (instanceName != null)
            {
                request.AddQueryParameter("instance_name", instanceName.ToString());
            }
            request.AddHeader("Accept", "application/json");
            return await client.PostAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// activate license and save the data in database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ServiceException"></exception>
        [HttpPost("activate")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateLicenseAsync([FromBody] LicenseRequestModel model)
        {
            if (string.IsNullOrEmpty(model.LicenseKey))
            {
                throw new ArgumentException("License Key is required.");
            }

            try
            {

                var response = await SendLemonSqueezyRequest("/v1/licenses/activate", model.LicenseKey, CurrentUser.Id);

                if (!response.IsSuccessful)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(response.Content)
                    );
                }

                var licenseResponses = JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(
                    response.Content
                );

                // Map to the License entity.
                var data = new License
                {
                    Id = new Guid(),
                    Status = Domain.Enums.LicenseStatusType.Active,
                    LicenseKey = model.LicenseKey,
                    LicenseKeyId = licenseResponses.LicenseKey.Id,
                    CreatedBy = new Guid(),
                    CustomerEmail = licenseResponses.Meta.CustomerEmail,
                    CustomerName = licenseResponses.Meta.CustomerName,
                    CreatedOn = DateTime.UtcNow,
                    ActivatedOn = licenseResponses.Meta.CreatedAt,
                    ExpiredOn = licenseResponses.LicenseKey.ExpiresAt,
                    VariantName = licenseResponses.Meta.VariantName,
                    VariantId = licenseResponses.Meta.VariantId,
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

        /// <summary>
        /// validate the license key.
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ServiceException"></exception>
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
                var response = await SendLemonSqueezyRequest("/v1/licenses/validate", licenseKey);

                if (!response.IsSuccessful)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(response.Content)
                    );
                }

                var licenseResponse = JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(
                    response.Content
                );

                return Ok(licenseResponse);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }

        /// <summary>
        ///  get the list of license
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        [HttpGet("license")]
        [AllowAnonymous]
        public IActionResult GetLicenseAsync()
        {
            try
            {
                var data = _unitOfWork.GetRepository<License>().GetAll();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// get the checkout url for license key.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        [HttpGet("checkout")]
        [AllowAnonymous]
        public IActionResult CheckoutLicenseAsync()
        {
            try
            {
                var checkoutUrl = $"{LEMON_SQUEEZY_CHECKOUT_URL}/{LEMON_SQUEEZY_CHECKOUT_KEY}";
                return Ok(new { checkoutUrl });
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Update the license key.
        /// </summary>
        /// <param name="model">The model containing the new license key.</param>
        /// <returns>The updated license information.</returns>
        /// <exception cref="ServiceException"></exception>
        [HttpPut("update")]
        public async Task<ActionResult> UpdateLicenseKeyAsync([FromBody] UpdateLicenseKeyModel model)
        {
            if (string.IsNullOrEmpty(model.LicenseKey))
            {
                return BadRequest("License Key is required.");
            }

            try
            {
                // Validate the new license key with LemonSqueezy
                var response = await SendLemonSqueezyRequest("/v1/licenses/validate", model.LicenseKey);

                if (!response.IsSuccessful)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(response.Content)
                    );
                }

                var licenseResponse = JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(
                    response.Content
                );

                if (!licenseResponse.Valid)
                {
                    return BadRequest("Invalid license key.");
                }

                // Update the license in the database
                var existingLicense = await _unitOfWork.GetRepository<License>()
                    .GetFirstOrDefaultAsync(predicate: l => l.LicenseKey == model.LicenseKey);

                if (existingLicense == null)
                {
                    // If the license doesn't exist, create a new one
                    var newLicense = new License
                    {
                        Id = Guid.NewGuid(),
                        Status = Domain.Enums.LicenseStatusType.Active,
                        LicenseKey = model.LicenseKey,
                        LicenseKeyId = licenseResponse.LicenseKey.Id,
                        CreatedBy = CurrentUser.Id,
                        CustomerEmail = licenseResponse.Meta.CustomerEmail,
                        CustomerName = licenseResponse.Meta.CustomerName,
                        CreatedOn = DateTime.UtcNow,
                    };

                    await _unitOfWork.GetRepository<License>().InsertAsync(newLicense).ConfigureAwait(false);
                }
                else
                {
                    // Update existing license
                    existingLicense.LicenseKey = model.LicenseKey;
                    existingLicense.LicenseKeyId = licenseResponse.LicenseKey.Id;
                    existingLicense.Status = Domain.Enums.LicenseStatusType.Active;
                    existingLicense.CustomerEmail = licenseResponse.Meta.CustomerEmail;
                    existingLicense.CustomerName = licenseResponse.Meta.CustomerName;
                    existingLicense.UpdatedOn = DateTime.UtcNow;
                    existingLicense.UpdatedBy = CurrentUser.Id;

                    _unitOfWork.GetRepository<License>().Update(existingLicense);
                }

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                return Ok(licenseResponse);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }

        public class UpdateLicenseKeyModel
        {
            [Required]
            public string LicenseKey { get; set; }
        }

        /// <summary>
        /// Response From LemonSqueezy
        /// </summary>
        public class LicenseKey
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("activation_limit")]
            public int ActivationLimit { get; set; }

            [JsonProperty("activation_usage")]
            public int ActivationUsage { get; set; }

            [JsonProperty("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("expires_at")]
            public DateTime ExpiresAt { get; set; }

            [JsonProperty("test_mode")]
            public bool TestMode { get; set; }
        }

        public class Meta
        {
            [JsonProperty("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("store_id")]
            public int StoreId { get; set; }

            [JsonProperty("order_id")]
            public int OrderId { get; set; }

            [JsonProperty("order_item_id")]
            public int OrderItemId { get; set; }

            [JsonProperty("variant_id")]
            public int VariantId { get; set; }

            [JsonProperty("variant_name")]
            public string VariantName { get; set; }

            [JsonProperty("product_id")]
            public int ProductId { get; set; }

            [JsonProperty("product_name")]
            public string ProductName { get; set; }

            [JsonProperty("customer_id")]
            public int CustomerId { get; set; }

            [JsonProperty("customer_name")]
            public string CustomerName { get; set; }

            [JsonProperty("customer_email")]
            public string CustomerEmail { get; set; }
        }

        public class LemonSqueezyResponseModel
        {
            [JsonProperty("activated")]
            public bool Activated { get; set; }

            [JsonProperty("valid")]
            public bool Valid { get; set; }

            [JsonProperty("error")]
            public string Error { get; set; }

            [JsonProperty("license_key")]
            public LicenseKey LicenseKey { get; set; }

            [JsonProperty("instance")]
            public object Instance { get; set; }

            [JsonProperty("meta")]
            public Meta Meta { get; set; }
        }
    }
}
