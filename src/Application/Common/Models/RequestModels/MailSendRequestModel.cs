using System.ComponentModel.DataAnnotations;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public class MailSendRequestModel
    {
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
