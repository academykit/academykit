namespace AcademyKit.Infrastructure.Services;

using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Interfaces;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;
using AcademyKit.Infrastructure.Common;
using AcademyKit.Infrastructure.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

public class LicenseService : BaseGenericService<License, BaseSearchCriteria>, ILicenseService
{
    private readonly string _lemonSqueezyBaseUrl;
    private readonly string _licenseHandlerUrl;

    public LicenseService(
        IUnitOfWork unitOfWork,
        ILogger<LicenseService> logger,
        IStringLocalizer<ExceptionLocalizer> localizer,
        IConfiguration configuration
    )
        : base(unitOfWork, logger, localizer)
    {
        _lemonSqueezyBaseUrl = configuration["LEMON_SQUEEZY:BASE_URL"] ?? string.Empty;
        _licenseHandlerUrl = configuration["LEMON_SQUEEZY:LICENSE_HANDLER_URL"] ?? string.Empty;
    }

    /// <summary>
    /// Validates the license key
    /// </summary>
    /// <param name="userCount">The number of users</param>
    /// <returns>The license validation response</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the license is not found</exception>
    public async Task<LemonSqueezyResponseModel> ValidateLicenseAsync(int userCount)
    {
        try
        {
            var license = await _unitOfWork
                .GetRepository<License>()
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (license == null)
            {
                _logger.LogError("License not found");
                throw new EntityNotFoundException(_localizer.GetString("LicenseNotFound"));
            }

            var client = new RestClient(_lemonSqueezyBaseUrl);
            var request = new RestRequest("/v1/licenses/validate")
                .AddQueryParameter("license_key", license.LicenseKey)
                .AddHeader("Accept", "application/json");

            var response = await client.PostAsync(request);

            var licenseData = JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(
                response.Content
            );

            var handlerClient = new RestClient(_licenseHandlerUrl);
            var handlerRequest = new RestRequest("/license-handler", Method.Post)
                .AddJsonBody(new { userCount, licenseData })
                .AddHeader("Accept", "application/json")
                .AddHeader("Content-Type", "application/json");

            var handlerResponse = await handlerClient.PostAsync(handlerRequest);

            var responseObject = JsonConvert.DeserializeObject<LemonSqueezyResponseModel>(
                handlerResponse.Content
            );

            return responseObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating and validating license");
            throw ex is ServiceException
                ? ex
                : new ServiceException(_localizer.GetString("LicenseUpdateError"));
        }
    }
}
