using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class ServerLogsResponseModel
    {
        public Guid Id { get; set; }
        public SeverityType Type { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
        public string TrackBy { get; set; }
    }
}
