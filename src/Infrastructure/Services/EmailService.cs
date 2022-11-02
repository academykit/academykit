namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Infrastructure.Configurations;
    using MailKit.Net.Smtp;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MimeKit;

    public class EmailService : IEmailService
    {
        private readonly string _footerEmail = "Sincerely,<br>- The Vurilo Team";
        private readonly EmailSettings _emailSetting;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSetting,
            ILogger<EmailService> logger)
        {
            _emailSetting = emailSetting.Value;
            _logger = logger;
        }

        public async Task SendMailWithHtmlBodyAsync(EmailRequestDto emailRequestDto)
        {
            try
            {
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Templates", "DefaultTemplate.html"));
                using StreamReader str = new(FilePath);
                string htmlBody = str.ReadToEnd();

                htmlBody = htmlBody.Replace("[content]", emailRequestDto.Message);

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailSetting.SenderName, _emailSetting.Sender));
                mimeMessage.To.Add(new MailboxAddress(emailRequestDto.To, emailRequestDto.To));
                mimeMessage.ReplyTo.Add(new MailboxAddress(_emailSetting.SenderName, _emailSetting.ReplayTo));
                mimeMessage.Subject = emailRequestDto.Subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                mimeMessage.Body = builder.ToMessageBody();

                foreach (var item in emailRequestDto.Attachments)
                {
                    builder.Attachments.Add(item.FileName, item.File, item.ContentType);
                }

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSetting.MailServer, _emailSetting.MailPort, true);
                await client.AuthenticateAsync(_emailSetting.UserName, _emailSetting.Password);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
            }
        }

        public async Task SendForgetPasswordEmail(string emailAddress, string firstName, string resetToken)
        {
            try
            {

                var html = $"Dear {firstName},<br><br>";
                html += $"Your account requested for forgot password. <br> Your Token is <b><u>'{resetToken}'</u></b> for password reset.<br><br>";
                html += _footerEmail;

                var mail = new EmailRequestDto
                {
                    To = emailAddress,
                    Subject = "Forgot Password",
                    Message = html
                };
                await SendMailWithHtmlBodyAsync(mail).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message, ex);
            }
        }
    }
}
