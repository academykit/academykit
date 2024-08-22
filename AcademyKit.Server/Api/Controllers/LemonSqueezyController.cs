using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Common;
using Application.Common.Models.ResponseModels;
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
            LEMON_SQUEEZY_CHECKOUT_KEY = configuration.GetSection("LEMON_SQUEEZY:CHECKOUT_KEY").Value;
            LEMON_SQUEEZY_CHECKOUT_URL = configuration.GetSection("LEMON_SQUEEZY:CHECKOUT_URL").Value;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// activate license and save the data in database
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
                var license = new RestClient(LEMON_SQUEEZY_BASE_URL);
                var request = new RestRequest("/v1/licenses/activate");
                request.AddQueryParameter("license_key", model.LicenseKey);
                request.AddQueryParameter("instance_name", CurrentUser.Id);
                request.AddHeader("Accept", "application/json");
                var response = await license.PostAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessful)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        JsonConvert.DeserializeObject<LicenseResponseModel>(response.Content)
                    );
                }

                var licenseResponsess = JsonConvert.DeserializeObject<LicenseResponseModel>(
                    response.Content
                );

                // Map to the License entity
                var data = new License
                {
                    Id = new Guid(),
                    status = Domain.Enums.LicenseStatusType.Active,
                    licenseKey = model.LicenseKey,
                    licenseKeyId = licenseResponsess.license_key.id,
                    CreatedBy = new Guid(),
                    customerEmail = licenseResponsess.meta.customer_email,
                    customerName = licenseResponsess.meta.customer_email,
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
                    return StatusCode(
                        (int)response.StatusCode,
                        JsonConvert.DeserializeObject<LicenseResponseModel>(response.Content)
                    );
                }

                var licenseResponse = JsonConvert.DeserializeObject<LicenseResponseModel>(
                    response.Content
                );

                return Ok(licenseResponse);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }

        [HttpGet("license")]
        [AllowAnonymous]
        public IActionResult GetLicenseAsync()
        {
            try
            {
                var datas = _unitOfWork.GetRepository<License>().GetAll();
                return Ok(datas);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message, ex);
            }
        }

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
