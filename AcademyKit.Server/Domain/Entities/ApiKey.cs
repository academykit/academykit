using AcademyKit.Domain.Common;

namespace AcademyKit.Domain.Entities
{
    public class ApiKey : AuditableEntity
    {
        public string Key { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
