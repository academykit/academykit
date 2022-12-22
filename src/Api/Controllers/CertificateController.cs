namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/course/{identity}/certificate")]
    public class CertificateController : BaseApiController
    {
        private readonly ICourseService _courseService;
        public CertificateController(ICourseService courseService)
        {
            _courseService = courseService;
        }
    }
}
