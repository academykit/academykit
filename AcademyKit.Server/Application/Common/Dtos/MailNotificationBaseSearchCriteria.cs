using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Dtos
{
    public class MailNotificationBaseSearchCriteria : BaseSearchCriteria
    {
        public bool? IsActive { get; set; }
        public Guid? MailId { get; set; }
        public MailType? MailType { get; set; }
    }
}
