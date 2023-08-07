using FluentValidation;
using Lingtren.Api.Common;
using Lingtren.Api.Controllers;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Infrastructure.Localization;
using Lingtren.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhysicalLessonController : BaseApiController
    {
        private readonly IPhysicalLessonServices _physicsLessonServices;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;
        private readonly IValidator<PhysicalLessonReviewRequestModel> _validator;
        public PhysicalLessonController(IStringLocalizer<ExceptionLocalizer> localizer, IPhysicalLessonServices physicsLessonServices, IValidator<PhysicalLessonReviewRequestModel> validator)
        {
            _physicsLessonServices = physicsLessonServices;
            _localizer = localizer;
            _validator = validator;
        }

        /// <summary>
        /// Reviews physical lesson attendance
        /// </summary>
        /// <param name="Idenity"></param>
        /// <returns></returns>
        [HttpPost("Attendance")]
        public async Task<IActionResult> Attendance(string Idenity)
        {
            await _physicsLessonServices.PhysicalLessonAttendanceAsync(Idenity, CurrentUser.Id.ToString()).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("SuccessfulAttendance") });
        }

        [HttpPost("Review")]
        public async Task<IActionResult> Review(PhysicalLessonReviewRequestModel model)
        {
            await _validator.ValidateAsync(model,option => option.ThrowOnFailures()).ConfigureAwait(false);
            await _physicsLessonServices.PhysicalLessonReviewAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("SuccessfullyReviewed") });
        }
    }
}