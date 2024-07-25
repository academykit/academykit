namespace Lingtren.Infrastructure.Services
{
    using System.Linq.Expressions;
    using System.Text;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class AssessmentQuestionService
        : BaseGenericService<AssessmentQuestion, AssessmentQuestionBaseSearchCriteria>,
            IAssessmentQuestionService
    {
        public AssessmentQuestionService(
            IUnitOfWork unitOfWork,
            ILogger<AssessmentQuestionService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        #region Protected Region

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<AssessmentQuestion, bool>> ConstructQueryConditions(
            Expression<Func<AssessmentQuestion, bool>> predicate,
            AssessmentQuestionBaseSearchCriteria criteria
        )
        {
            if (criteria.AssessmentIdentity != null)
            {
                predicate = predicate.And(x => x.AssessmentId == criteria.AssessmentIdentity);
            }
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
            }

            return predicate;
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<AssessmentQuestion, bool>> PredicateForIdOrSlug(
            string identity
        )
        {
            return p => p.Id.ToString() == identity;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<
            AssessmentQuestion,
            object
        > IncludeNavigationProperties(IQueryable<AssessmentQuestion> query)
        {
            return query
                .Include(x => x.User)
                .Include(x => x.Assessment)
                .Include(x => x.AssessmentOptions);
        }

        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(AssessmentQuestion entity)
        {
            var order = await _unitOfWork
                .GetRepository<AssessmentQuestion>()
                .MaxAsync(
                    predicate: p => p.AssessmentId == entity.AssessmentId && p.IsActive,
                    selector: x => (int?)x.Order
                )
                .ConfigureAwait(false);

            entity.Order = order == null ? 1 : order.Value + 1;

            if (entity.AssessmentOptions.Count > 0)
            {
                await _unitOfWork
                    .GetRepository<AssessmentOptions>()
                    .InsertAsync(entity.AssessmentOptions)
                    .ConfigureAwait(false);
            }

            await Task.FromResult(0);
        }

        /// <summary>
        /// Handel to populate live session retrieved entity
        /// </summary>
        /// <param name="entity">the instance of <see cref="LiveSession"/></param>
        /// <returns></returns>
        protected override async Task PopulateRetrievedEntity(AssessmentQuestion entity)
        {
            entity.AssessmentOptions = await _unitOfWork
                .GetRepository<AssessmentOptions>()
                .GetAllAsync(predicate: p => p.AssessmentQuestionId == entity.Id)
                .ConfigureAwait(false);
        }

        #endregion Protected Region

        #region Private Region

        /// <summary>
        /// Handle to validate and get lesson for Feedback
        /// </summary>
        /// <param name="entity">the instance of <see cref="Feedback"/></param>
        /// <returns>the instance of <see cref="Lesson"/></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private async Task<AssessmentQuestion> ValidateAndGetLessonForFeedback(
            AssessmentQuestion entity
        )
        {
            var AssessmentQuestion = await _unitOfWork
                .GetRepository<AssessmentQuestion>()
                .GetFirstOrDefaultAsync(predicate: p => p.Id == entity.AssessmentId)
                .ConfigureAwait(false);
            if (AssessmentQuestion == null)
            {
                _logger.LogWarning(
                    "Lesson with id : {lessonId} not found for Feedback with id : {id}.",
                    entity.AssessmentId,
                    entity.Id
                );
                throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
            }
            return AssessmentQuestion;
        }

        #endregion Private Region

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the Feedback id or slug</param>
        /// <param name="model">the instance of <see cref="FeedbackRequestModel"/> </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<AssessmentQuestion> UpdateAsync(
            string identity,
            AssessmentQuestionRequestModel model,
            Guid currentUserId
        )
        {
            try
            {
                var existing = await GetByIdOrSlugAsync(identity, currentUserId)
                    .ConfigureAwait(false);
                var currentTimeStamp = DateTime.UtcNow;

                existing.Id = existing.Id;
                existing.Name = model.QuestionName;
                existing.AssessmentId = existing.AssessmentId;
                existing.Type = model.Type;
                existing.Hints = model.Hints;
                existing.Description = model.Description;
                existing.UpdatedBy = currentUserId;
                existing.UpdatedOn = currentTimeStamp;

                var assessmentQuestionOptions = new List<AssessmentOptions>();

                if (
                    model.Type == AssessmentTypeEnum.SingleChoice
                    || model.Type == AssessmentTypeEnum.MultipleChoice
                )
                {
                    foreach (
                        var item in model.assessmentQuestionOptions.Select(
                            (answer, i) => new { i, answer }
                        )
                    )
                    {
                        assessmentQuestionOptions.Add(
                            new AssessmentOptions
                            {
                                Id = Guid.NewGuid(),
                                AssessmentQuestionId = existing.Id,
                                Order = item.i + 1,
                                Option = item.answer.Option,
                                IsCorrect = item.answer.IsCorrect,
                                CreatedBy = currentUserId,
                                CreatedOn = currentTimeStamp,
                                UpdatedBy = currentUserId,
                                UpdatedOn = currentTimeStamp,
                            }
                        );
                    }
                }

                if (existing.AssessmentOptions.Count > 0)
                {
                    _unitOfWork
                        .GetRepository<AssessmentOptions>()
                        .Delete(existing.AssessmentOptions);
                }

                if (assessmentQuestionOptions.Count > 0)
                {
                    await _unitOfWork
                        .GetRepository<AssessmentOptions>()
                        .InsertAsync(assessmentQuestionOptions)
                        .ConfigureAwait(false);
                }

                _unitOfWork.GetRepository<AssessmentQuestion>().Update(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update feedback.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("UpdateFeedBackError"));
            }
        }

        public async Task<AssessmentExamResponseModel> GetExamQuestion(
            Assessment existingAssessment,
            Guid currentUserId
        )
        {
            var assessmentQuestions = await _unitOfWork
                .GetRepository<AssessmentQuestion>()
                .GetAllAsync(
                    predicate: p => p.AssessmentId == existingAssessment.Id,
                    include: src => src.Include(x => x.AssessmentOptions)
                )
                .ConfigureAwait(false);
            var assessmentSubmission = new AssessmentSubmission
            {
                Id = new Guid(),
                StartTime = DateTime.Now,
                UserId = currentUserId,
                AssessmentId = existingAssessment.Id,
                CreatedOn = DateTime.Now
            };

            await _unitOfWork
                .GetRepository<AssessmentSubmission>()
                .InsertAsync(assessmentSubmission);
            await _unitOfWork.SaveChangesAsync();

            var response = new AssessmentExamResponseModel();

            response.AssessmentId = existingAssessment.Id;
            response.AssessmentName = existingAssessment.Title;
            response.Duration = existingAssessment.Duration * 60;
            response.Description = existingAssessment.Description;
            response.StartDateTime = existingAssessment.StartDate;
            response.EndDateTime = existingAssessment.EndDate;
            response.Questions = new List<AssessmentExamQuestionResponseModel>();

            foreach (var item in assessmentQuestions)
            {
                var questionModel = new AssessmentExamQuestionResponseModel
                {
                    QuestionId = item.Id,
                    QuestionName = item.Name,
                    Order = item.Order,
                    Type = item.Type,
                    Description = item.Description,
                    Hints = item.Hints,
                    assessmentQuestionOptions = new List<AssessmentOptionsExamResponseModel>()
                };

                if (
                    item.Type == AssessmentTypeEnum.SingleChoice
                    || item.Type == AssessmentTypeEnum.MultipleChoice
                )
                {
                    // Add assessment options if applicable
                    var orderedOptions = item.AssessmentOptions.OrderBy(opt => opt.Order);
                    foreach (var assOpt in orderedOptions)
                    {
                        questionModel.assessmentQuestionOptions.Add(
                            new AssessmentOptionsExamResponseModel
                            {
                                OptionId = assOpt.Id,
                                Option = assOpt.Option,
                                Order = assOpt.Order,
                            }
                        );
                    }
                }

                response.Questions.Add(questionModel);
                response.Questions = response.Questions.OrderBy(q => q.Order).ToList();
            }
            return response;
        }
    }
}
