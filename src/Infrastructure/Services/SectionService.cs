namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;

    public class SectionService : BaseGenericService<Section, SectionBaseSearchCriteria>, ISectionService
    {
        public SectionService(IUnitOfWork unitOfWork, ILogger<SectionService> logger) : base(unitOfWork, logger)
        {
        }

        #region Protected Region

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Section, bool>> ConstructQueryConditions(Expression<Func<Section, bool>> predicate, SectionBaseSearchCriteria criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
            }
            return predicate.And(p => !p.IsDeleted && p.CourseId == criteria.CourseId);
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Section, object> IncludeNavigationProperties(IQueryable<Section> query)
        {
            return query.Include(x => x.User);
        }
        /// <summary>
        /// Handel to populate live session retrieved entity
        /// </summary>
        /// <param name="entity">the instance of <see cref="LiveSession"/></param>
        /// <returns></returns>
        protected override async Task PopulateRetrievedEntity(Section entity)
        {
            var lessons = await _unitOfWork.GetRepository<Lesson>().GetAllAsync(predicate: p => p.SectionId == entity.Id).ConfigureAwait(false);
            entity.Lessons = lessons;
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Section entity)
        {
            await CheckCourseTeacherAsync(entity, entity.CreatedBy).ConfigureAwait(false);
            await CheckDuplicateNameAsync(entity).ConfigureAwait(false);
            var order = await LastSectionOrder(entity).ConfigureAwait(false);
            entity.Order = order;
            entity.Slug = CommonHelper.GetEntityTitleSlug<Section>(_unitOfWork, (slug) => q => q.Slug == slug, entity.Name);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Updates the <paramref name="existing"/> entity according to <paramref name="newEntity"/> entity.
        /// </summary>
        /// <remarks>Override in child services to update navigation properties.</remarks>
        /// <param name="existing">The existing entity.</param>
        /// <param name="newEntity">The new entity.</param>
        protected override async Task UpdateEntityFieldsAsync(Section existing, Section newEntity)
        {
            await CheckCourseTeacherAsync(newEntity, newEntity.UpdatedBy.Value).ConfigureAwait(false);
            await CheckDuplicateNameAsync(newEntity).ConfigureAwait(false);
            _unitOfWork.GetRepository<Section>().Update(newEntity);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Section, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }

        #endregion Protected Region

        /// <summary>
        /// Handle to delete the section
        /// </summary>
        /// <param name="identity"> the course id or slug </param>
        /// <param name="sectionIdentity"> the section id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task DeleteSectionAsync(string identity, string sectionIdentity, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity).ConfigureAwait(false);
                var section = await _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(
                    predicate: x => !x.IsDeleted && x.CourseId == course.Id && (x.Id.ToString() == sectionIdentity || x.Slug.Equals(sectionIdentity)),
                    include: s => s.Include(x => x.Lessons).Include(x => x.Course)).ConfigureAwait(false);
                if (section == null)
                {
                    throw new EntityNotFoundException("Section not found");
                }
                await CheckCourseTeacherAsync(section, currentUserId).ConfigureAwait(false);
                if (section.Status == CourseStatus.Published || section.Course.Status == CourseStatus.Published)
                {
                    throw new ForbiddenException("Course section is published.");
                }

                if (section.Lessons.Any(x => !x.IsDeleted))
                {
                    throw new ArgumentException("Course section consist lessons");
                }

                section.IsDeleted = true;
                section.UpdatedBy = currentUserId;
                section.UpdatedOn = DateTime.UtcNow;

                _unitOfWork.GetRepository<Section>().Update(section);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to delete section.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to delete section.");
            }
        }

        /// <summary>
        /// Handle to reorder section
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="ids">the section ids</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task ReorderAsync(string identity, IList<Guid> ids, Guid currentUserId)
        {
            try
            {
                var course = await ValidateAndGetCourse(currentUserId, identity, validateForModify: true).ConfigureAwait(false);
                if (course == null)
                {
                    _logger.LogWarning("ReorderAsync(): Course with identity : {identity} not found for user with id :{userId}.", identity, currentUserId);
                    throw new EntityNotFoundException("Course was not found");
                }

                var sections = await _unitOfWork.GetRepository<Section>().GetAllAsync(
                    predicate: p => p.CourseId == course.Id && ids.Contains(p.Id)
                    ).ConfigureAwait(false);

                var order = 1;
                var currentTimeStamp = DateTime.UtcNow;
                var updateEntities = new List<Section>();
                foreach (var id in ids)
                {
                    var section = sections.FirstOrDefault(x => x.Id == id);
                    if (section != null)
                    {
                        section.Order = order;
                        section.UpdatedBy = currentUserId;
                        section.UpdatedOn = currentTimeStamp;
                        updateEntities.Add(section);
                        order++;
                    }
                }
                if (updateEntities.Count > 0)
                {
                    _unitOfWork.GetRepository<Section>().Update(updateEntities);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to reorder the lessons");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while attempting to reorder the lessons");
            }
        }

        #region Private Methods
        /// <summary>
        /// Check duplicate name
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        private async Task CheckDuplicateNameAsync(Section entity)
        {
            var sectionExist = await _unitOfWork.GetRepository<Section>().ExistsAsync(
                predicate: p => p.Id != entity.Id && p.CourseId == entity.CourseId && p.Name.ToLower() == entity.Name.ToLower() && !p.IsDeleted).ConfigureAwait(false);
            if (sectionExist)
            {
                _logger.LogWarning("Duplicate section name : {name} is found for the section with id : {id}", entity.Name, entity.Id);
                throw new ServiceException("Duplicate section name is found");
            }
        }

        /// <summary>
        /// Handle to check the course teacher
        /// </summary>
        /// <param name="entity"> the instance of <see cref="Section" /> .</param>
        /// <returns> the task complete ˝</returns>
        private async Task CheckCourseTeacherAsync(Section entity, Guid userId)
        {
            var teacher = await _unitOfWork.GetRepository<CourseTeacher>().GetFirstOrDefaultAsync(predicate: x => x.CourseId == entity.CourseId &&
                            x.UserId == userId).ConfigureAwait(false);
            if (teacher == null)
            {
                _logger.LogWarning("Unauthorized user with id : {userId} is not teacher of course with id :{courseId}", entity.CreatedBy, entity.CourseId);
                throw new ForbiddenException("Unauthorized user");
            }
        }

        /// <summary>
        /// Handle to get last order number
        /// </summary>
        /// <param name="entity"> the instance of <see cref="Section" /> .</param>
        /// <returns> the int value </returns>
        private async Task<int> LastSectionOrder(Section entity)
        {
            var section = await _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == entity.CourseId && !x.IsDeleted,
                orderBy: x => x.OrderByDescending(x => x.Order)).ConfigureAwait(false);
            return section != null ? section.Order + 1 : 1;
        }

        #endregion Private Methods
    }
}