namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;
    public class SMTPSetting : AuditableEntity
    {
        public string MailServer { get; set; }
        public string MailPort { get; set; }
        public string SenderEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReplayTo { get; set; }
        public bool UseSSL { get; set; }
        public User User { get; set; }
    }
}