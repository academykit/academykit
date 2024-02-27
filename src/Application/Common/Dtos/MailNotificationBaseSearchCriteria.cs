using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Dtos
{
    public class MailNotificationBaseSearchCriteria : BaseSearchCriteria
    {
        public bool? IsActive { get; set; }
        public Guid? MailId { get; set; }
        public MailType? MailType { get; set; }
    }
}
