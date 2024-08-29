namespace AcademyKit.Infrastructure.Services
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Helpers;
    using AcademyKit.Infrastructure.Localization;
    using AcademyKit.Server.Application.Common.Interfaces;
    using AcademyKit.Server.Application.Common.Models.RequestModels;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class GeneralSettingService
        : BaseGenericService<GeneralSetting, BaseSearchCriteria>,
            IGeneralSettingService
    {
        private readonly IPasswordHasher _passwordHasher;

        public GeneralSettingService(
            IUnitOfWork unitOfWork,
            ILogger<GeneralSettingService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IPasswordHasher passwordHasher
        )
            : base(unitOfWork, logger, localizer)
        {
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<GeneralSetting, object> IncludeNavigationProperties(
            IQueryable<GeneralSetting> query
        )
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// Handles the initial setup process by creating general settings, a user,
        /// and a default group based on the provided company name.
        /// </summary>
        /// <param name="setupRequest">The instance of <see cref="InitialSetupRequestModel"/> containing setup details.</param>
        /// <returns>the instance of <see cref="GeneralSetting"/></returns>
        /// <exception cref="ServiceException">Thrown when an error occurs during the setup process.</exception>
        public async Task<GeneralSetting> InitialSetupAsync(InitialSetupRequestModel setupRequest)
        {
            try
            {
                var existingGeneralSetting = await GetFirstOrDefaultAsync().ConfigureAwait(false);

                if (existingGeneralSetting != null && existingGeneralSetting.IsSetupCompleted)
                {
                    _logger.LogInformation("Company setup is already completed.");
                    throw new ForbiddenException(
                        _localizer.GetString("CompanySetupAlreadyCompleted")
                    );
                }

                var userId = Guid.NewGuid();
                var currentTimeStamp = DateTime.UtcNow;

                var user = new User
                {
                    Id = userId,
                    FirstName = setupRequest.FirstName,
                    LastName = setupRequest.LastName,
                    Email = setupRequest.Email,
                    HashPassword = _passwordHasher.HashPassword(setupRequest.Password),
                    Role = UserRole.SuperAdmin,
                    Status = UserStatus.Active,
                    CreatedBy = userId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = userId,
                    UpdatedOn = currentTimeStamp
                };

                var generalSetting = new GeneralSetting
                {
                    Id = Guid.NewGuid(),
                    CompanyName = setupRequest.CompanyName,
                    CompanyAddress = setupRequest.CompanyAddress,
                    LogoUrl = setupRequest.LogoUrl,
                    IsSetupCompleted = true,
                    CreatedBy = userId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = userId,
                    UpdatedOn = currentTimeStamp
                };

                var defaultGroup = new Group
                {
                    Id = Guid.NewGuid(),
                    Name = setupRequest.CompanyName,
                    IsDefault = true,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = userId,
                    UpdatedOn = currentTimeStamp
                };

                defaultGroup.Slug = CommonHelper.GetEntityTitleSlug<Group>(
                    _unitOfWork,
                    (slug) => q => q.Slug == slug,
                    defaultGroup.Name
                );

                await _unitOfWork
                    .GetRepository<GeneralSetting>()
                    .InsertAsync(generalSetting)
                    .ConfigureAwait(false);
                await _unitOfWork.GetRepository<User>().InsertAsync(user).ConfigureAwait(false);
                await _unitOfWork
                    .GetRepository<Group>()
                    .InsertAsync(defaultGroup)
                    .ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                return generalSetting;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during initial setup.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("InitialSetupError"));
            }
        }
    }
}
