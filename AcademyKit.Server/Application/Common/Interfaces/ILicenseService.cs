namespace AcademyKit.Application.Common.Interfaces;

using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;

public interface ILicenseService : IGenericService<License, BaseSearchCriteria>
{
    /// <summary>
    /// Validates the license key
    /// </summary>
    /// <param name="userCount">The number of users</param>
    /// <returns>The license validation response</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the license is not found</exception>
    Task<LemonSqueezyResponseModel> ValidateLicenseAsync(int userCount);
}
