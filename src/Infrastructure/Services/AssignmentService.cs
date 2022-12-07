namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Common;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using System.Linq.Expressions;

    public class AssignmentService : BaseGenericService<Assignment, AssignmentBaseSearchCriteria>, IAssignmentService
    {
        public AssignmentService(
            IUnitOfWork unitOfWork,
            ILogger<AssignmentService> logger) : base(unitOfWork, logger)
        {
        }

        #region Protected Region

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Assignment, bool>> ConstructQueryConditions(Expression<Func<Assignment, bool>> predicate, AssignmentBaseSearchCriteria criteria)
        {
            var lesson = _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: p => p.Id.ToString() == criteria.LessonIdentity || p.Slug == criteria.LessonIdentity).Result;

            if (lesson == null)
            {
                _logger.LogWarning("Lesson with identity : {identity} not found for user with id : {id}", criteria.LessonIdentity, criteria.CurrentUserId);
                throw new EntityNotFoundException("Lesson not found.");
            }

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Name.ToLower().Trim().Contains(search));
            }

            return predicate.And(x => x.LessonId == lesson.Id);
        }
        /// <summary>
        /// Check the validations required for delete
        /// </summary>
        /// <param name="entity">the instance of <see cref="Assignment"/></param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        protected override async Task CheckDeletePermissionsAsync(Assignment entity, Guid currentUserId)
        {
            await ValidateAndGetLessonForAssignment(entity).ConfigureAwait(false);

            if (entity.Type == QuestionTypeEnum.MultipleChoice || entity.Type == QuestionTypeEnum.SingleChoice)
            {
                var assignmentSubmissions = await _unitOfWork.GetRepository<AssignmentSubmission>().ExistsAsync(
                    predicate: p => p.AssignmentId == entity.Id).ConfigureAwait(false);
                if (assignmentSubmissions)
                {
                    _logger.LogWarning("Assignment with id : {id} having type : {type} contains assignment submissions", entity.Id, entity.Type);
                    throw new ForbiddenException("Assignment contains assignment submissions");
                }
            }

            if (entity.Type == QuestionTypeEnum.Subjective)
            {
                //TO-DO : Task Check Assignment Submission or not
            }

            _unitOfWork.GetRepository<AssignmentAttachment>().Delete(entity.AssignmentAttachments);
            _unitOfWork.GetRepository<AssignmentQuestionOption>().Delete(entity.AssignmentQuestionOptions);
        }

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Assignment, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity;
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
            await ValidateAndGetLessonForAssignment(entity).ConfigureAwait(false);

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

        /// <summary>
        /// Handel to populate live session retrieved entity
        /// </summary>
        /// <param name="entity">the instance of <see cref="LiveSession"/></param>
        /// <returns></returns>
        protected override async Task PopulateRetrievedEntity(Assignment entity)
        {
            entity.AssignmentAttachments = await _unitOfWork.GetRepository<AssignmentAttachment>().GetAllAsync(predicate: p => p.AssignmentId == entity.Id).ConfigureAwait(false);
            entity.AssignmentQuestionOptions = await _unitOfWork.GetRepository<AssignmentQuestionOption>().GetAllAsync(predicate: p => p.AssignmentId == entity.Id).ConfigureAwait(false);
        }

        #endregion Protected Region

        #region Private Region

        /// <summary>
        /// Handle to validate and get lesson for assignment
        /// </summary>
        /// <param name="entity">the instance of <see cref="Assignment"/></param>
        /// <returns>the instance of <see cref="Lesson"/></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private async Task<Lesson> ValidateAndGetLessonForAssignment(Assignment entity)
        {
            var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                            predicate: p => p.Id == entity.LessonId && !p.IsDeleted).ConfigureAwait(false);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson with id : {lessonId} not found for assignment with id : {id}", entity.LessonId, entity.Id);
                throw new EntityNotFoundException("Lesson not found");
            }
            if (lesson.Type != LessonType.Assignment)
            {
                _logger.LogWarning("Lesson with id : {lessonId} is of invalid lesson type to create,edit or delete assignment for user with id :{userId}", lesson.Id, entity.CreatedBy);
                throw new ArgumentException("Invalid lesson type for assignment.");
            }
            await ValidateAndGetCourse(entity.CreatedBy, lesson.CourseId.ToString(), validateForModify: true).ConfigureAwait(false);
            return lesson;
        }

        #endregion Private Region

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the assignment id or slug</param>
        /// <param name="model">the instance of <see cref="AssignmentRequestModel"/> </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        public async Task<Assignment> UpdateAsync(string identity, AssignmentRequestModel model, Guid currentUserId)
        {
            try
            {
                var existing = await GetByIdOrSlugAsync(identity, currentUserId).ConfigureAwait(false);
                var currentTimeStamp = DateTime.UtcNow;

                existing.Id = existing.Id;
                existing.Name = model.Name;
                existing.Hints = model.Hints;
                existing.LessonId = model.LessonId;
                existing.Type = model.Type;
                existing.Description = model.Description;
                existing.UpdatedBy = currentUserId;
                existing.UpdatedOn = currentTimeStamp;

                var assignmentQuestionOptions = new List<AssignmentQuestionOption>();
                var assignmentAttachments = new List<AssignmentAttachment>();

                if (model.Type == QuestionTypeEnum.SingleChoice || model.Type == QuestionTypeEnum.MultipleChoice)
                {
                    foreach (var item in model.Answers.Select((answer, i) => new { i, answer }))
                    {
                        assignmentQuestionOptions.Add(new AssignmentQuestionOption
                        {
                            Id = Guid.NewGuid(),
                            AssignmentId = existing.Id,
                            Order = item.i + 1,
                            Option = item.answer.Option,
                            IsCorrect = item.answer.IsCorrect,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp,
                        });
                    }
                }
                if ((model.Type == QuestionTypeEnum.SingleChoice || model.Type == QuestionTypeEnum.MultipleChoice) && model.FileUrls.Count > 0)
                {
                    foreach (var item in model.FileUrls.Select((fileUrl, i) => new { i, fileUrl }))
                    {
                        assignmentAttachments.Add(new AssignmentAttachment
                        {
                            Id = Guid.NewGuid(),
                            AssignmentId = existing.Id,
                            FileUrl = item.fileUrl,
                            Order = item.i + 1,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = currentUserId,
                            UpdatedOn = currentTimeStamp,
                        });
                    }
                }
                if (existing.AssignmentAttachments.Count > 0)
                {
                    _unitOfWork.GetRepository<AssignmentAttachment>().Delete(existing.AssignmentAttachments);
                }
                if (existing.AssignmentQuestionOptions.Count > 0)
                {
                    _unitOfWork.GetRepository<AssignmentQuestionOption>().Delete(existing.AssignmentQuestionOptions);
                }
                if (assignmentAttachments.Count > 0)
                {
                    await _unitOfWork.GetRepository<AssignmentAttachment>().InsertAsync(assignmentAttachments).ConfigureAwait(false);
                }
                if (assignmentQuestionOptions.Count > 0)
                {
                    await _unitOfWork.GetRepository<AssignmentQuestionOption>().InsertAsync(assignmentQuestionOptions).ConfigureAwait(false);
                }
                _unitOfWork.GetRepository<Assignment>().Update(existing);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to update assignment.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to update assignment.");
            }
        }

        /// <summary>
        /// Handle to submit assignments by the user
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="models">the list of <see cref="AssignmentSubmissionRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        public async Task AssignmentSubmissionAsync(string lessonIdentity, IList<AssignmentSubmissionRequestModel> models, Guid currentUserId)
        {
            try
            {
                var lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                    predicate: p => p.Id.ToString() == lessonIdentity || p.Slug == lessonIdentity).ConfigureAwait(false);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with identity: {identity} not found for user with id: {id}", lessonIdentity, currentUserId);
                    throw new EntityNotFoundException("Lesson not found");
                }
                if (lesson.Status != CourseStatus.Published)
                {
                    _logger.LogWarning("Lesson with id: {id} not published for user with id: {id}", lesson.Id, currentUserId);
                    throw new EntityNotFoundException("Lesson not published");
                }
                await ValidateAndGetCourse(currentUserId, lesson.CourseId.ToString()).ConfigureAwait(false);

                if (lesson.Type != LessonType.Assignment)
                {
                    _logger.LogWarning("Lesson type not matched for assignment submission for lesson with id: {id} and user with id: {userId}",
                                        lesson.Id, currentUserId);
                    throw new ForbiddenException($"Invalid lesson type :{lesson.Type}");
                }

                var assignments = await _unitOfWork.GetRepository<Assignment>().GetAllAsync(
                    predicate: p => p.LessonId == lesson.Id && p.IsActive,
                    include: src => src.Include(x => x.AssignmentQuestionOptions)).ConfigureAwait(false);

                var assignmentSubmissions = new List<AssignmentSubmission>();
                foreach (var item in models)
                {
                    await InsertAssignmentSubmission(currentUserId, assignments, item).ConfigureAwait(false);
                }
                await _unitOfWork.GetRepository<AssignmentSubmission>().InsertAsync(assignmentSubmissions).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to submit the assignment.");
                throw ex is ServiceException ? ex : new ServiceException("An error occurred while trying to submit the assignment.");
            }
        }

        /// <summary>
        /// Handle to insert assignment submission and attachment details
        /// </summary>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <param name="assignments">the list of <see cref="Assignment"/></param>
        /// <param name="item">the instance of <see cref="AssignmentSubmissionRequestModel"/></param>
        /// <returns></returns>
        private async Task InsertAssignmentSubmission(Guid currentUserId, IList<Assignment> assignments, AssignmentSubmissionRequestModel item)
        {
            var currentTimeStamp = DateTime.UtcNow;
            var assignment = assignments.FirstOrDefault(x => x.Id == item.AssignmentId);
            if (assignment != null)
            {
                var assignmentSubmission = new AssignmentSubmission
                {
                    Id = Guid.NewGuid(),
                    AssignmentId = assignment.Id,
                    UserId = currentUserId,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                    AssignmentSubmissionAttachments = new List<AssignmentSubmissionAttachment>(),
                };
                if (assignment.Type == QuestionTypeEnum.SingleChoice || assignment.Type == QuestionTypeEnum.MultipleChoice)
                {
                    var answerIds = assignment.AssignmentQuestionOptions?.Where(x => x.IsCorrect).Select(x => x.Id);
                    bool? isCorrect = answerIds?.OrderBy(x => x).ToList().SequenceEqual(item.SelectedOption.OrderBy(x => x).ToList());

                    assignmentSubmission.IsCorrect = isCorrect ?? false;
                    assignmentSubmission.SelectedOption = string.Join(",", item.SelectedOption);
                }
                if (assignment.Type == QuestionTypeEnum.Subjective)
                {
                    item.AttachmentUrls.ForEach(attachment => assignmentSubmission.AssignmentSubmissionAttachments.Add(new AssignmentSubmissionAttachment
                    {
                        Id = Guid.NewGuid(),
                        AssignmentSubmissionId = assignmentSubmission.Id,
                        FileUrl = attachment,
                        CreatedBy = currentUserId,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = currentUserId,
                        UpdatedOn = currentTimeStamp,
                    }));

                    await _unitOfWork.GetRepository<AssignmentSubmissionAttachment>().InsertAsync(assignmentSubmission.AssignmentSubmissionAttachments).ConfigureAwait(false);
                }
            }
        }
    }
}
