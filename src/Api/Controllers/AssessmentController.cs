// <copyright file="DepartmentController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;

    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Localization;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class AssessmentController : BaseApiController
    {
        private readonly IAssessmentService assessmentService;
        private readonly IValidator<AssessmentRequestModel> validator;
        private readonly IValidator<AssessmentStatusRequestModel> assessmentStatusValidator;

        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public AssessmentController(
            IAssessmentService assessmentService,
            IValidator<AssessmentRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IValidator<AssessmentStatusRequestModel> assessmentStatusValidator
        )
        {
            this.assessmentService = assessmentService;
            this.validator = validator;
            this.localizer = localizer;
            this.assessmentStatusValidator = assessmentStatusValidator;
        }

        /// <summary>
        /// course search api.
        /// </summary>
        /// <returns> the list of <see cref="CourseResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<AssessmentResponseModel>> SearchAsync(
            [FromQuery] AssessmentBaseSearchCriteria searchCriteria
        )
        {
            if (searchCriteria.UserId != Guid.Empty)
            {
                searchCriteria.CurrentUserId = searchCriteria.UserId;
            }
            else
            {
                searchCriteria.CurrentUserId = CurrentUser.Id;
            }
            searchCriteria.SortBy = "CreatedOn";
            searchCriteria.SortType = SortType.Descending;

            var searchResult = await assessmentService
                .SearchAsync(searchCriteria)
                .ConfigureAwait(false);

            var response = new SearchResult<AssessmentResponseModel>
            {
                Items = new List<AssessmentResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            foreach (var item in searchResult.Items)
            {
                var eligibilityStatus = await assessmentService.GetUserEligibilityStatus(
                    item,
                    CurrentUser.Id
                );
                var assestmentResponseModel = new AssessmentResponseModel(
                    item,
                    eligibilityStatus.Item1
                );
                assestmentResponseModel.EligibilityCreationRequestModels =
                    eligibilityStatus.Item2.ToList();
                response.Items.Add(assestmentResponseModel);
            }

            return response;
        }

        /// <summary>
        /// change course status api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="CourseStatusRequestModel" /> . </param>
        /// <returns> the task complete. </returns>
        [HttpPatch("status")]
        public async Task<IActionResult> ChangeStatus(AssessmentStatusRequestModel model)
        {
            await assessmentStatusValidator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var existingAssessment = await assessmentService
                .GetByIdOrSlugAsync(model.Identity, CurrentUser.Id, false)
                .ConfigureAwait(false);
            var result = await assessmentService
                .ChangeStatusAsync(model, CurrentUser.Id, existingAssessment)
                .ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = result });
        }

        /// <summary>
        /// get course by id or slug.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<AssessmentResponseModel> Get(string identity)
        {
            var existingAssessment = await assessmentService
                .GetByIdOrSlugAsync(identity, CurrentUser.Id, true)
                .ConfigureAwait(false);
            (var completed, var remainingAttempts) = await assessmentService
                .GetAssessmentCriteria(existingAssessment, identity, CurrentUser.Id)
                .ConfigureAwait(false);

            var Eligibility = await assessmentService.GetUserEligibilityStatus(
                existingAssessment,
                CurrentUser.Id
            );
            var existingQuestion = existingAssessment.AssessmentQuestions.Count();

            var response = new AssessmentResponseModel(
                existingAssessment,
                Eligibility.Item1,
                existingQuestion,
                completed,
                remainingAttempts
            );

            response.EligibilityCreationRequestModels = Eligibility.Item2.ToList();

            return response;
        }

        /// <summary>
        /// create department api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="SkillsResponseModel" />. </param>
        /// <returns> the instance of <see cref="SkillsRequestModel" /> .</returns>
        [HttpPost]
        public async Task<AssessmentResponseModel> CreateAsync(AssessmentRequestModel model)
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            var entity = new Assessment
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                Retakes = model.Retakes,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Duration = model.Duration,
                Weightage = model.Weightage,
                Description = model.Description,
                AssessmentStatus = AssessmentStatus.Draft,
                SkillsCriteria = new List<SkillsCriteria>(),
                EligibilityCreations = new List<EligibilityCreation>(),
            };

            foreach (var item in model.SkillsCriteriaRequestModels)
            {
                entity.SkillsCriteria.Add(
                    new SkillsCriteria
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = entity.Id,
                        SkillId = item.SkillId,
                        SkillAssessmentRule = item.SkillAssessmentRule,
                        Percentage = item.Percentage,
                        CreatedOn = currentTimeStamp,
                        CreatedBy = CurrentUser.Id,
                        UpdatedOn = currentTimeStamp,
                        UpdatedBy = CurrentUser.Id,
                    }
                );
            }
            foreach (var item in model.EligibilityCreationRequestModels)
            {
                if (
                    item.SkillId != null
                    || item.Role != 0
                    || item.GroupId != null
                    || item.DepartmentId != null
                    || item.AssessmentId != null
                    || item.TrainingId != null
                )
                {
                    entity.EligibilityCreations.Add(
                        new EligibilityCreation
                        {
                            Id = Guid.NewGuid(),
                            AssessmentId = entity.Id,
                            SkillId = item.SkillId,
                            GroupId = item.GroupId,
                            TrainingId = item.TrainingId,
                            DepartmentId = item.DepartmentId,
                            CompletedAssessmentId = item.AssessmentId,
                            Role = item.Role,
                            CreatedOn = currentTimeStamp,
                            CreatedBy = CurrentUser.Id,
                            UpdatedOn = currentTimeStamp,
                            UpdatedBy = CurrentUser.Id,
                        }
                    );
                }
            }
            var response = await assessmentService.CreateAsync(entity).ConfigureAwait(false);
            return new AssessmentResponseModel(response);
        }

        /// <summary>
        /// update assessment api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="DepartmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="DepartmentResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<AssessmentResponseModel> UpdateAsync(
            string identity,
            AssessmentRequestModel model
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var existingAssessment = await assessmentService
                .GetByIdOrSlugAsync(identity, CurrentUser.Id, true)
                .ConfigureAwait(false);
            var savedEntity = await assessmentService
                .UpdateAsync(existingAssessment.Id, model, CurrentUser.Id, existingAssessment)
                .ConfigureAwait(false);

            return new AssessmentResponseModel(savedEntity);
        }

        /// <summary>
        /// delete skills api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await assessmentService
                .DeleteAssessmentAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AssessmentRemoved")
                }
            );
        }
    }
}
