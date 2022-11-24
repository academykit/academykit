namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq.Expressions;

    public class AssignmentService : BaseGenericService<Assignment, BaseSearchCriteria>, IAssignmentService
    {
        public AssignmentService(
            IUnitOfWork unitOfWork,
            ILogger<AssignmentService> logger) : base(unitOfWork, logger)
        {

        }

        #region Protected Region
        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Assignment, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity.ToString();
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Assignment, object> IncludeNavigationProperties(IQueryable<Assignment> query)
        {
            return query.Include(x => x.User);
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Assignment entity)
        {
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: p => p.Id == entity.LessonId && !p.IsDeleted).ConfigureAwait(false);
            if(lesson == null)
            {
                _logger.LogWarning("Lesson with id : {0} not found for assigment with id : {1}",entity.LessonId,entity.Id);
                throw new EntityNotFoundException("Lesson not found");
            }
            if(lesson.Type != LessonType.Assignment)
            {
                _logger.LogWarning("Lesson with id : {0} is of invalid lesson type to create assignment for user with id :{1}",lesson.Id,entity.CreatedBy);
                throw new ArgumentException("Invalid lesson type for assignment.");
            }
            await ValidateAndGetCourse(entity.CreatedBy, lesson.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);

            if (entity.AssignmentQuestionOptions.Count > 0)
            {
                await _unitOfWork.GetRepository<AssignmentQuestionOption>().InsertAsync(entity.AssignmentQuestionOptions).ConfigureAwait(false);
            }
            if (entity.AssignmentAttachments.Count > 0)
            {
                await _unitOfWork.GetRepository<AssignmentAttachment>().InsertAsync(entity.AssignmentAttachments).ConfigureAwait(false);
            }
            await Task.FromResult(0);
        }
        #endregion Protected Region
    }
}
