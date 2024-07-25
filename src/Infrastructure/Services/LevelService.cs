namespace Lingtren.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class LevelService : BaseGenericService<Level, BaseSearchCriteria>, ILevelService
    {
        public LevelService(
            IUnitOfWork unitOfWork,
            ILogger<LevelService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        /// <summary>
        /// Handle to create the level
        /// </summary>
        /// <param name="name"> the tag name </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="Level" /> .</returns>
        public async Task<Level> CreateLevelAsync(string name, Guid currentUserId)
        {
            try
            {
                var levelName = name.TrimStart().TrimEnd();
                var isAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!isAdmin)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var slug = CommonHelper.GetEntityTitleSlug<Level>(
                    _unitOfWork,
                    (slug) => q => q.Slug == slug,
                    levelName
                );
                var level = await _unitOfWork
                    .GetRepository<Level>()
                    .GetFirstOrDefaultAsync(predicate: x =>
                        x.Name.ToLower() == levelName.ToLower() && x.IsActive
                    )
                    .ConfigureAwait(false);
                if (level != default)
                {
                    throw new ForbiddenException(_localizer.GetString("LevelAlreadyExist"));
                }

                var entity = new Level()
                {
                    Id = Guid.NewGuid(),
                    Slug = slug,
                    Name = levelName,
                    IsActive = true,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Level>().InsertAsync(entity).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to create level.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("LevelCreateError"));
            }
        }

        /// <summary>
        /// Handle to update level async
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="name"> the level name </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="Level" />.</returns>
        public async Task<Level> UpdateLevelAsync(string identity, string name, Guid currentUserId)
        {
            try
            {
                var levelName = name.TrimStart().TrimEnd();
                var isAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!isAdmin)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var levels = await _unitOfWork
                    .GetRepository<Level>()
                    .GetAllAsync(predicate: x => x.IsActive)
                    .ConfigureAwait(false);

                var level = levels.FirstOrDefault(x =>
                    x.Id.ToString() == identity || x.Slug.Equals(identity)
                );
                if (level == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("LevelNotFound"));
                }

                var levelNameExist = levels.Any(x =>
                    x.Id != level.Id && x.Name.ToLower() == levelName.ToLower()
                );
                if (levelNameExist)
                {
                    throw new ForbiddenException(_localizer.GetString("LevelNameAlreadyExist"));
                }

                level.Name = levelName;
                level.UpdatedOn = DateTime.UtcNow;
                level.UpdatedBy = currentUserId;
                _unitOfWork.GetRepository<Level>().Update(level);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return level;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update level");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("LevelUpdate"));
            }
        }

        /// <summary>
        /// Handle to delete the level
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task DeleteLevelAsync(string identity, Guid currentUserId)
        {
            try
            {
                var isAdmin = await IsSuperAdminOrAdmin(currentUserId).ConfigureAwait(false);
                if (!isAdmin)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var level = await _unitOfWork
                    .GetRepository<Level>()
                    .GetFirstOrDefaultAsync(predicate: x =>
                        x.Id.ToString() == identity || x.Slug.Equals(identity)
                    )
                    .ConfigureAwait(false);
                if (level == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("TagNotFound"));
                }

                level.IsActive = false;
                level.UpdatedBy = currentUserId;
                level.UpdatedOn = DateTime.UtcNow;

                _unitOfWork.GetRepository<Level>().Update(level);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to delete level.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("DeleteLevelError"));
            }
        }

        /// <summary>
        /// Handle to get levels
        /// </summary>
        /// <returns> the list of <see cref="Level" />.</returns>
        public async Task<IList<Level>> GetLevelsAsync()
        {
            try
            {
                var levels = await _unitOfWork
                    .GetRepository<Level>()
                    .GetAllAsync(predicate: x => x.IsActive)
                    .ConfigureAwait(false);
                return levels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch levels.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("FetchLevelError"));
            }
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Level, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }
    }
}
