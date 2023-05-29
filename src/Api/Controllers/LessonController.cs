namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/course/{identity}/lesson")]
    public class LessonController : BaseApiController
    {
        private readonly ILessonService _lessonService;
        private readonly IValidator<LessonRequestModel> _validator;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;
        public LessonController(
            ILessonService lessonService,
            IValidator<LessonRequestModel> validator,
            IStringLocalizer<ExceptionLocalizer> localizer)
        {
            _lessonService = lessonService;
            _validator = validator;
            _localizer = localizer;
        }

        /// <summary>
        /// get lesson api
        /// </summary>
        /// <returns> the list of <see cref="LessonResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<LessonResponseModel>> SearchAsync(string identity, [FromQuery] LessonBaseSearchCriteria searchCriteria)
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            searchCriteria.CourseIdentity = identity;
            var searchResult = await _lessonService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<LessonResponseModel>
            {
                Items = new List<LessonResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new LessonResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// create lesson api
        /// </summary>
        /// <param name="model"> the instance of <see cref="LessonRequestModel" />. </param>
        /// <returns> the instance of <see cref="LessonRequestModel" /> .</returns>
        [HttpPost]
        public async Task<LessonResponseModel> CreateAsync(string identity, LessonRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var response = await _lessonService.AddAsync(identity, model, CurrentUser.Id).ConfigureAwait(false);
            return new LessonResponseModel(response);
        }

        /// <summary>
        /// get lesson by id or slug
        /// </summary>
        /// <param name="lessonIdentity"> the department id or slug</param>
        /// <returns> the instance of <see cref="LessonResponseModel" /> .</returns>
        [HttpGet("detail")]
        public async Task<LessonResponseModel> GetDetail(string identity, [FromQuery] string lessonIdentity)
        {
            return await _lessonService.GetLessonAsync(identity, lessonIdentity, CurrentUser.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// lesson reorder api
        /// </summary>
        /// <param name="identity"> the course id or slug</param>
        /// <param name="model"> the instance of <see cref="LessonReorderRequestModel"/></param>
        /// <returns> the task complete </returns>
        [HttpPut("reorder")]

        public async Task<IActionResult> LessonReorder(string identity, LessonReorderRequestModel model)
        {
            await _lessonService.ReorderAsync(identity, model, CurrentUser.Id);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("LessonReorder") });
        }

        /// <summary>
        /// update department api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="LessonRequestModel" />. </param>
        /// <returns> the instance of <see cref="LessonResponseModel" /> .</returns>
        [HttpPut("{lessonIdentity}")]
        public async Task<LessonResponseModel> UpdateAsync(string identity, string lessonIdentity, LessonRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var response = await _lessonService.UpdateAsync(identity, lessonIdentity, model, CurrentUser.Id).ConfigureAwait(false);
            return new LessonResponseModel(response);
        }

        /// <summary>
        /// delete lesson api
        /// </summary>
        /// <param name="identity">course id or slug </param>
        /// <param name="lessonIdentity">lesson id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{lessonIdentity}")]
        public async Task<IActionResult> DeleteAsync(string identity, string lessonIdentity)
        {
            await _lessonService.DeleteLessonAsync(identity, lessonIdentity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("LessonRemoved") });
        }

        /// <summary>
        /// live class join api
        /// </summary>
        /// <param name="identity">the department id or slug</param>
        /// <param name="enabled">the boolean</param>
        /// <returns>the instance of <see cref="LessonResponseModel"/></returns>
        [HttpGet("{lessonIdentity}/join")]
        public async Task<MeetingJoinResponseModel> Join(string identity, string lessonIdentity)
        {
            var response = await _lessonService.GetJoinMeetingAsync(identity, lessonIdentity, CurrentUser.Id).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// get user meeting report api
        /// </summary>
        /// <param name="identity"> the lesson identity </param>
        /// <param name="userId"> the user id </param>
        /// <returns> the list of  <see cref="MeetingReportResponseModel" />. </returns>
        [HttpGet("{lessonidentity}/meetingreport/{userId}")]
        public async Task<IList<MeetingReportResponseModel>> MeetingReport(string identity, string lessonidentity, string userId)
        {
            var report = await _lessonService.GetMeetingReportAsync(identity, lessonidentity, userId, CurrentUser.Id).ConfigureAwait(false);
            return report;
        }
    }
}
