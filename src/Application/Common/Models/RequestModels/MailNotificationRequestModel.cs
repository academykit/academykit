using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public class MailNotificationRequestModel
    {
        public string MailName { get; set; }
        public string MailSubject { get; set; }
        public string MailMessage { get; set; }
        public MailType MailType { get; set; }
        public bool IsActive { get; set; }
    }
}
