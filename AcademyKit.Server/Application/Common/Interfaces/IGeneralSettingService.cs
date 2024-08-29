namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Server.Application.Common.Models.RequestModels;

    public interface IGeneralSettingService : IGenericService<GeneralSetting, BaseSearchCriteria>
    {
        /// <summary>
        /// Handles the initial setup process by creating general settings, a user,
        /// and a default group based on the provided company name.
        /// </summary>
        /// <param name="setupRequest">The instance of <see cref="InitialSetupRequestModel"/> containing setup details.</param>
        /// <returns>the instance of <see cref="GeneralSetting"/></returns>
        /// <exception cref="ServiceException">Thrown when an error occurs during the setup process.</exception>
        Task<GeneralSetting> InitialSetupAsync(InitialSetupRequestModel setupRequest);
    }
}
