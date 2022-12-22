using FluentValidation;
using Lingtren.Api.Common;
using Lingtren.Api.Controllers;
using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.Common.Models.ResponseModels;
using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;
using Lingtren.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class FeedbackController : BaseApiController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IValidator<FeedbackRequestModel> _validator;
        public FeedbackController(
            IFeedbackService feedbackService,
            IValidator<FeedbackRequestModel> validator
            )
        {
            _feedbackService = feedbackService;
            _validator = validator;
        }

        /// <summary>
        /// get Feedback api
        /// </summary>
        /// <returns> the list of <see cref="FeedbackResponseModel" /> .</returns>
        [HttpGet]
        public async Task<IList<FeedbackResponseModel>> SearchAsync([FromQuery] FeedbackBaseSearchCriteria searchCriteria)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(searchCriteria.LessonIdentity, nameof(searchCriteria.LessonIdentity));
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await _feedbackService.SearchAsync(searchCriteria).ConfigureAwait(false);
        }

        /// <summary>
        /// create Feedback api
        /// </summary>
        /// <param name="model"> the instance of <see cref="FeedbackRequestModel" />. </param>
        /// <returns> the instance of <see cref="FeedbackRequestModel" /> .</returns>
        [HttpPost]
        public async Task<FeedbackResponseModel> CreateAsync(FeedbackRequestModel model)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new Feedback
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                LessonId = model.LessonId,
                Type = model.Type,
                IsActive = true,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                FeedbackQuestionOptions = new List<FeedbackQuestionOption>()
            };
            if (model.Type == FeedbackTypeEnum.SingleChoice || model.Type == FeedbackTypeEnum.MultipleChoice)
            {
                foreach (var item in model.Answers.Select((answer, i) => new { i, answer }))
                {
                    entity.FeedbackQuestionOptions.Add(new FeedbackQuestionOption
                    {
                        Id = Guid.NewGuid(),
                        FeedbackId = entity.Id,
                        Order = item.i + 1,
                        Option = item.answer.Option,
                        CreatedBy = CurrentUser.Id,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = CurrentUser.Id,
                        UpdatedOn = currentTimeStamp,
                    });
                }
            }
            var response = await _feedbackService.CreateAsync(entity).ConfigureAwait(false);
            return new FeedbackResponseModel(response);
        }

        /// <summary>
        /// get Feedback by id or slug
        /// </summary>
        /// <param name="identity"> the Feedback id or slug</param>
        /// <returns> the instance of <see cref="FeedbackResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<FeedbackResponseModel> Get(string identity)
        {
            var model = await _feedbackService.GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            return new FeedbackResponseModel(model);
        }

        /// <summary>
        /// update Feedback api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="FeedbackRequestModel" />. </param>
        /// <returns> the instance of <see cref="FeedbackResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<FeedbackResponseModel> UpdateAsync(string identity, FeedbackRequestModel model)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var savedEntity = await _feedbackService.UpdateAsync(identity, model, CurrentUser.Id).ConfigureAwait(false);
            return new FeedbackResponseModel(savedEntity);
        }

        /// <summary>
        /// delete Feedback api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            await _feedbackService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Feedback removed successfully." });
        }

        /// <summary>
        /// Feedback submission api
        /// </summary>
        /// <param name="identity">lesson id or slug </param>
        /// <returns> the task complete </returns>
        [HttpPost("{lessonIdentity}/submissions")]
        public async Task<IActionResult> SubmissionAsync(string lessonIdentity, IList<FeedbackSubmissionRequestModel> model)
        {
            await _feedbackService.FeedbackSubmissionAsync(lessonIdentity, model, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Feedback Submitted Successfully." });
        }

        /// <summary>
        /// Get feedback submitted student list api
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <returns></returns>
        [HttpGet("{lessonIdentity}/users")]
        public async Task<IList<FeedbackSubmissionStudentResponseModel>> SubmissionAsync(string lessonIdentity) =>
            await _feedbackService.GetFeedbackSubmittedStudent(lessonIdentity, CurrentUser.Id).ConfigureAwait(false);

    }
}