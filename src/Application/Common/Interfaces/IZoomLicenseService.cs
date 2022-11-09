namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;

    public interface IZoomLicenseService : IGenericService<ZoomLicense, ZoomLicenseBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to get active available zoom license with in given time period
        /// </summary>
        /// <param name="startDateTime">meeting start date</param>
        /// <param name="duration">meeting duration</param>
        /// <returns></returns>
        Task<IList<ZoomLicenseResponseModel>> GetActiveLicenses(DateTime startDateTime, int duration);
    }
}
