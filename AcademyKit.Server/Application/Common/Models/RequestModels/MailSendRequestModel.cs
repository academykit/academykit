using System.ComponentModel.DataAnnotations;

namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class MailSendRequestModel
    {
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
