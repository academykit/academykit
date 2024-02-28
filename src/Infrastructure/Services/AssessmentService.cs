using System.Data;
using System.Linq.Expressions;
using System.Text;
using Hangfire;
using Infrastructure.Persistence.Migrations;
using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.Common.Models.ResponseModels;
using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;
using Lingtren.Infrastructure.Common;
using Lingtren.Infrastructure.Helpers;
using Lingtren.Infrastructure.Localization;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Esf;

namespace Lingtren.Infrastructure.Services
{
    public class AssessmentService
        : BaseGenericService<Assessment, AssessmentBaseSearchCriteria>,
            IAssessmentService
    {
        public AssessmentService(
            IUnitOfWork unitOfWork,
            ILogger<AssessmentService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        #region Protected Methods
        /// <summary>
        /// This is called before entity is saved to DB.
        /// </summary>
        /// <remarks>
        /// It should be overridden in child services to do other updates before entity is saved.
        /// </remarks>
        protected override async Task CreatePreHookAsync(Assessment entity)
        {
            entity.Slug = CommonHelper.GetEntityTitleSlug<Assessment>(
                _unitOfWork,
                (slug) => q => q.Slug == slug,
                entity.Title
            );
            await CheckDuplicateSkillsNameAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the <paramref name="existing"/> entity according to <paramref name="newEntity"/> entity.
        /// </summary>
        /// <remarks>Override in child services to update navigation properties.</remarks>
        /// <param name="existing">The existing entity.</param>
        /// <param name="newEntity">The new entity.</param>
        protected override async Task UpdateEntityFieldsAsync(
            Assessment existing,
            Assessment newEntity
        )
        {
            await CheckDuplicateSkillsNameAsync(newEntity).ConfigureAwait(false);
            _unitOfWork.GetRepository<Assessment>().Update(newEntity);
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected override IIncludableQueryable<Assessment, object> IncludeNavigationProperties(
            IQueryable<Assessment> query
        )
        {
            return query
                .Include(x => x.SkillsCriteria)
                .ThenInclude(x => x.SkillType)
                .Include(x => x.EligibilityCreations)
                .ThenInclude(x => x.Skills)
                .Include(x => x.EligibilityCreations)
                .ThenInclude(x => x.Department)
                .Include(x => x.EligibilityCreations)
                .ThenInclude(x => x.CompletedAssessment)
                .Include(x => x.EligibilityCreations)
                .ThenInclude(x => x.Course)
                .Include(x => x.EligibilityCreations)
                .ThenInclude(x => x.Group)
                .Include(x => x.User)
                .Include(x => x.AssessmentQuestions)
                .Include(x => x.AssessmentSubmissions);
        }

        // /// <summary>
        // /// Check if entity could be deleted
        // /// </summary>
        // /// <param name="entityToDelete">The entity being deleted</param>
        // protected override async Task CheckDeletePermissionsAsync(Skills entityToDelete, Guid CurrentUserId)
        // {
        //     var existSkill = await _unitOfWork
        //         .GetRepository<User>()
        //         .ExistsAsync(predicate: user => user.Skills.Any(skill => skill.Id == entityToDelete.Id))
        //         .ConfigureAwait(false);

        //     if (existSkill)
        //     {
        //         _logger.LogWarning(
        //             "Skills with id: {id} is assigned to User so it cannot be deleted for Skill with id: {Id}.",
        //             entityToDelete.Id,
        //             CurrentUserId
        //         );
        //         throw new ForbiddenException(_localizer.GetString("SkillCannotBeDeletedAssignedToUser"));
        //     }
        // }
        #endregion Protected Methods

        #region Private Methods
        /// <summary>
        /// Check duplicate name
        /// /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        private async Task CheckDuplicateSkillsNameAsync(Assessment entity)
        {
            var assessmentExist = await _unitOfWork
                .GetRepository<Assessment>()
                .ExistsAsync(
                    predicate: p => p.Id != entity.Id && p.Title.ToLower() == entity.Title.ToLower()
                )
                .ConfigureAwait(false);
            if (assessmentExist)
            {
                _logger.LogWarning(
                    "Duplicate Assessment name : {Title} is found for the Assessment with id : {id}.",
                    entity.Title,
                    entity.Id
                );
                throw new ServiceException(_localizer.GetString("DuplicateAssessmentFound"));
            }
        }

        #endregion Private Methods

        /// <summary>
        /// If entity needs to support the get by slug or id then has to override this method.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The expression to filter by slug or slug</returns>
        protected override Expression<Func<Assessment, bool>> PredicateForIdOrSlug(string identity)
        {
            return p => p.Id.ToString() == identity || p.Slug == identity;
        }

        /// <summary>
        /// /// Applies filters to the given query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated predicate with applied filters.</returns>
        protected override Expression<Func<Assessment, bool>> ConstructQueryConditions(
            Expression<Func<Assessment, bool>> predicate,
            AssessmentBaseSearchCriteria criteria
        )
        {
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.ToLower().Trim();
                predicate = predicate.And(x => x.Title.ToLower().Trim().Contains(search));
            }

            if (criteria.AssessmentStatus.HasValue)
            {
                predicate = predicate.And(
                    p => p.AssessmentStatus.Equals(criteria.AssessmentStatus.Value)
                );
            }

            var isSuperAdminOrAdmin = IsSuperAdminOrAdmin(criteria.CurrentUserId).Result;

            if (criteria.CurrentUserId != default)
            {
                if (isSuperAdminOrAdmin == true)
                {
                    return predicate;
                }
                else
                {
                    return predicate.And(
                        x =>
                            x.AssessmentStatus == AssessmentStatus.Published
                            || x.CreatedBy == criteria.CurrentUserId
                    );
                }
            }

            return predicate;
        }

        /// <summary>
        /// /// Update assessment
        /// </summary>
        /// <param name="title">The predicate.</param>
        /// <param name="slug">The search criteria.</param>
        /// <returns>The updated assessment</returns>
        public async Task<Assessment> UpdateAsync(
            Guid identity,
            AssessmentRequestModel model,
            Guid currentUserId,
            Assessment existingAssessment
        )
        {
            return await ExecuteWithResultAsync<Assessment>(async () =>
            {
                var currentTimeStamp = DateTime.UtcNow;

                existingAssessment.Title = model.Title;
                existingAssessment.Description = model.Description;
                existingAssessment.Retakes = model.Retakes;
                existingAssessment.Duration = model.Duration;
                existingAssessment.StartDate = model.StartDate;
                existingAssessment.EndDate = model.EndDate;
                existingAssessment.Weightage = model.Weightage;
                existingAssessment.UpdatedBy = currentUserId;
                existingAssessment.UpdatedOn = currentTimeStamp;

                if (existingAssessment.SkillsCriteria.Count > 0)
                {
                    _unitOfWork
                        .GetRepository<SkillsCriteria>()
                        .Delete(existingAssessment.SkillsCriteria);
                }

                if (existingAssessment.EligibilityCreations.Count > 0)
                {
                    _unitOfWork
                        .GetRepository<EligibilityCreation>()
                        .Delete(existingAssessment.EligibilityCreations);
                }

                existingAssessment.SkillsCriteria = new List<SkillsCriteria>();
                existingAssessment.EligibilityCreations = new List<EligibilityCreation>();

                model.SkillsCriteriaRequestModels.ForEach(i =>
                {
                    existingAssessment.SkillsCriteria.Add(
                        new SkillsCriteria()
                        {
                            Id = Guid.NewGuid(),
                            CreatedBy = currentUserId,
                            UpdatedBy = currentUserId,
                            CreatedOn = currentTimeStamp,
                            UpdatedOn = currentTimeStamp,
                            SkillId = i.SkillId,
                            SkillAssessmentRule = i.SkillAssessmentRule,
                            Percentage = i.Percentage,
                            AssessmentId = existingAssessment.Id,
                        }
                    );
                });

                await _unitOfWork
                    .GetRepository<SkillsCriteria>()
                    .InsertAsync(existingAssessment.SkillsCriteria)
                    .ConfigureAwait(false);
                model.EligibilityCreationRequestModels.ForEach(i =>
                {
                    if (
                        i.SkillId != null
                        || i.Role != 0
                        || i.GroupId != null
                        || i.DepartmentId != null
                        || i.AssessmentId != null
                        || i.TrainingId != null
                    )
                    {
                        existingAssessment.EligibilityCreations.Add(
                            new EligibilityCreation()
                            {
                                Id = Guid.NewGuid(),
                                CreatedBy = currentUserId,
                                UpdatedBy = currentUserId,
                                CreatedOn = currentTimeStamp,
                                UpdatedOn = currentTimeStamp,
                                SkillId = i.SkillId,
                                Role = i.Role,
                                TrainingId = i.TrainingId,
                                DepartmentId = i.DepartmentId,
                                GroupId = i.GroupId,
                                CompletedAssessmentId = i.AssessmentId,
                                AssessmentId = existingAssessment.Id,
                            }
                        );
                    }
                });

                await _unitOfWork
                    .GetRepository<EligibilityCreation>()
                    .InsertAsync(existingAssessment.EligibilityCreations)
                    .ConfigureAwait(false);
                _unitOfWork.GetRepository<Assessment>().Update(existingAssessment);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve updated Assessment by identity
                return await GetByIdOrSlugAsync(identity.ToString(), currentUserId);
            });
        }

        public async Task<string> ChangeStatusAsync(
            AssessmentStatusRequestModel model,
            Guid currentUserId,
            Assessment existingAssessment
        )
        {
            return await ExecuteWithResultAsync(async () =>
            {
                if (
                    (
                        existingAssessment.AssessmentStatus == AssessmentStatus.Published
                        && (
                            model.Status == AssessmentStatus.Review
                            || model.Status == AssessmentStatus.Rejected
                        )
                    )
                    || (
                        existingAssessment.AssessmentStatus == AssessmentStatus.Rejected
                        && model.Status == AssessmentStatus.Published
                    )
                    || (
                        existingAssessment.AssessmentStatus != AssessmentStatus.Published
                        && model.Status == AssessmentStatus.Completed
                    )
                )
                {
                    _logger.LogWarning(
                        "Training with id: {id} cannot be changed from {status} status to {changeStatus} status.",
                        existingAssessment.Id,
                        existingAssessment.AssessmentStatus,
                        model.Status
                    );
                    throw new ForbiddenException(
                        _localizer.GetString("TrainingStatusCannotChanged")
                    );
                }

                var isSuperAdminOrAdminAccess = await IsSuperAdminOrAdmin(currentUserId)
                    .ConfigureAwait(false);
                if (
                    !isSuperAdminOrAdminAccess
                    && (
                        model.Status == AssessmentStatus.Published
                        || model.Status == AssessmentStatus.Rejected
                    )
                )
                {
                    _logger.LogWarning(
                        "User with id: {userId} is unauthorized user to change training with id: {id} status from {status} to {changeStatus}.",
                        currentUserId,
                        existingAssessment.Id,
                        existingAssessment.AssessmentStatus,
                        model.Status
                    );
                    throw new ForbiddenException(_localizer.GetString("UnauthorizedUser"));
                }

                var currentTimeStamp = DateTime.UtcNow;
                if (
                    isSuperAdminOrAdminAccess
                    && model.Status != AssessmentStatus.Rejected
                    && model.Status != AssessmentStatus.Draft
                )
                {
                    existingAssessment.AssessmentStatus = AssessmentStatus.Published;
                }
                else
                {
                    existingAssessment.AssessmentStatus = model.Status;
                }

                if (model.Status == AssessmentStatus.Rejected && model.Message != null)
                {
                    existingAssessment.Message = model.Message;
                }

                existingAssessment.UpdatedBy = currentUserId;
                existingAssessment.UpdatedOn = currentTimeStamp;

                _unitOfWork.GetRepository<Assessment>().Update(existingAssessment);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                var newUsers = new List<User>();

                if (isSuperAdminOrAdminAccess && model.Status == AssessmentStatus.Rejected)
                {
                    // send mail to trainer if assessment is rejected
                    BackgroundJob.Enqueue<IHangfireJobService>(
                        job => job.SendAssessmentRejectMailAsync(existingAssessment.Id, null)
                    );
                }
                else if (isSuperAdminOrAdminAccess && model.Status == AssessmentStatus.Published)
                {
                    // send mail to trainer if assessment is accepted
                    BackgroundJob.Enqueue<IHangfireJobService>(
                        job => job.SendAssessmentAcceptMailAsync(existingAssessment.Id, null)
                    );
                }
                else if (!isSuperAdminOrAdminAccess && model.Status == AssessmentStatus.Review)
                {
                    // send mail to admin/super admin if assessment is sent for review by trainer
                    BackgroundJob.Enqueue<IHangfireJobService>(
                        job =>
                            job.SendAssessmentReviewMailAsync(existingAssessment.Id, newUsers, null)
                    );
                }

                if (isSuperAdminOrAdminAccess)
                {
                    return _localizer.GetString("AssessmentPublishedSuccessfully");
                }
                else
                {
                    return _localizer.GetString("TrainingStatus");
                }
            });
        }

        public async Task DeleteAssessmentAsync(string assessmentIdentity, Guid currentUserId)
        {
            await ExecuteAsync(async () =>
                {
                    var assessment = await _unitOfWork
                        .GetRepository<Assessment>()
                        .GetFirstOrDefaultAsync(
                            predicate: p =>
                                p.Id.ToString() == assessmentIdentity
                                || p.Slug == assessmentIdentity
                        )
                        .ConfigureAwait(false);
                    if (assessment == null)
                    {
                        _logger.LogWarning(
                            "DeleteAssessmentAsync(): Assessment with identity : {AssessmentIdentity} was not found for user with id : {userId}",
                            assessmentIdentity,
                            currentUserId
                        );
                        throw new EntityNotFoundException(
                            _localizer.GetString("AssessmentNotFound")
                        );
                    }

                    var assessmentSkills = await _unitOfWork
                        .GetRepository<SkillsCriteria>()
                        .GetAllAsync(predicate: p => p.AssessmentId == assessment.Id)
                        .ConfigureAwait(false);
                    var assessmentEligibility = await _unitOfWork
                        .GetRepository<EligibilityCreation>()
                        .GetAllAsync(predicate: p => p.AssessmentId == assessment.Id)
                        .ConfigureAwait(false);
                    var AssessmentQuestion = await _unitOfWork
                        .GetRepository<AssessmentQuestion>()
                        .GetAllAsync(predicate: p => p.AssessmentId == assessment.Id)
                        .ConfigureAwait(false);

                    var assessmentSubmissions = await _unitOfWork
                        .GetRepository<AssessmentSubmission>()
                        .GetAllAsync(predicate: p => p.AssessmentId == assessment.Id)
                        .ConfigureAwait(false);
                    if (assessmentSubmissions.Count > 0)
                    {
                        _logger.LogWarning(
                            "DeleteAssessmentAsync(): Assessment with identity : {AssessmentIdentity} was not found for user with id : {userId}",
                            assessmentIdentity,
                            currentUserId
                        );
                        throw new EntityNotFoundException(
                            _localizer.GetString("AssessmentCannotBeDeleted")
                        );
                    }

                    _unitOfWork.GetRepository<SkillsCriteria>().Delete(assessmentSkills);
                    _unitOfWork.GetRepository<EligibilityCreation>().Delete(assessmentEligibility);
                    _unitOfWork.GetRepository<AssessmentQuestion>().Delete(AssessmentQuestion);
                    _unitOfWork.GetRepository<Assessment>().Delete(assessment);
                    await _unitOfWork.SaveChangesAsync();
                })
                .ConfigureAwait(false);
        }

        public async Task<(bool hasCompleted, int RemainingAttempts)> GetAssessmentCriteria(
            Assessment assessment,
            string identity,
            Guid currentUserId
        )
        {
            var hasCompleted = false;
            var RemainingAttempts = 0;
            var Retakes = assessment.Retakes;
            if (await IsSuperAdminOrAdmin(currentUserId))
            {
                hasCompleted = false;
            }
            else
            {
                var submissionCount =
                    assessment.AssessmentSubmissions?.Count(
                        sub =>
                            sub.AssessmentId == assessment.Id
                            && sub.UserId == currentUserId
                            && sub.EndTime != default
                    ) ?? 0;
                if (Retakes == 0 && submissionCount == 0)
                {
                    hasCompleted = false;
                    RemainingAttempts = 1;
                }
                else if (Retakes == 0 && submissionCount == 1)
                {
                    hasCompleted = true;
                    RemainingAttempts = 0;
                }
                else
                {
                    RemainingAttempts = Retakes - submissionCount + 1;
                    if (RemainingAttempts <= 0)
                    {
                        hasCompleted = true;
                    }
                }

                if (assessment.EndDate < DateTime.Now)
                {
                    hasCompleted = true;
                }
            }

            return (hasCompleted, RemainingAttempts);
        }

        public async Task<(
            bool,
            IEnumerable<EligibilityCreationResponseModel>
        )> GetUserEligibilityStatus(Assessment Entity, Guid currentUserId)
        {
            var isEligibleNew = true;
            var isEligibleOld = true;
            var count = 0;

            var eligibilities = new List<EligibilityCreationResponseModel>();

            foreach (var item in Entity.EligibilityCreations)
            {
                var eligibilityCreationResponseModel = new EligibilityCreationResponseModel()
                {
                    Id = item.Id,
                    Role = item.Role,
                    SkillId = item.SkillId,
                    SkillName = item.Skills?.SkillName,
                    AssessmentId = item.CompletedAssessmentId,
                    AssessmentName = item.CompletedAssessment?.Title,
                    DepartmentId = item.DepartmentId,
                    DepartmentName = item.Department?.Name,
                    TrainingId = item.TrainingId,
                    TrainingName = item.Course?.Name,
                    GroupId = item.GroupId,
                    GroupName = item.Group?.Name
                };

                count = count + 1;
                var isEligibleDepartment = true;
                var isEligibleSkills = true;
                var isEligibleTraining = true;
                var isEligibleGroup = true;
                var isEligibleAssessment = true;
                if (item.SkillId != null)
                {
                    var existingSkill = await _unitOfWork
                        .GetRepository<UserSkills>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.SkillId == item.SkillId && p.UserId == currentUserId
                        );
                    if (existingSkill != null)
                    {
                        eligibilityCreationResponseModel.IsEligible = true;
                        isEligibleSkills = true;
                    }
                    else
                    {
                        isEligibleSkills = false;
                    }
                }

                if (item.DepartmentId != null)
                {
                    var existingDepartment = await _unitOfWork
                        .GetRepository<Department>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.Id == item.DepartmentId,
                            include: src => src.Include(x => x.Users)
                        );
                    var isCurrentUserPresent = existingDepartment.Users.Any(
                        member => member.Id == currentUserId
                    );

                    if (existingDepartment != null)
                    {
                        isEligibleDepartment = true;
                    }
                    else
                    {
                        isEligibleDepartment = false;
                    }
                }

                if (item.TrainingId != null)
                {
                    var existingCourse = await _unitOfWork
                        .GetRepository<Course>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.Id == item.TrainingId,
                            include: src => src.Include(x => x.CourseEnrollments)
                        );
                    var isCurrentUserPresent = existingCourse.CourseEnrollments.Any(
                        member => member.UserId == currentUserId
                    );
                    if (isCurrentUserPresent == true)
                    {
                        isEligibleTraining = true;
                    }
                    else
                    {
                        isEligibleTraining = false;
                    }
                }

                if (item.GroupId != null)
                {
                    var existingGroup = await _unitOfWork
                        .GetRepository<Group>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.Id == item.GroupId,
                            include: src => src.Include(x => x.GroupMembers)
                        );
                    var isCurrentUserPresent = existingGroup.GroupMembers.Any(
                        member => member.UserId == currentUserId && member.IsActive
                    );

                    if (isCurrentUserPresent == true)
                    {
                        isEligibleGroup = true;
                    }
                    else
                    {
                        isEligibleGroup = false;
                    }
                }

                if (item.CompletedAssessmentId != null)
                {
                    var existingAssessment = await _unitOfWork
                        .GetRepository<Assessment>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.Id == item.CompletedAssessmentId,
                            include: src => src.Include(x => x.AssessmentResults)
                        );
                    var isCurrentUserPresent = existingAssessment.AssessmentResults.Any(
                        member => member.UserId == currentUserId
                    );
                    if (isCurrentUserPresent == true)
                    {
                        isEligibleAssessment = true;
                    }
                    else
                    {
                        isEligibleAssessment = false;
                    }
                }

                if (item.Role > 0)
                {
                    var existingUser = await _unitOfWork
                        .GetRepository<User>()
                        .GetFirstOrDefaultAsync(
                            predicate: p => p.Id == currentUserId && p.Role == item.Role
                        );
                    if (existingUser != null)
                    {
                        isEligibleAssessment = true;
                    }
                    else
                    {
                        isEligibleAssessment = false;
                    }
                }

                if (
                    isEligibleDepartment
                    && isEligibleSkills
                    && isEligibleTraining
                    && isEligibleGroup
                    && isEligibleAssessment
                )
                {
                    isEligibleOld = true;
                }
                else
                {
                    isEligibleOld = false;
                }

                eligibilityCreationResponseModel.IsEligible = isEligibleOld;

                if (count == 1)
                {
                    isEligibleNew = isEligibleOld;
                }

                isEligibleNew = isEligibleNew || isEligibleOld;

                eligibilities.Add(eligibilityCreationResponseModel);
            }

            return (isEligibleNew, eligibilities);
        }
    }
}
