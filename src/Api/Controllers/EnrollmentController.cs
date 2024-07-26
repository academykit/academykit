namespace Api.Controllers
{
    using AcademyKit.Api.Common;
    using AcademyKit.Api.Controllers;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : BaseApiController
    {
        private readonly IEnrollmentService enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            this.enrollmentService = enrollmentService;
        }

        /// <summary>
        /// enroll user in course.
        /// </summary>
        /// <param name="emailOrMobileNumber">email of user.</param>
        /// <param name="courseIdentity">Course id or slug.</param>
        /// <returns>Task completed.</returns>
        [HttpPost("Enrollment")]
        public async Task<IActionResult> EnrollUser(
            IList<string> emailOrMobileNumber,
            string courseIdentity
        )
        {
            var message = await enrollmentService
                .EnrollUserAsync(emailOrMobileNumber, CurrentUser.Id, courseIdentity)
                .ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = message });
        }

        /// <summary>
        /// search operation for course user.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns>List of user.</returns>
        [HttpGet("User")]
        public async Task<SearchResult<UserResponseModel>> SearchUser(
            [FromQuery] EnrollmentBaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            var result = await enrollmentService
                .CourseUserSearchAsync(searchCriteria)
                .ConfigureAwait(false);
            return result;
        }
    }
}
