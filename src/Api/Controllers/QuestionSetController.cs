﻿namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class QuestionSetController : BaseApiController
    {
        private readonly IQuestionSetService _questionSetService;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;
        public QuestionSetController(IQuestionSetService questionSetService, IStringLocalizer<ExceptionLocalizer> localizer)
        {
            _questionSetService = questionSetService;
            _localizer = localizer;
        }

        /// <summary>
        /// update question pool
        /// </summary>
        /// <param name="identity"> the question id or  slug</param>
        /// <returns> the instance of <see cref="QuestionSetResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        [AllowAnonymous]
        public async Task<QuestionSetResponseModel> Get(string identity)
        {
            var model = await _questionSetService.GetByIdOrSlugAsync(identity, CurrentUser?.Id).ConfigureAwait(false);
            return new QuestionSetResponseModel(model);
        }

        [HttpPost("{identity}/AddQuestion")]
        public async Task<IActionResult> AddQuestion(string identity, QuestionSetAddQuestionRequestModel model)
        {
            if (model.QuestionPoolQuestionIds.Count == 0)
            {
                throw new ForbiddenException("At least one question is required.");
            }
            await _questionSetService.AddQuestionsAsync(identity, model, CurrentUser.Id);
            return Ok();
        }

        /// <summary>
        /// Get question list
        /// </summary>
        /// <param name="identity"> the question id or  slug</param>
        /// <returns>the instance of <see cref="QuestionResponseModel"/></returns>
        [HttpGet("{identity}/questions")]
        public async Task<IList<QuestionResponseModel>> GetQuestions(string identity)
                    => await _questionSetService.GetQuestions(identity, CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// Handle to set start time
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>the instance of <see cref="QuestionSetSubmissionResponseModel"/></returns>
        [HttpPost("{identity}/startExam")]
        public async Task<QuestionSetSubmissionResponseModel> StartExam(string identity) => await _questionSetService.StartExam(identity, CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// Answer Submission
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="questionSetSubmissionId">the question set submission id</param>
        /// <param name="answers">the list of answer</param>
        /// <returns>the instance of <see cref="AssignmentResponseModel"/></returns>
        [HttpPost("{identity}/submission/{questionSetSubmissionId}")]
        public async Task<IActionResult> AnswerSubmission(string identity, Guid questionSetSubmissionId, IList<AnswerSubmissionRequestModel> answers)
        {
            await _questionSetService.AnswerSubmission(identity, questionSetSubmissionId, answers, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new { statusCode = 200, message = _localizer.GetString("QuestionSetAnswer") });
        }
    }
}
