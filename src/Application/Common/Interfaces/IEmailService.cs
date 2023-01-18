namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    public interface IEmailService
    {
        /// <summary>
        /// Handle to send mail with html body
        /// </summary>
        /// <param name="model">the instance of <see cref="EmailRequestDto"/></param>
        /// <returns></returns>
        Task SendMailWithHtmlBodyAsync(EmailRequestDto model);

        /// <summary>
        /// Email for forget password
        /// </summary>
        /// <param name="emailAddress">the email address of the receiver</param>
        /// <param name="firstName">the first name of the receiver</param>
        /// <param name="resetToken">the reset token</param>
        /// <param name="companyName"> the company name </param>
        /// <returns></returns>
        Task SendForgetPasswordEmail(string emailAddress, string firstName, string resetToken, string companyName);

        /// <summary>
        /// Email for account created and password
        /// </summary>
        /// <param name="emailAddress">the email address of the receiver</param>
        /// <param name="firstName">the first name of the receiver</param>
        /// <param name="password">the login password of the receiver</param>
        /// <param name="companyName"> the company name </param>
        /// <returns></returns>
        Task SendUserCreatedPasswordEmail(string emailAddress, string firstName, string password, string companyName);

        /// <summary>
        /// Email for account created and password
        /// </summary>
        /// <param name="email">the email address of the receiver</param>
        /// <param name="firstName">the first name of the receiver</param>
        /// <param name="token">the jwt token</param>
        /// <param name="expiredTime">the login password of the receiver</param>
        /// <param name="companyName"> the company name </param>
        /// <returns></returns>
        Task SendChangePasswordMailAsync(string email, string firstName, string token, int expiredTime, string companyName);
    }
}
