using Lingtren.Api.Common;
using Lingtren.Api.Controllers;
using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : BaseApiController
    {
        private readonly IEnrollmentService _enrollmentService;
        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// enroll user in course
        /// </summary>
        /// <param name="emailOrMobileNumber">email of user</param>
        /// <param name="courseIdentity">Course id or slug</param>
        /// <returns>Task completed</returns>
        [HttpPost("Enrollnment")]
        public async Task<IActionResult> Enrolluser(IList<string> emailOrMobileNumber,string courseIdentity)
        {
            var message = await _enrollmentService.EnrollUserAsync(emailOrMobileNumber, CurrentUser.Id, courseIdentity).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = message});
        }

        /// <summary>
        /// get not enrolled user list
        /// </summary>
        /// <param name="searchCritera"></param>
        /// <returns>List of user</returns>
        [HttpGet("Notenrolled")]
        public async Task<SearchResult<UserResponseModel>> NotEnrolledUser([FromQuery]EnrollmentBaseSearchCritera searchCritera)
        {
            searchCritera.CurrentUserId = CurrentUser.Id;
            var result = await _enrollmentService.GetUnenrolledUser(searchCritera).ConfigureAwait(false);
            return result;
        }
    }
}
