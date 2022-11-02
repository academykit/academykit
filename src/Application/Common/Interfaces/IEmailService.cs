using Lingtren.Application.Common.Dtos;

namespace Lingtren.Application.Common.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Handle to send forget password email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="firstName"></param>
        /// <param name="resetPasswordTokenLink"></param>
        /// <returns></returns>
        Task SendForgetPasswordEmail(string emailAddress, string firstName, string resetPasswordTokenLink);

        /// <summary>
        /// Handle to send mail with html body
        /// </summary>
        /// <param name="model">the instance of <see cref="EmailRequestDto"/></param>
        /// <returns></returns>
        Task SendMailWithHtmlBodyAsync(EmailRequestDto model);
    }
}
