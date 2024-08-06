namespace Api.Controllers
{
    using AcademyKit.Api.Common;
    using AcademyKit.Api.Controllers;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Infrastructure.Localization;
    using AcademyKit.Infrastructure.Services;
    using FluentValidation;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    [Route("api/[controller]")]
    [ApiController]
    public class PhysicalLessonController : BaseApiController
    {
        private readonly IPhysicalLessonServices physicsLessonServices;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;
        private readonly IValidator<PhysicalLessonReviewRequestModel> validator;

        public PhysicalLessonController(
            IStringLocalizer<ExceptionLocalizer> localizer,
            IPhysicalLessonServices physicsLessonServices,
            IValidator<PhysicalLessonReviewRequestModel> validator
        )
        {
            this.physicsLessonServices = physicsLessonServices;
            this.localizer = localizer;
            this.validator = validator;
        }

        /// <summary>
        /// physical lesson attendance.
        /// </summary>
        /// <param name="idenity">lesson identity.</param>
        /// <returns>Task completed.</returns>
        [HttpPost("Attendance")]
        public async Task<IActionResult> Attendance(string idenity)
        {
            await physicsLessonServices
                .PhysicalLessonAttendanceAsync(idenity, CurrentUser.Id.ToString())
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("SuccessfulAttendance")
                }
            );
        }

        /// <summary>
        /// physical lesson attendance review.
        /// </summary>
        /// <param name="model">instance of <see cref="PhysicalLessonReviewRequestModel"/>.</param>
        /// <returns>Task completed.</returns>
        [HttpPost("Review")]
        public async Task<IActionResult> Review(PhysicalLessonReviewRequestModel model)
        {
            await validator
                .ValidateAsync(model, option => option.ThrowOnFailures())
                .ConfigureAwait(false);
            await physicsLessonServices
                .PhysicalLessonReviewAsync(model, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("SuccessfullyReviewed")
                }
            );
        }
    }
}
