// <copyright file="QuestionSetResultController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/questionSet/{identity}/results")]
    public class QuestionSetResultController : BaseApiController
    {
        private readonly IQuestionSetService questionSetService;

        public QuestionSetResultController(
            IQuestionSetService questionSetService)
        {
            this.questionSetService = questionSetService;
        }

        /// <summary>
        /// get results api.
        /// </summary>
        /// <param name="identity"the question set id or slug></param>
        /// <returns>the list of <see cref="QuestionSetResultResponseModel"/>.</returns>
        [HttpGet]
        public async Task<SearchResult<QuestionSetResultResponseModel>> GetResults([FromQuery] BaseSearchCriteria searchCriteria, string identity)
                                => await questionSetService.GetResults(searchCriteria, identity, CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get particular user results.
        /// </summary>
        /// <param name="identity"the question set id or slug></param>
        /// <returns>the list of <see cref="QuestionSetResultResponseModel"/>.</returns>
        [HttpGet("{userId}")]
        public async Task<StudentResultResponseModel> GetStudentResults(string identity, Guid userId)
                                => await questionSetService.GetStudentResult(identity, userId, CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get result detail api.
        /// </summary>
        /// <param name="identity">the question set id or slug.</param>
        /// <param name="questionSetSubmissionId">the question set submission id.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{questionSetSubmissionId}/detail")]
        public async Task<QuestionSetUserResultResponseModel> GetResultDetail(string identity, Guid questionSetSubmissionId) => await questionSetService.GetResultDetail(identity, questionSetSubmissionId, CurrentUser.Id).ConfigureAwait(false);
    }
}
