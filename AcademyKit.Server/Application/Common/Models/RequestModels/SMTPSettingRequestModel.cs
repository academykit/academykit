﻿namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class SMTPSettingRequestModel
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReplyTo { get; set; }
        public bool UseSSL { get; set; }
    }
}
