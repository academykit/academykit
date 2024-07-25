// <copyright file="EnrollmentController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Api.Controllers
{
    using Lingtren.Api.Common;
    using Lingtren.Api.Controllers;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
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
