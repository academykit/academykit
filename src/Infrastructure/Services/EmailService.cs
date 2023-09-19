namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Infrastructure.Localization;
    using MailKit.Net.Smtp;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly ISMTPSettingService _smtpSettingService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _appUrl;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;

        public EmailService(
            ILogger<EmailService> logger,
            ISMTPSettingService smtpSettingService,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration,
            IStringLocalizer<ExceptionLocalizer> localizer
            )
        {
            _logger = logger;
            _smtpSettingService = smtpSettingService;
            _hostingEnvironment = hostingEnvironment;
            _appUrl = configuration.GetSection("AppUrls:App").Value;
            _localizer = localizer;
        }

        public async Task SendMailWithHtmlBodyAsync(EmailRequestDto emailRequestDto)
        {
            try
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, Path.Combine("Templates", "DefaultTemplate.html"));
                using StreamReader str = new(filePath);
                var htmlBody = str.ReadToEnd();

                var smtpSetting = await _smtpSettingService.GetFirstOrDefaultAsync().ConfigureAwait(false);
                if (smtpSetting == null)
                {
                    _logger.LogWarning("SMTP Setting not found.");
                    throw new EntityNotFoundException(_localizer.GetString("SMTPSettingNotFound"));
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
        /// <param name="companyName"> the company name </param>
        /// <returns></returns>
        public async Task SendForgetPasswordEmail(string emailAddress, string firstName, string resetToken, string companyName)
        {
            try
            {
                var html = $"Dear {firstName},<br><br>";
                html += $"We have received a request for a password reset for your account. To proceed with the reset, please use the following token:<br>";
                html += $"Token: {resetToken}<br>";
                html += $"Please note that this token is valid for 5 minutes only. If you do not reset your password within this timeframe, you will need to request a new token.<br>";
                html += $"If you did not initiate this password reset request, please disregard this email and ensure the security of your account.<br>";
                html += $"Thank You.<br>Best regards,<br>{companyName}";

                var mail = new EmailRequestDto
                {
                    To = emailAddress,
                    Subject = "Password Reset Token for Your Account",
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
        /// <param name="email">the email address of the receiver</param>
        /// <param name="firstName">the first name of the receiver</param>
        /// <param name="token">the jwt token</param>
        /// <param name="expiredTime">the login password of the receiver</param>
        /// <param name="companyName"> the company name </param>
        /// <returns></returns>
        public async Task SendChangePasswordMailAsync(string email, string firstName, string token, int expiredTime, string companyName)
        {
            try
            {
                var html = $"Dear {firstName},<br><br>";
                html += @$"Your request to change the email address for LMS has been successfully processed. To finalize the update, please click on the provided link within {expiredTime} minutes.";
                html += @$" <a href='{_appUrl}/changeEmail?token={token}'> <u  style='color:blue;'>Click here</u></a> to change the email.<br>";
                html += $@"If you encounter any issues or have any questions, please don't hesitate to reach out to us.<br><br>";
                html += $"Thank You,<br> {companyName}";

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
