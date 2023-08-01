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
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class TagService : BaseGenericService<Tag, TagBaseSearchCriteria>, ITagService
    {
        public TagService(IUnitOfWork unitOfWork, ILogger<TagService> logger,
        IStringLocalizer<ExceptionLocalizer> localizer)
        : base(unitOfWork, logger, localizer)
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
                var tagName = name.TrimStart().TrimEnd();
                var slug = CommonHelper.GetEntityTitleSlug<Tag>(_unitOfWork, (slug) => q => q.Slug == slug, tagName);
                var tag = await _unitOfWork.GetRepository<Tag>().GetFirstOrDefaultAsync(predicate: x => x.Name.ToLower() == tagName.ToLower()
                          && x.IsActive).ConfigureAwait(false);
                if (tag != default)
                {
                    return tag;
                }

                var entity = new Tag()
                {
                    Id = Guid.NewGuid(),
                    Slug = slug,
                    Name = tagName,
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
                _logger.LogError(ex, "An error occurred while trying to create tag.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("CreateTagError"));
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
                    throw new EntityNotFoundException(_localizer.GetString("TagNotFound"));
                }

                var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: x => x.Id == currentUserId).ConfigureAwait(false);
                if (user == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("UserNotFound"));
                }

                var access = ValidateUser(user, tag);
                if (!access)
                {
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                // to do check tag exist on other services 

                tag.IsActive = false;
                tag.UpdatedBy = currentUserId; tag.UpdatedOn = DateTime.UtcNow;

                _unitOfWork.GetRepository<Tag>().Update(tag);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to delete tag.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("DeleteTagError"));
            }
        }

        /// <summary>
        /// Handle to update the tag
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="name"> the tag name </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="Tag" />.</returns>
        public async Task<Tag> UpdateTagAsync(string identity, string name, Guid currentUserId)
        {
            try
            {
                var tagName = name.TrimStart().TrimEnd();
                var tags = await _unitOfWork.GetRepository<Tag>().GetAllAsync(predicate: x => x.IsActive).ConfigureAwait(false);

                var tag = tags.FirstOrDefault(x => x.Id.ToString() == identity || x.Slug.Equals(identity));
                if (tag == null)
                {
                    throw new EntityNotFoundException(_localizer.GetString("TagNotFound"));
                }

                var tagNameExist = tags.Any(x => x.Id != tag.Id && x.Name.ToLower() == tagName.ToLower());
                if (tagNameExist)
                {
                    throw new ForbiddenException(_localizer.GetString("TagAlreadyExist"));
                }

                tag.Name = tagName;
                tag.UpdatedBy = currentUserId;
                tag.UpdatedOn = DateTime.UtcNow;
                _unitOfWork.GetRepository<Tag>().Update(tag);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return tag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update tag.");
                throw ex is ServiceException ? ex : new ServiceException(_localizer.GetString("UpdateTagError"));
            }
        }

        #region Protected Region

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Tag, bool>> ConstructQueryConditions(Expression<Func<Tag, bool>> predicate,TagBaseSearchCriteria criteria)
        {
            
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search)
                 || x.User.FirstName.ToLower().Trim().Contains(search));
            }
            if(!string.IsNullOrWhiteSpace(criteria.Idenitiy))
            {
                switch(criteria.TrainingType)
                {
                    case TrainingTypeEnum.Course:
                        predicate = predicate.And(x => x.CourseTags.Any(x => x.Course.Id.ToString() == criteria.Idenitiy.Trim() || x.Course.Slug.ToLower() == criteria.Idenitiy.ToLower().Trim()));
                        break;
                    case TrainingTypeEnum.QuestionPool:
                        var questionTags =_unitOfWork.GetRepository<QuestionTag>().GetAll(predicate : p=>p.Question.QuestionPoolQuestions.Any(x=>x.QuestionPool.Slug.ToLower() == criteria.Idenitiy.ToLower().Trim() || 
                        x.QuestionPool.Id.ToString() == criteria.Idenitiy.ToString().Trim())).ToList();
                        predicate = predicate.And(x => questionTags.Select(x => x.TagId).Contains(x.Id));
                        break;
                }
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

        #endregion
        /// <summary>
        /// Handle to validate the user
        /// </summary>
        /// <param name="user"> the instance of <see cref="User"/></param>
        /// <param name="tag"> the instance of <see cref="Tag"/></param>
        /// <returns> the boolean value </returns>
        private static bool ValidateUser(User user, Tag tag)
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
