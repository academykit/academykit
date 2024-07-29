namespace AcademyKit.Infrastructure.Services
{
    using System.Linq.Expressions;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Helpers;
    using AcademyKit.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class QuestionService
        : BaseGenericService<Question, QuestionBaseSearchCriteria>,
            IQuestionService
    {
        public QuestionService(
            IUnitOfWork unitOfWork,
            ILogger<QuestionService> logger,
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
        protected override Expression<Func<Question, bool>> ConstructQueryConditions(
            Expression<Func<Question, bool>> predicate,
            QuestionBaseSearchCriteria criteria
        )
        {
            var questionPool = _unitOfWork
                .GetRepository<QuestionPool>()
                .GetFirstOrDefaultAsync(predicate: p =>
                    p.Id.ToString() == criteria.PoolIdentity || p.Slug == criteria.PoolIdentity
                )
                .Result;

            predicate = predicate.And(p =>
                p.QuestionPoolQuestions.Any(x => x.QuestionPoolId == questionPool.Id)
            );

            if (criteria.Tags?.Count > 0)
            {
                predicate = predicate.And(p =>
                    p.QuestionTags.Any(x => criteria.Tags.Contains(x.TagId))
                );
            }

            if (criteria.Type.HasValue)
            {
                predicate = predicate.And(p => p.Type.Equals(criteria.Type.Value));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));

                predicate = predicate.And(x =>
                    x.Name.ToLower().Trim().Contains(search)
                    || (
                        (x.User.FirstName.Trim() + " " + x.User.MiddleName.Trim()).Trim()
                        + " "
                        + x.User.LastName.Trim()
                    )
                        .Trim()
                        .Contains(search)
                );
            }

            return predicate;
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Question, object> IncludeNavigationProperties(
            IQueryable<Question> query
        )
        {
            return query
                .Include(x => x.User)
                .Include(x => x.QuestionOptions)
                .Include(x => x.QuestionPoolQuestions)
                .Include(x => x.QuestionTags)
                .ThenInclude(x => x.Tag);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Question, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
        }
        #endregion Protected Region

        /// <summary>
        /// Handle to add question
        /// </summary>
        /// <param name="identity"> the question pool id or slug </param>
        /// <param name="question"> the instance of <see cref="QuestionRequestModel"/></param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        public async Task<Question> AddAsync(
            string identity,
            QuestionRequestModel question,
            Guid currentUserId
        )
        {
            try
            {
                var questionPool = await _unitOfWork
                    .GetRepository<QuestionPool>()
                    .GetFirstOrDefaultAsync(
                        predicate: x =>
                            (x.Id.ToString() == identity || x.Slug.Equals(identity))
                            && !x.IsDeleted,
                        include: src => src.Include(x => x.QuestionPoolTeachers)
                    )
                    .ConfigureAwait(false);

                if (questionPool == null)
                {
                    _logger.LogWarning(
                        "Question pool not found with identity: {identity}.",
                        identity.SanitizeForLogger()
                    );
                    throw new EntityNotFoundException(_localizer.GetString("QuestionPoolNotFound"));
                }

                if (
                    currentUserId != questionPool.CreatedBy
                    && !questionPool.QuestionPoolTeachers.Any(x => x.UserId == currentUserId)
                )
                {
                    _logger.LogWarning(
                        "User with id: {currentUserId} is not allowed to add question in the question pool with id: {questionPoolId}.",
                        currentUserId,
                        questionPool.Id
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("UnauthorizedUserAddQuestion")
                    );
                }

                var currentTimeStamp = DateTime.UtcNow;
                var entity = new Question()
                {
                    Id = Guid.NewGuid(),
                    Name = question.Name,
                    Description = question.Description,
                    Type = question.Type,
                    Hints = question.Hints,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    QuestionOptions = new List<QuestionOption>(),
                    QuestionTags = new List<QuestionTag>(),
                };
                foreach (var item in question.Tags)
                {
                    entity.QuestionTags.Add(
                        new QuestionTag
                        {
                            Id = Guid.NewGuid(),
                            TagId = item,
                            QuestionId = entity.Id,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp
                        }
                    );
                }

                var questionPoolQuestionCount = await _unitOfWork
                    .GetRepository<QuestionPoolQuestion>()
                    .CountAsync(predicate: p => p.QuestionPoolId == questionPool.Id)
                    .ConfigureAwait(false);
                foreach (var item in question.Answers.Select((answer, i) => new { i, answer }))
                {
                    entity.QuestionOptions.Add(
                        new QuestionOption
                        {
                            Id = Guid.NewGuid(),
                            QuestionId = entity.Id,
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

                var questionPoolQuestion = new QuestionPoolQuestion
                {
                    Id = Guid.NewGuid(),
                    QuestionId = entity.Id,
                    QuestionPoolId = questionPool.Id,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    Order = questionPoolQuestionCount
                };
                await _unitOfWork
                    .GetRepository<QuestionPoolQuestion>()
                    .InsertAsync(questionPoolQuestion)
                    .ConfigureAwait(false);
                await _unitOfWork
                    .GetRepository<QuestionTag>()
                    .InsertAsync(entity.QuestionTags)
                    .ConfigureAwait(false);
                await _unitOfWork
                    .GetRepository<QuestionOption>()
                    .InsertAsync(entity.QuestionOptions)
                    .ConfigureAwait(false);
                await _unitOfWork
                    .GetRepository<Question>()
                    .InsertAsync(entity)
                    .ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to create question.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("QuestionCreateError"));
            }
        }

        /// <summary>
        /// Handle to update question
        /// </summary>
        /// <param name="poolIdentity">the question pool id or slug</param>
        /// <param name="questionId">the question id </param>
        /// <param name="question">the instance of <see cref="QuestionRequestModel"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<Question> UpdateAsync(
            string poolIdentity,
            Guid questionId,
            QuestionRequestModel question,
            Guid currentUserId
        )
        {
            try
            {
                var questionPool = await _unitOfWork
                    .GetRepository<QuestionPool>()
                    .GetFirstOrDefaultAsync(
                        predicate: x =>
                            (x.Id.ToString() == poolIdentity || x.Slug.Equals(poolIdentity))
                            && !x.IsDeleted,
                        include: src => src.Include(x => x.QuestionPoolTeachers)
                    )
                    .ConfigureAwait(false);
                if (questionPool == null)
                {
                    _logger.LogWarning(
                        "Question pool not found with identity: {poolIdentity}.",
                        poolIdentity.SanitizeForLogger()
                    );
                    throw new EntityNotFoundException(_localizer.GetString("QuestionPoolNotFound"));
                }

                var existing = await _unitOfWork
                    .GetRepository<Question>()
                    .GetFirstOrDefaultAsync(
                        predicate: x => x.Id == questionId,
                        include: src =>
                            src.Include(x => x.QuestionOptions).Include(x => x.QuestionTags)
                    )
                    .ConfigureAwait(false);
                if (existing == null)
                {
                    _logger.LogWarning("Question not found with id: {questionId}.", questionId);
                    throw new EntityNotFoundException(_localizer.GetString("QuestionNotFound"));
                }

                if (existing.CreatedBy != currentUserId)
                {
                    _logger.LogWarning(
                        "User with id: {currentUserId} is not allowed to update question with id: {id}.",
                        currentUserId,
                        existing.Id
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("UnauthorizedUserUpdateQuestion")
                    );
                }

                var questionPoolQuestion = await _unitOfWork
                    .GetRepository<QuestionPoolQuestion>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.QuestionId == existing.Id && p.QuestionPoolId == questionPool.Id
                    )
                    .ConfigureAwait(false);
                if (questionPoolQuestion != null)
                {
                    var questionSetIds = await _unitOfWork
                        .GetRepository<QuestionSetQuestion>()
                        .GetAllAsync(
                            selector: s => s.QuestionSetId,
                            predicate: p =>
                                p.QuestionId == existing.Id
                                || p.QuestionPoolQuestionId == questionPoolQuestion.Id
                        )
                        .ConfigureAwait(false);

                    var existQuestionSetSubmissions = await _unitOfWork
                        .GetRepository<QuestionSetSubmission>()
                        .ExistsAsync(predicate: p => questionSetIds.Contains(p.QuestionSetId))
                        .ConfigureAwait(false);
                    if (existQuestionSetSubmissions)
                    {
                        _logger.LogWarning(
                            "Question with id: {id} cannot be edited such that it is associated with exam submission.",
                            existing.Id
                        );
                        throw new ForbiddenException(_localizer.GetString("QuestionCannotEdit"));
                    }

                    var existingquestionpool = await _unitOfWork
                        .GetRepository<QuestionPool>()
                        .GetFirstOrDefaultAsync(predicate: p => p.Id == question.QuestionPoolId)
                        .ConfigureAwait(false);
                    questionPoolQuestion.QuestionPool = existingquestionpool;
                    questionPoolQuestion.QuestionPoolId = question.QuestionPoolId;
                    _unitOfWork.GetRepository<QuestionPoolQuestion>().Update(questionPoolQuestion);
                    await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                }

                var currentTimeStamp = DateTime.UtcNow;

                existing.Id = existing.Id;
                existing.Name = question.Name;
                existing.Description = question.Description;
                existing.Type = question.Type;
                existing.Hints = question.Hints;
                existing.UpdatedBy = currentUserId;
                existing.UpdatedOn = currentTimeStamp;
                var questionOptions = new List<QuestionOption>();
                var questionTags = new List<QuestionTag>();

                foreach (var item in question.Answers.Select((answer, i) => new { i, answer }))
                {
                    questionOptions.Add(
                        new QuestionOption()
                        {
                            Id = Guid.NewGuid(),
                            QuestionId = existing.Id,
                            Order = item.i + 1,
                            Option = item.answer.Option,
                            IsCorrect = item.answer.IsCorrect,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp
                        }
                    );
                }

                foreach (var item in question.Tags)
                {
                    questionTags.Add(
                        new QuestionTag
                        {
                            Id = Guid.NewGuid(),
                            TagId = item,
                            QuestionId = existing.Id,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp
                        }
                    );
                }

                if (existing.QuestionOptions.Count > 0)
                {
                    _unitOfWork.GetRepository<QuestionOption>().Delete(existing.QuestionOptions);
                }

                if (existing.QuestionTags.Count > 0)
                {
                    _unitOfWork.GetRepository<QuestionTag>().Delete(existing.QuestionTags);
                }

                await _unitOfWork
                    .GetRepository<QuestionOption>()
                    .InsertAsync(questionOptions)
                    .ConfigureAwait(false);
                await _unitOfWork
                    .GetRepository<QuestionTag>()
                    .InsertAsync(questionTags)
                    .ConfigureAwait(false);
                _unitOfWork.GetRepository<Question>().Update(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while attempting to update question pool's question."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("QuestionPoolUpdateError"));
            }
        }

        /// <summary>
        /// Handle to delete question
        /// </summary>
        /// <param name="poolIdentity">the question pool id or slug</param>
        /// <param name="questionId">the question id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task DeleteQuestionAsync(
            string poolIdentity,
            Guid questionId,
            Guid currentUserId
        )
        {
            try
            {
                var questionPool = await _unitOfWork
                    .GetRepository<QuestionPool>()
                    .GetFirstOrDefaultAsync(predicate: x =>
                        (x.Id.ToString() == poolIdentity || x.Slug.Equals(poolIdentity))
                        && !x.IsDeleted
                    )
                    .ConfigureAwait(false);
                if (questionPool == null)
                {
                    _logger.LogWarning(
                        "Question pool not found with identity: {poolIdentity}.",
                        poolIdentity.SanitizeForLogger()
                    );
                    throw new EntityNotFoundException(_localizer.GetString("QuestionPoolNotFound"));
                }

                var existing = await _unitOfWork
                    .GetRepository<Question>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id == questionId,
                        include: src =>
                            src.Include(x => x.QuestionOptions).Include(x => x.QuestionTags)
                    )
                    .ConfigureAwait(false);
                if (existing == null)
                {
                    _logger.LogWarning("Question not found with id: {id}.", questionId);
                    throw new EntityNotFoundException(_localizer.GetString("QuestionNotFound"));
                }

                if (existing.CreatedBy != currentUserId)
                {
                    _logger.LogWarning(
                        "User with id: {currentUserId} is not allowed to update question with id: {id}.",
                        currentUserId,
                        existing.Id
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("UnauthorizedUserDeleteQuestion")
                    );
                }

                var questionPoolQuestion = await _unitOfWork
                    .GetRepository<QuestionPoolQuestion>()
                    .GetFirstOrDefaultAsync(predicate: p =>
                        p.QuestionPoolId == questionPool.Id && p.QuestionId == existing.Id
                    )
                    .ConfigureAwait(false);

                var checkQuestionSetQuestionExist = await _unitOfWork
                    .GetRepository<QuestionSetQuestion>()
                    .ExistsAsync(predicate: p =>
                        p.QuestionPoolQuestionId == questionPoolQuestion.Id
                    )
                    .ConfigureAwait(false);

                if (checkQuestionSetQuestionExist)
                {
                    _logger.LogWarning(
                        "Question with id: {id} is associated with Question-Set-Questions.",
                        existing.Id
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("QuestionAssociatedQuestionSet")
                    );
                }

                _unitOfWork.GetRepository<QuestionPoolQuestion>().Delete(questionPoolQuestion);
                _unitOfWork.GetRepository<QuestionOption>().Delete(existing.QuestionOptions);
                _unitOfWork.GetRepository<QuestionTag>().Delete(existing.QuestionTags);
                _unitOfWork.GetRepository<Question>().Delete(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while attempting to delete question pool's question."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("QuestionPoolDeleteError"));
            }
        }

        public async Task<QuestionSetAddQuestionRequestModel> BulkAddQuestions(
            List<QuestionRequestModel> questions,
            Guid currentUserId,
            string identity
        )
        {
            var questionPool = await _unitOfWork
                .GetRepository<QuestionPool>()
                .GetFirstOrDefaultAsync(
                    predicate: x =>
                        (x.Id.ToString() == identity || x.Slug.Equals(identity)) && !x.IsDeleted,
                    include: src => src.Include(x => x.QuestionPoolTeachers)
                )
                .ConfigureAwait(false);

            if (questionPool == null)
            {
                _logger.LogWarning("Question pool not found with identity: {identity}.", identity);
                throw new EntityNotFoundException(_localizer.GetString("QuestionPoolNotFound"));
            }

            if (
                currentUserId != questionPool.CreatedBy
                && !questionPool.QuestionPoolTeachers.Any(x => x.UserId == currentUserId)
            )
            {
                _logger.LogWarning(
                    "User with id: {currentUserId} is not allowed to add question in the question pool with id: {questionPoolId}.",
                    currentUserId,
                    questionPool.Id
                );
                throw new ForbiddenException(_localizer.GetString("UnauthorizedUserAddQuestion"));
            }
            var questionPoolQuestionCount = await _unitOfWork
                .GetRepository<QuestionPoolQuestion>()
                .CountAsync(predicate: p => p.QuestionPoolId == questionPool.Id)
                .ConfigureAwait(false);

            var currentTimeStamp = DateTime.UtcNow;
            var entities = new List<Question>();
            var questionEntities = new List<QuestionPoolQuestion>();
            var QuestionPoolQuestionIds = new QuestionSetAddQuestionRequestModel();

            foreach (var question in questions)
            {
                var questionId = Guid.NewGuid();
                var entity = new Question()
                {
                    Id = questionId,
                    Name = question.Name,
                    Description = question.Description,
                    Type = question.Type,
                    Hints = question.Hints,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    QuestionOptions = question
                        .Answers.Select(
                            (answer, i) =>
                                new QuestionOption
                                {
                                    Id = Guid.NewGuid(),
                                    QuestionId = questionId,
                                    Order = i + 1,
                                    Option = answer.Option,
                                    IsCorrect = answer.IsCorrect,
                                    CreatedBy = currentUserId,
                                    CreatedOn = currentTimeStamp,
                                    UpdatedBy = currentUserId,
                                    UpdatedOn = currentTimeStamp,
                                }
                        )
                        .ToList(),
                };

                var questionPoolQuestion = new QuestionPoolQuestion
                {
                    Id = Guid.NewGuid(),
                    QuestionId = entity.Id,
                    QuestionPoolId = questionPool.Id,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    Order = questionPoolQuestionCount
                };
                entities.Add(entity);
                questionEntities.Add(questionPoolQuestion);
                QuestionPoolQuestionIds.QuestionPoolQuestionIds.Add(questionPool.Id);
            }

            await _unitOfWork
                .GetRepository<QuestionOption>()
                .InsertAsync(entities.SelectMany(e => e.QuestionOptions))
                .ConfigureAwait(false);
            await _unitOfWork.GetRepository<Question>().InsertAsync(entities).ConfigureAwait(false);
            await _unitOfWork
                .GetRepository<QuestionPoolQuestion>()
                .InsertAsync(questionEntities)
                .ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var responseModels = entities
                .Select(entity => new AddQuestionResponseModel
                {
                    Name = entity.Name,
                    Type = entity.Type,
                    Description = entity.Description,
                    Hints = entity.Hints,
                    Answers = entity
                        .QuestionOptions.Select(option => new QuestionOptionResponseModel
                        {
                            Option = option.Option,
                            IsCorrect = option.IsCorrect
                        })
                        .ToList()
                })
                .ToList();

            return QuestionPoolQuestionIds;
        }
    }
}
