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
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.Extensions.Logging;
    public class LevelService : BaseGenericService<Level, BaseSearchCriteria>, ILevelService
    {
        public LevelService(IUnitOfWork unitOfWork, ILogger<LevelService> logger) : base(unitOfWork, logger)
        {
        }

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
                var slug = CommonHelper.GetEntityTitleSlug<Level>(_unitOfWork, (slug) => q => q.Slug == slug, name);
                var tag = await _unitOfWork.GetRepository<Level>().GetFirstOrDefaultAsync(predicate: x => x.Name.ToLower() == name.ToLower()
                          && x.IsActive).ConfigureAwait(false);
                if (tag != default)
                {
                    throw new ArgumentException("Tag already exist");
                }
                var entity = new Level()
                {
                    Id = Guid.NewGuid(),
                    Slug = slug,
                    Name = name,
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
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
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
                var level = await _unitOfWork.GetRepository<Level>().GetFirstOrDefaultAsync(predicate: x => x.Id.ToString() == identity ||
                x.Slug.Equals(identity)).ConfigureAwait(false);
                if (level == default)
                {
                    throw new EntityNotFoundException("Tag not found");
                }

                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: x => x.Id == currentUserId).ConfigureAwait(false);
                if (user == null)
                {
                    throw new EntityNotFoundException("User not found");
                }


                level.IsActive = false;
                level.UpdatedBy = currentUserId;
                level.UpdatedOn = DateTime.UtcNow;

                _unitOfWork.GetRepository<Level>().Update(level);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Handle to get levels
        /// </summary>
        /// <returns> the list of <see cref="Level" />.</returns>
        public Task<IList<Level>> GetLevelsAsync()
        {
            throw new NotImplementedException();
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