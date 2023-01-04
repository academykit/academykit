namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using MailKit.Net.Smtp;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public class EmailService : IEmailService
    {
        private readonly string _footerEmail = "Sincerely,<br>- The Vurilo Team";
        private readonly ILogger<EmailService> _logger;
        private readonly ISMTPSettingService _smtpSettingService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _appUrl;

        public EmailService(
            ILogger<EmailService> logger,
            ISMTPSettingService smtpSettingService,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _smtpSettingService = smtpSettingService;
            _hostingEnvironment = hostingEnvironment;
            _appUrl = configuration.GetSection("AppUrls:App").Value;
        }

        public async Task SendMailWithHtmlBodyAsync(EmailRequestDto emailRequestDto)
        {
            try
            {
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, Path.Combine("Templates", "DefaultTemplate.html"));
                using StreamReader str = new(filePath);
                string htmlBody = str.ReadToEnd();

                var smtpSetting = await _smtpSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
                if (smtpSetting == null)
                {
                    _logger.LogWarning("SMTP Setting not found");
                    throw new EntityNotFoundException("SMTP Setting not found");
                }

                htmlBody = htmlBody.Replace("[content]", emailRequestDto.Message);
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(smtpSetting.SenderName, smtpSetting.SenderEmail));
                mimeMessage.To.Add(new MailboxAddress(emailRequestDto.To, emailRequestDto.To));
                mimeMessage.ReplyTo.Add(new MailboxAddress(smtpSetting.SenderName, smtpSetting.ReplyTo));
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
                await client.ConnectAsync(smtpSetting.MailServer, smtpSetting.MailPort, true);
                await client.AuthenticateAsync(smtpSetting.UserName, smtpSetting.Password);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to send email with html body contain.");
            }
        }

        /// <summary>
        /// Email for forget password
        /// </summary>
        /// <param name="emailAddress">the email address of the receiver</param>
        /// <param name="firstName">the first name of the receiver</param>
        /// <param name="resetToken">the reset token</param>
        /// <returns></returns>
        public async Task SendForgetPasswordEmail(string emailAddress, string firstName, string resetToken)
        {
            try
            {
                var html = $"Dear {firstName},<br><br>";
                html += $"Requested for password reset. <br> Your Token is <b><u>{resetToken}</u></b> for password reset. Token is valid for 5 minutes only.<br><br>";
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
                _logger.LogError(ex, "An error occurred while attempting to send forget password email.");
            }
        }

        /// <summary>
        /// Email for account created and password
        /// </summary>
        /// <param name="emailAddress">the email address of the receiver</param>
        /// <param name="firstName">the first name of the receiver</param>
        /// <param name="password">the login password of the receiver</param>
        /// <returns></returns>
        public async Task SendUserCreatedPasswordEmail(string emailAddress, string firstName, string password)
        {
            try
            {
                var html = $"Dear {firstName},<br><br>";
                html += $"Your account has been created in Vurilo Team. <br> Your Login Password is <b><u>{password}</u></b><br><br>";
                html += _footerEmail;

                var mail = new EmailRequestDto
                {
                    To = emailAddress,
                    Subject = "Account Created",
                    Message = html
                };
                await SendMailWithHtmlBodyAsync(mail).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to send change email address mail.");
            }
        }

        /// <summary>
        /// Email for account created and password
        /// </summary>
        /// <param name="email">the email address of the receiver</param>
        /// <param name="firstName">the first name of the receiver</param>
        /// <param name="token">the jwt token</param>
        /// <param name="expiredTime">the login password of the receiver</param>
        /// <returns></returns>
        public async Task SendChangePasswordMailAsync(string email, string firstName, string token, int expiredTime)
        {
            try
            {
                var html = $"Dear {firstName},<br><br>";
                html += @$"Please <a href='{_appUrl}/changeEmail?token={token}'> <u  style='color:blue;'>Click here</u></a> to change the email for E-learning. 
                                <br> The link will expire in {expiredTime} minute<br><br>";
                html += _footerEmail;

                var mail = new EmailRequestDto
                {
                    To = email,
                    Subject = "Change Email",
                    Message = html
                };
                await SendMailWithHtmlBodyAsync(mail).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to send forget password email.");
            }
        }
    }
}
