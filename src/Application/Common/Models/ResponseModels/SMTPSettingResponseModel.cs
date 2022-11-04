using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class SMTPSettingResponseModel
    {
        public Guid Id { get; set; }
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReplayTo { get; set; }
        public bool UseSSL { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserResponseModel User { get; set; }
        public SMTPSettingResponseModel(SMTPSetting model)
        {
            Id = model.Id;
            MailServer = model.MailServer;
            MailPort = model.MailPort;
            SenderName = model.SenderName;
            SenderEmail = model.SenderEmail;
            UserName = model.UserName;
            Password = model.Password;
            ReplayTo = model.ReplayTo;
            UseSSL = model.UseSSL;
            UpdatedOn = model.UpdatedOn;
            User = model.User != null ? new UserResponseModel(model.User) : new UserResponseModel();
        }
    }
}
