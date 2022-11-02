namespace Lingtren.Infrastructure.Configurations
{
    public class EmailSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string Sender { get; set; }
        public string ReplayTo { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string App { get; set; }
    }
}
