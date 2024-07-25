// <copyright file="DepartmentController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using System.Globalization;
    using CsvHelper;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Localization;
    using Lingtren.Infrastructure.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class AssessmentExamController : BaseApiController
    {
        private readonly IAssessmentSubmissionService assessmentSubmissionService;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public AssessmentExamController(
            IAssessmentSubmissionService assessmentSubmissionService,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.assessmentSubmissionService = assessmentSubmissionService;
            this.localizer = localizer;
        }

        [HttpPost("{identity}/AnswerSubmission")]
        public async Task<IActionResult> AnswerSubmission(
            string identity,
            IList<AssessmentSubmissionRequestModel> answers
        )
        {
            await assessmentSubmissionService
                .AnswerSubmission(identity, answers, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(new { statusCode = 200, message = localizer.GetString("QuestionSetAnswer") });
        }

        /// <summary>
        /// get results api.
        /// </summary>
        /// <param name="identity"the question set id or slug></param>
        /// <returns>the list of <see cref="questio"/>.</returns>
        [HttpGet("{identity}/GetResults")]
        public async Task<SearchResult<AssessmentResultResponseModel>> GetResults(
            [FromQuery] BaseSearchCriteria searchCriteria,
            string identity
        ) =>
            await assessmentSubmissionService
                .GetResults(searchCriteria, identity, CurrentUser.Id)
                .ConfigureAwait(false);

        /// <summary>
        /// get particular user results.
        /// </summary>
        /// <param name="identity"the AssessmentId id or slug></param>
        /// <returns>the list of <see cref="QuestionSetResultResponseModel"/>.</returns>
        [HttpGet("{identity}/GetStudentResults/{userId}")]
        public async Task<StudentAssessmentResultResponseModel> GetStudentResults(
            string identity,
            Guid userId
        ) =>
            await assessmentSubmissionService
                .GetStudentResult(identity, userId, CurrentUser.Id)
                .ConfigureAwait(false);

        /// <summary>
        /// get result detail api.
        /// </summary>
        /// <param name="identity">the question set id or slug.</param>
        /// <param name="assessmentSubmissionId">the question set submission id.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/GetResultDetail/{assessmentSubmissionId}/detail")]
        public async Task<AssessmentUserResultResponseModel> GetResultDetail(
            string identity,
            Guid assessmentSubmissionId
        ) =>
            await assessmentSubmissionService
                .GetResultDetail(identity, assessmentSubmissionId, CurrentUser.Id)
                .ConfigureAwait(false);

        [HttpGet("{identity}/GetStudentResults/{userId}/Export")]
        public async Task<IActionResult> ExportStudentResults(string identity, Guid userId)
        {
            var response = await assessmentSubmissionService
                .GetResultsExportAsync(identity, userId)
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
