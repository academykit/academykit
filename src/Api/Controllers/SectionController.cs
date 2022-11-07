namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;

    public class SectionController : BaseApiController
    {
        private readonly ISectionService _sectionService;
        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }
    }
}