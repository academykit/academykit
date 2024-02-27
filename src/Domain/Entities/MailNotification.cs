using Lingtren.Domain.Common;
using Lingtren.Domain.Enums;

namespace Lingtren.Domain.Entities
{
    public class MailNotification : AuditableEntity
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public MailType MailType { get; set; }
    }
}
