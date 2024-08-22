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
                        JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(response.Content)
                    );
                }

                var licenseResponsess = JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(
                    response.Content
                );

                // Map to the License entity.
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
                var license = new RestClient(LEMON_SQUEEZY_BASE_URL);
                var request = new RestRequest("/v1/licenses/validate");
                request.AddQueryParameter("license_key", licenseKey);
                request.AddHeader("Accept", "application/json");
                var response = await license.PostAsync(request).ConfigureAwait(false);
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
                var datas = _unitOfWork.GetRepository<License>().GetAll();
                return Ok(datas);
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
        /// Response From LemonSqueezy
        /// </summary>
        private class LicenseKey
        {
            public int id { get; set; }

            public string status { get; set; }

            public string key { get; set; }

            public int activation_limit { get; set; }

            public int activation_usage { get; set; }

            public DateTime created_at { get; set; }

            public DateTime? expires_at { get; set; }

            public bool test_mode { get; set; }
        }

        private class Meta
        {
            public int store_id { get; set; }

            public int order_id { get; set; }

            public int order_item_id { get; set; }

            public int variant_id { get; set; }

            public string variant_name { get; set; }

            public int ProductId { get; set; }

            public string product_name { get; set; }

            public int customer_id { get; set; }

            public string customer_name { get; set; }

            public string customer_email { get; set; }
        }

        private class LemonSqueezyResponseModel
        {
            public bool activated { get; set; }

            public bool valid { get; set; }

            public string error { get; set; }

            public LicenseKey license_key { get; set; }

            public object instance { get; set; }

            public Meta meta { get; set; }
        }
    }
}
