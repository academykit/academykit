namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Api.Common;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Localization;
    using FluentValidation;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/course/{identity}/lesson")]
    public class LessonController : BaseApiController
    {
        private readonly ILessonService lessonService;
        private readonly IValidator<LessonRequestModel> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;
        private readonly IAssignmentService assignmentService;
        private readonly IQuestionSetService questionSetService;
        private readonly IFeedbackService feedbackService;

        public LessonController(
            ILessonService lessonService,
            IValidator<LessonRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IAssignmentService assignmentService,
            IQuestionSetService questionSetService,
            IFeedbackService feedbackService
        )
        {
            this.lessonService = lessonService;
            this.validator = validator;
            this.localizer = localizer;
            this.assignmentService = assignmentService;
            this.questionSetService = questionSetService;
            this.feedbackService = feedbackService;
        }

        /// <summary>
        /// get lesson api.
        /// </summary>
        /// <returns> the list of <see cref="LessonResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<LessonResponseModel>> SearchAsync(
            string identity,
            [FromQuery] LessonBaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            searchCriteria.CourseIdentity = identity;
            var searchResult = await lessonService
                .SearchAsync(searchCriteria)
                .ConfigureAwait(false);

            var response = new SearchResult<LessonResponseModel>
            {
                Items = new List<LessonResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p => response.Items.Add(new LessonResponseModel(p)));
            return response;
        }

        /// <summary>
        /// create lesson api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="LessonRequestModel" />. </param>
        /// <returns> the instance of <see cref="LessonRequestModel" /> .</returns>
        [HttpPost]
        public async Task<LessonResponseModel> CreateAsync(
            string identity,
            LessonRequestModel model
        )
        {
            if (model.Type == LessonType.Exam && model.QuestionSet?.AllowedRetake < 1)
            {
                model.QuestionSet.AllowedRetake = 1;
            }

            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var response = await lessonService
                .AddAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new LessonResponseModel(response);
        }

        /// <summary>
        /// get lesson by id or slug.
        /// </summary>
        /// <param name="lessonIdentity"> the department id or slug.</param>
        /// <returns> the instance of <see cref="LessonResponseModel" /> .</returns>
        [HttpGet("detail")]
        public async Task<LessonResponseModel> GetDetail(
            string identity,
            [FromQuery] string lessonIdentity
        )
        {
            return await lessonService
                .GetLessonAsync(identity, lessonIdentity, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// lesson reorder api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <param name="model"> the instance of <see cref="LessonReorderRequestModel"/>.</param>
        /// <returns> the task complete. </returns>
        [HttpPut("reorder")]
        public async Task<IActionResult> LessonReorder(
            string identity,
            LessonReorderRequestModel model
        )
        {
            await lessonService.ReorderAsync(identity, model, CurrentUser.Id);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("LessonReorder")
                }
            );
        }

        /// <summary>
        /// /// update department api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="LessonRequestModel" />. </param>
        /// <returns> the instance of <see cref="LessonResponseModel" /> .</returns>
        [HttpPut("{lessonIdentity}")]
        public async Task<LessonResponseModel> UpdateAsync(
            string identity,
            string lessonIdentity,
            LessonRequestModel model
        )
        {
            if (model.Type == LessonType.Exam && model.QuestionSet?.AllowedRetake < 1)
            {
                model.QuestionSet.AllowedRetake = 1;
            }

            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var response = await lessonService
                .UpdateAsync(identity, lessonIdentity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new LessonResponseModel(response);
        }

        /// <summary>
        /// update department api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="LessonRequestModel" />. </param>
        /// <returns> the instance of <see cref="LessonResponseModel" /> .</returns>
        [HttpPut("{lessonIdentity}/updateShuffle")]
        public async Task<QuestionSetResponseModel> UpdateShuffleAsync(
            string identity,
            string lessonIdentity,
            QuestionSetRequestModel model
        )
        {
            var response = await lessonService
                .UpdateQuestionAsync(identity, lessonIdentity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            return new QuestionSetResponseModel(response);
        }

        /// <summary>
        /// update department api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="LessonRequestModel" />. </param>
        /// <returns> the instance of <see cref="LessonResponseModel" /> .</returns>
        [HttpGet("{lessonIdentity}/GetShuffle")]
        public async Task<QuestionSetResponseModel> GetShuffle(
            string identity,
            string lessonIdentity
        )
        {
            var response = await lessonService
                .GetQuestionAsync(identity, lessonIdentity, CurrentUser.Id)
                .ConfigureAwait(false);
            return new QuestionSetResponseModel(response);
        }

        /// <summary>
        /// delete lesson api.
        /// </summary>
        /// <param name="identity">course id or slug. </param>
        /// <param name="lessonIdentity">lesson id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{lessonIdentity}")]
        public async Task<IActionResult> DeleteAsync(string identity, string lessonIdentity)
        {
            await lessonService
                .DeleteLessonAsync(identity, lessonIdentity, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("LessonRemoved")
                }
            );
        }

        /// <summary>
        /// live class join api.
        /// </summary>
        /// <param name="identity">the department id or slug.</param>
        /// <param name="enabled">the boolean.</param>
        /// <returns>the instance of <see cref="LessonResponseModel"/>.</returns>
        [HttpGet("{lessonIdentity}/join")]
        public async Task<MeetingJoinResponseModel> Join(string identity, string lessonIdentity)
        {
            var response = await lessonService
                .GetJoinMeetingAsync(identity, lessonIdentity, CurrentUser.Id)
                .ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// get user meeting report api.
        /// </summary>
        /// <param name="lessonIdentity"> the lesson identity. </param>
        /// <param name="userId"> the user id. </param>
        /// <returns> the list of  <see cref="MeetingReportResponseModel" />. </returns>
        [HttpGet("{lessonIdentity}/meetingReport/{userId}")]
        public async Task<IList<MeetingReportResponseModel>> MeetingReport(
            string identity,
            string lessonIdentity,
            string userId
        )
        {
            var report = await lessonService
                .GetMeetingReportAsync(identity, lessonIdentity, userId, CurrentUser.Id)
                .ConfigureAwait(false);
            return report;
        }

        /// <summary>
        /// reorder assignment questions.
        /// </summary>
        /// <param name="lessonIdentity">lesson id or slug.</param>
        /// <param name="ids">list of question ids in lesson.</param>
        /// <returns>task completed.</returns>
        [HttpPost("Reorder")]
        public async Task<IActionResult> Reorder(
            string lessonIdentity,
            LessonType lessonType,
            List<Guid> ids
        )
        {
            switch (lessonType)
            {
                case LessonType.Assignment:
                    await assignmentService.ReorderAssignmentQuestionAsync(
                        CurrentUser.Id,
                        lessonIdentity,
                        ids
                    );
                    break;
                case LessonType.Exam:
                    await questionSetService
                        .ReorderQuestionSetQuestionsAsync(CurrentUser.Id, lessonIdentity, ids)
                        .ConfigureAwait(false);
                    break;
                case LessonType.Feedback:
                    await feedbackService
                        .ReorderFeedbackQuestionsAsync(CurrentUser.Id, lessonIdentity, ids)
                        .ConfigureAwait(false);
                    break;
            }

            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("AssignmentUpdatedSuccessfully")
                }
            );
        }
    }
}
