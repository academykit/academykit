namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;
    public class CourseController : BaseApiController
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }
    }
}