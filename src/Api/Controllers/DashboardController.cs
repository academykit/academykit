// <copyright file="DashboardController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : BaseApiController
    {
        private readonly ICourseService courseService;

        public DashboardController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        /// <summary>
        /// dashboard api.
        /// </summary>
        /// <returns> the list of <see cref="" /> .</returns>
        [HttpGet]
        public async Task<DashboardResponseModel> Get()
        {
            return await courseService
                .GetDashboardStats(CurrentUser.Id, CurrentUser.Role)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// dashboard course api.
        /// </summary>
        /// <returns> the list of <see cref="" /> .</returns>
        [HttpGet("course")]
        public async Task<SearchResult<DashboardCourseResponseModel>> GetCourses(
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            return await courseService
                .GetDashboardCourses(CurrentUser.Id, CurrentUser.Role, searchCriteria)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// dashboard api to get upcoming lesson.
        /// </summary>
        /// <returns> the list of lesson <see cref="Lesson" /> .</returns>
        [HttpGet("UpcomingLesson")]
        public async Task<List<DashboardLessonResponseModel>> GetUpcomingLesson()
        {
            return await courseService.GetUpcomingLesson(CurrentUser.Id).ConfigureAwait(false);
        }
    }
}
