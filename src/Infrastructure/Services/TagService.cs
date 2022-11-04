namespace Lingtren.Infrastructure.Services
{
    using System;
    using System.Linq.Expressions;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.Extensions.Logging;

    public class TagService : BaseGenericService<Tag, BaseSearchCriteria>, ITagService
    {
        public TagService(IUnitOfWork unitOfWork, ILogger<TagService> logger)
        : base(unitOfWork, logger)
        {

        }


        /// <summary>
        /// Handle to create the tag
        /// </summary>
        /// <param name="name"> the tag name </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="Tag" /> .</returns>
        public async Task<Tag> CreateTagAsync(string name, Guid currentUserId)
        {
            try
            {
                var slug = CommonHelper.GetEntityTitleSlug<Tag>(_unitOfWork, (slug) => q => q.Slug == slug, name);
                var tag = await _unitOfWork.GetRepository<Tag>().GetFirstOrDefaultAsync(predicate: x => x.Name.ToLower() == name.ToLower()
                          && x.IsActive).ConfigureAwait(false);
                if (tag != default)
                {
                    throw new ArgumentException("Tag already exist");
                }

                var entity = new Tag()
                {
                    Id = Guid.NewGuid(),
                    Slug = slug,
                    Name = name,
                    IsActive = true,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Tag>().InsertAsync(entity).ConfigureAwait(false);
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
        /// Handle to delete the tag 
        /// </summary>
        /// <param name="identity"> the slug or id </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task DeleteTagAsync(string identity, Guid currentUserId)
        {
            try
            {
                var tag = await _unitOfWork.GetRepository<Tag>().GetFirstOrDefaultAsync(predicate: x => x.Id.ToString() == identity ||
                x.Slug.Equals(identity)).ConfigureAwait(false);
                if (tag == default)
                {
                    throw new EntityNotFoundException("Tag not found");
                }

                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: x => x.Id == currentUserId).ConfigureAwait(false);
                if (user == null)
                {
                    throw new EntityNotFoundException("User not found");
                }

                var access = ValidateUser(user, tag);
                if (!access)
                {
                    throw new ForbiddenException("Unauthorized user");
                }

                // to do check tag exist on other services 

                tag.IsActive = false;
                tag.UpdatedBy = currentUserId; tag.UpdatedOn = DateTime.UtcNow;

                _unitOfWork.GetRepository<Tag>().Update(tag);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex is ServiceException ? ex : new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Tag, bool>> ConstructQueryConditions(Expression<Func<Tag, bool>> predicate, BaseSearchCriteria criteria)
        {

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search)
                 || x.User.FirstName.ToLower().Trim().Contains(search));
            }
            return predicate.And(x => x.IsActive);
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Tag entity)
        {
            entity.Slug = CommonHelper.GetEntityTitleSlug<Tag>(_unitOfWork, (slug) => q => q.Slug == slug, entity.Name);
            await Task.FromResult(0);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Tag, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }

        /// <summary>
        /// Handle to validate the user 
        /// </summary>
        /// <param name="user"> the instance of <see cref="User"/></param>
        /// <param name="tag"> the instance of <see cref="Tag"/></param>
        /// <returns> the boolean value </returns>
        private bool ValidateUser(User user, Tag tag)
        {
            if (tag.CreatedBy == user.Id)
            {
                return true;
            }

            if (user.Role == UserRole.Admin)
            {
                return true;
            }

            return false;
        }
    }
}