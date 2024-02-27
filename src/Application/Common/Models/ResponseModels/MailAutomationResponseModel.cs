using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class MailNotificationResponseModel
    {
        public Guid Id { get; set; }

        public string MailName { get; set; }
        public string MailSubject { get; set; }
        public string MailMessage { get; set; }
        public MailType MailType { get; set; }
        public bool IsActive { get; set; }

        public MailNotificationResponseModel(MailNotification model)
        {
            Id = model.Id;
            MailName = model.Name;
            MailSubject = model.Subject;
            MailMessage = model.Message;
            MailType = model.MailType;
            IsActive = model.IsActive;
        }

        public MailNotificationResponseModel() { }
    }
}
