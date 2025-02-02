﻿using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Localization;
using AcademyKit.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using RestSharp;

namespace AcademyKit.Api.Controllers;

/// <summary>
/// Controller for handling Lemon Squeezy related operations.
/// </summary>
public class LemonSqueezyController : BaseApiController
{
    private readonly string _lemonSqueezyBaseUrl;
    private readonly string _lemonSqueezyCheckoutKey;
    private readonly string _lemonSqueezyCheckoutUrl;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<ExceptionLocalizer> _stringLocalizer;

    private readonly ILicenseService _licenseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LemonSqueezyController"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    public LemonSqueezyController(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IStringLocalizer<ExceptionLocalizer> stringLocalizer,
        ILicenseService licenseService
    )
    {
        _lemonSqueezyBaseUrl = configuration["LEMON_SQUEEZY:BASE_URL"];
        _lemonSqueezyCheckoutKey = configuration["LEMON_SQUEEZY:CHECKOUT_KEY"];
        _lemonSqueezyCheckoutUrl = configuration["LEMON_SQUEEZY:CHECKOUT_URL"];
        _unitOfWork = unitOfWork;
        _stringLocalizer = stringLocalizer;
        _licenseService = licenseService;
    }

    /// <summary>
    /// Activates a license and saves the data in the database.
    /// </summary>
    /// <param name="model">The license request model.</param>
    /// <returns>The activated license information.</returns>
    [HttpPost("activate")]
    [AllowAnonymous]
    public async Task<IActionResult> ActivateLicenseAsync([FromBody] LicenseRequestModel model)
    {
        if (string.IsNullOrEmpty(model.LicenseKey))
        {
            return BadRequest("License Key is required.");
        }

        try
        {
            var response = await SendLemonSqueezyRequest(
                "/v1/licenses/activate",
                model.LicenseKey,
                CurrentUser.Id
            );
            return await ProcessLicenseResponse(response, model.LicenseKey, true);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Validates a license key.
    /// </summary>
    /// <param name="licenseKey">The license key to validate.</param>
    /// <returns>The validation result.</returns>
    [HttpGet("validate")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateLicenseAsync()
    {
        var userCount = await _unitOfWork.GetRepository<User>().CountAsync().ConfigureAwait(false);
        return Ok(await _licenseService.ValidateLicenseAsync(userCount).ConfigureAwait(false));
    }

    /// <summary>
    /// Gets the list of licenses.
    /// </summary>
    /// <returns>The list of licenses.</returns>
    [HttpGet("license")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLicenseAsync()
    {
        try
        {
            var data = await _unitOfWork
                .GetRepository<License>()
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gets the checkout URL for license key purchase.
    /// </summary>
    /// <returns>The checkout URL.</returns>
    [HttpGet("checkout")]
    [AllowAnonymous]
    public IActionResult CheckoutLicenseAsync()
    {
        try
        {
            var checkoutUrl = $"{_lemonSqueezyCheckoutUrl}/{_lemonSqueezyCheckoutKey}";
            return Ok(new { checkoutUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates the license key.
    /// </summary>
    /// <param name="model">The model containing the new license key.</param>
    /// <returns>The updated license information.</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateLicenseKeyAsync([FromBody] LicenseKeyRequestModel model)
    {
        if (string.IsNullOrEmpty(model.LicenseKey))
        {
            return BadRequest("License Key is required.");
        }

        try
        {
            var response = await SendLemonSqueezyRequest("/v1/licenses/validate", model.LicenseKey);
            if (!response.IsSuccessful)
            {
                return NotFound(new { message = _stringLocalizer.GetString("InvalidKey").Value });
            }

            var licenseResponse = JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(
                response.Content
            );
            if (licenseResponse.LicenseKey.Status.ToLower() != "active")
            {
                var activateResponse = await SendLemonSqueezyRequest(
                    "/v1/licenses/activate",
                    model.LicenseKey,
                    CurrentUser.Id
                );
                return await ProcessLicenseResponse(activateResponse, model.LicenseKey, true);
            }

            return await ProcessLicenseResponse(response, model.LicenseKey, true);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Sends a request to the Lemon Squeezy API.
    /// </summary>
    /// <param name="endpoint">The API endpoint.</param>
    /// <param name="licenseKey">The license key.</param>
    /// <param name="instanceName">The optional instance name.</param>
    /// <returns>The API response.</returns>
    private async Task<RestResponse> SendLemonSqueezyRequest(
        string endpoint,
        string licenseKey,
        Guid? instanceName = null
    )
    {
        var client = new RestClient(_lemonSqueezyBaseUrl);
        var request = new RestRequest(endpoint)
            .AddQueryParameter("license_key", licenseKey)
            .AddHeader("Accept", "application/json");

        if (instanceName.HasValue)
        {
            request.AddQueryParameter("instance_name", instanceName.ToString());
        }

        return await client.PostAsync(request);
    }

    /// <summary>
    /// Processes the license response from Lemon Squeezy API.
    /// </summary>
    /// <param name="response">The API response.</param>
    /// <param name="licenseKey">The license key.</param>
    /// <param name="saveToDatabase">Whether to save or update the license in the database.</param>
    /// <returns>The processed result.</returns>
    private async Task<IActionResult> ProcessLicenseResponse(
        RestResponse response,
        string licenseKey,
        bool saveToDatabase
    )
    {
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

        if (saveToDatabase)
        {
            var license = await SaveOrUpdateLicense(licenseResponse, licenseKey);
            return Ok(license);
        }

        return Ok(licenseResponse);
    }

    /// <summary>
    /// Saves a new license or updates an existing one in the database.
    /// </summary>
    /// <param name="response">The Lemon Squeezy response model.</param>
    /// <param name="licenseKey">The license key.</param>
    /// <returns>The saved or updated license.</returns>
    private async Task<License> SaveOrUpdateLicense(
        LemonSqueezyResponseModel response,
        string licenseKey
    )
    {
        var existingLicense = await _unitOfWork
            .GetRepository<License>()
            .GetFirstOrDefaultAsync()
            .ConfigureAwait(false);

        var license =
            existingLicense != null
                ? existingLicense
                : new License
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = CurrentUser.Id,
                    CreatedOn = DateTime.UtcNow,
                };

        UpdateLicenseProperties(license, response, licenseKey);

        if (existingLicense == null)
        {
            await _unitOfWork.GetRepository<License>().InsertAsync(license).ConfigureAwait(false);
        }
        else
        {
            _unitOfWork.GetRepository<License>().Update(license);
        }

        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        return existingLicense;
    }

    /// <summary>
    /// Updates the properties of a license entity.
    /// </summary>
    /// <param name="license">The license entity to update.</param>
    /// <param name="response">The Lemon Squeezy response model.</param>
    /// <param name="licenseKey">The license key.</param>
    private static void UpdateLicenseProperties(
        License license,
        LemonSqueezyResponseModel response,
        string licenseKey
    )
    {
        license.Status = Domain.Enums.LicenseStatusType.Active;
        license.LicenseKey = licenseKey;
        license.LicenseKeyId = response.LicenseKey.Id;
        license.CustomerEmail = response.Meta.CustomerEmail;
        license.CustomerName = response.Meta.CustomerName;
        license.ActivatedOn = response.LicenseKey.CreatedAt;
        license.ExpiredOn = response.LicenseKey.ExpiresAt;
        license.VariantName = response.Meta.VariantName;
        license.VariantId = response.Meta.VariantId;
    }
}
