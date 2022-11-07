namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;
    public class LessonController : BaseApiController
    {
        private readonly ILessonService _lessonService;
        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }
    }
}