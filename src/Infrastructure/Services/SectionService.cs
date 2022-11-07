namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Helpers;
    using Microsoft.Extensions.Logging;

    public class SectionService : BaseGenericService<Section, BaseSearchCriteria>, ISectionService
    {
        public SectionService(IUnitOfWork unitOfWork, ILogger<SectionService> logger) : base(unitOfWork, logger)
        {
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Section entity)
        {
            await CheckCourseTeacherAsync(entity).ConfigureAwait(false);
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
            await CheckCourseTeacherAsync(newEntity).ConfigureAwait(false);
            await CheckDuplicateNameAsync(newEntity).ConfigureAwait(false);
            _unitOfWork.GetRepository<Section>().Update(newEntity);
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
                predicate: p => p.Id != entity.Id && p.Name.ToLower() == entity.Name.ToLower()).ConfigureAwait(false);
            if (sectionExist)
            {
                _logger.LogWarning("Duplicate department name : {name} is found for the department with id : {id}", entity.Name, entity.Id);
                throw new ServiceException("Duplicate department name is found");
            }
        }

        /// <summary>
        /// Handle to check the course teacher
        /// </summary>
        /// <param name="entity"> the instance of <see cref="Section" /> .</param>
        /// <returns> the task complete ˝</returns>
        private async Task CheckCourseTeacherAsync(Section entity)
        {
            var teacher = await _unitOfWork.GetRepository<CourseTeacher>().GetFirstOrDefaultAsync(predicate: x => x.CourseId == entity.CourseId &&
                            x.UserId == entity.CreatedBy).ConfigureAwait(false);
            if (teacher == null)
            {
                _logger.LogWarning("Unauthroized user : {0} is not teacher of course {1}", entity.CreatedBy, entity.CourseId);
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
            var section = await _unitOfWork.GetRepository<Section>().GetFirstOrDefaultAsync(predicate: x => !x.IsDeleted, orderBy: x => x.OrderByDescending(x => x.Order)).ConfigureAwait(false);
            return section != null ? section.Order++ : 1;
        }

        #endregion Private Methods
    }
}