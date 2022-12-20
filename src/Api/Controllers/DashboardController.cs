namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : BaseApiController
    {
        private readonly ICourseService _courseService;
        public DashboardController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// dashboard api
        /// </summary>
        /// <returns> the list of <see cref="" /> .</returns>
        [HttpGet]
        public async Task<DashboardResponseModel> Get()
        {
            return await _courseService.GetDashboardStats(CurrentUser.Id, CurrentUser.Role).ConfigureAwait(false);
        }

        /// <summary>
        /// dashboard course api
        /// </summary>
        /// <returns> the list of <see cref="" /> .</returns>
        [HttpGet("course")]
        public async Task<SearchResult<DashboardCourseResponseModel>> GetCourses([FromQuery] BaseSearchCriteria searchCriteria)
        {
            return await _courseService.GetDashboardCourses(CurrentUser.Id, CurrentUser.Role, searchCriteria).ConfigureAwait(false);
        }
    }
}
