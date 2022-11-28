namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;

    public class MeetingController : BaseApiController
    {
        private readonly IMeetingService _meetingService;

        public MeetingController(
            IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }
    }
}
