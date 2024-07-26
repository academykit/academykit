using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class ServerLogsResponseModel
    {
        public int Id { get; set; }
        public SeverityType Type { get; set; }
        public string TimeStamp { get; set; }
        public string Message { get; set; }

        public ServerLogsResponseModel() { }

        public ServerLogsResponseModel(Logs log)
        {
            Id = log.Id;
            Type = (SeverityType)Enum.Parse(typeof(SeverityType), log.Level);
            Message = log.Message;
            TimeStamp = log.Logged.ToString("MM/dd/yyyy hh:mm tt");
        }
    }
}
