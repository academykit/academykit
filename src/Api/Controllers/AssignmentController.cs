namespace Lingtren.Api.Controllers
{
    using System.Globalization;
    using CsvHelper;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Helpers;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class AssignmentController : BaseApiController
    {
        private readonly IAssignmentService assignmentService;
        private readonly IValidator<AssignmentRequestModel> validator;
        private readonly IValidator<AssignmentReviewRequestModel> reviewValidator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public AssignmentController(
            IAssignmentService assignmentService,
            IValidator<AssignmentRequestModel> validator,
            IValidator<AssignmentReviewRequestModel> reviewValidator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.assignmentService = assignmentService;
            this.validator = validator;
            this.reviewValidator = reviewValidator;
            this.localizer = localizer;
        }

        /// <summary>
        /// get assignment api.
        /// </summary>
        /// <returns> the list of <see cref="AssignmentResponseModel" /> .</returns>
        [HttpGet]
        public async Task<IList<AssignmentResponseModel>> SearchAsync(
            [FromQuery] AssignmentBaseSearchCriteria searchCriteria
        )
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(
                searchCriteria.LessonIdentity,
                nameof(searchCriteria.LessonIdentity)
            );
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await assignmentService.SearchAsync(searchCriteria).ConfigureAwait(false);
        }

        /// <summary>
        /// create assignment api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="AssignmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="AssignmentRequestModel" /> .</returns>
        [HttpPost]
        public async Task<AssignmentResponseModel> CreateAsync(AssignmentRequestModel model)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new Assignment
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Hints = model.Hints,
                Description = model.Description,
                LessonId = model.LessonId,
                Type = model.Type,
                IsActive = true,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                AssignmentAttachments = new List<AssignmentAttachment>(),
                AssignmentQuestionOptions = new List<AssignmentQuestionOption>(),
            };
            if (model.Type == QuestionTypeEnum.Subjective && model.FileUrls?.Count > 0)
            {
                foreach (var item in model.FileUrls.Select((fileUrl, i) => new { i, fileUrl }))
                {
                    entity.AssignmentAttachments.Add(
                        new AssignmentAttachment
                        {
                            Id = Guid.NewGuid(),
                            AssignmentId = entity.Id,
                            FileUrl = item.fileUrl,
                            Order = item.i + 1,
                            CreatedBy = CurrentUser.Id,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = CurrentUser.Id,
                            UpdatedOn = currentTimeStamp,
                        }
                    );
                }
            }

            if (
                model.Type == QuestionTypeEnum.SingleChoice
                || model.Type == QuestionTypeEnum.MultipleChoice
            )
            {
                foreach (var item in model.Answers.Select((answer, i) => new { i, answer }))
                {
                    entity.AssignmentQuestionOptions.Add(
                        new AssignmentQuestionOption
                        {
                            Id = Guid.NewGuid(),
                            AssignmentId = entity.Id,
                            Order = item.i + 1,
                            Option = item.answer.Option,
                            IsCorrect = item.answer.IsCorrect,
                            CreatedBy = CurrentUser.Id,
                            CreatedOn = currentTimeStamp,
                            UpdatedBy = CurrentUser.Id,
                            UpdatedOn = currentTimeStamp,
                        }
                    );
                }
            }

            var response = await assignmentService.CreateAsync(entity).ConfigureAwait(false);
            return new AssignmentResponseModel(response, showHints: true, showCorrect: true);
        }

        /// <summary>
        /// get assignment by id or slug.
        /// </summary>
        /// <param name="identity"> the assignment id or slug.</param>
        /// <returns> the instance of <see cref="AssignmentResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<AssignmentResponseModel> Get(string identity)
        {
            var model = await assignmentService.GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            return new AssignmentResponseModel(model, showHints: true, showCorrect: true);
        }

        /// <summary>
        /// update assignment api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="AssignmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="AssignmentResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<AssignmentResponseModel> UpdateAsync(
            string identity,
            AssignmentRequestModel model
        )
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var savedEntity = await assignmentService
                .UpdateAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new AssignmentResponseModel(savedEntity, showHints: true, showCorrect: true);
        }

        /// <summary>
        /// delete assignment api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await assignmentService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AssignmentRemoved")
                }
            );
        }

        /// <summary>
        /// assignment submission api.
        /// </summary>
        /// <param name="identity">lesson id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpPost("{lessonIdentity}/submissions")]
        public async Task<IActionResult> SubmissionAsync(
            string lessonIdentity,
            IList<AssignmentSubmissionRequestModel> model
        )
        {
            await assignmentService
                .AssignmentSubmissionAsync(lessonIdentity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AssignmentSubmitted")
                }
            );
        }

        /// <summary>
        /// Get assignment submitted student api.
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug.</param>
        /// <param name="userId">the user id.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{lessonIdentity}/user/{userId}")]
        public async Task<AssignmentSubmissionStudentResponseModel> SubmissionAsync(
            string lessonIdentity,
            Guid userId
        ) =>
            await assignmentService
                .GetStudentSubmittedAssignment(lessonIdentity, userId, CurrentUser.Id)
                .ConfigureAwait(false);

        /// <summary>
        /// assignment review api.
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug.</param>
        /// <param name="model"></param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPost("{lessonIdentity}/review")]
        public async Task<IActionResult> ReviewAsync(
            string lessonIdentity,
            AssignmentReviewRequestModel model
        )
        {
            await reviewValidator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            await assignmentService
                .AssignmentReviewAsync(lessonIdentity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AssignmentReviewed")
                }
            );
        }

        /// <summary>
        /// assignment review api.
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug.</param>
        /// <param name="model"></param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPut("{lessonIdentity}/review/{id}")]
        public async Task<IActionResult> UpdateReviewAsync(
            string lessonIdentity,
            Guid id,
            AssignmentReviewRequestModel model
        )
        {
            await reviewValidator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            await assignmentService
                .UpdateAssignmentReviewAsync(lessonIdentity, id, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AssignmentReviewUpdate")
                }
            );
        }

        /// <summary>
        /// assignment review api.
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug.</param>
        /// <param name="model"></param>
        /// <returns>task completed.</returns>
        [HttpDelete("{lessonIdentity}/review/{id}")]
        public async Task<IActionResult> DeleteReviewAsync(string lessonIdentity, Guid id)
        {
            await assignmentService
                .DeleteReviewAsync(lessonIdentity, id, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AssignmentReviewDeleted")
                }
            );
        }

        [HttpGet("{lessonIdentity}/AssignmentExport")]
        public async Task<IActionResult> Export(string lessonIdentity)
        {
            var response = await assignmentService
                .GetResultsExportAsync(lessonIdentity, CurrentUser.Id)
                .ConfigureAwait(false);

            using var memoryStream = new MemoryStream();
            using var steamWriter = new StreamWriter(memoryStream);
            using (var csv = new CsvWriter(steamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(response);
                csv.Flush();
            }

            return File(memoryStream.ToArray(), "text/csv", "Results.csv");
        }

        [HttpGet("{lessonIdentity}/AssignmentIndividualExport")]
        public async Task<IActionResult> ExportIndividual(string lessonIdentity)
        {
            var response = await assignmentService
                .GetIndividualResultsExportAsync(lessonIdentity, CurrentUser.Id)
                .ConfigureAwait(false);

            using var memoryStream = new MemoryStream();
            using var steamWriter = new StreamWriter(memoryStream);
            using (var csv = new CsvWriter(steamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(response);
                csv.Flush();
            }

            return File(memoryStream.ToArray(), "text/csv", "Results.csv");
        }
    }
}
