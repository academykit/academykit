namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class License : AuditableEntity
    {
        public string licenseKey { get; set; }
        public LicenseStatusType status { get; set; }
        public int licenseKeyId { get; set; }
        public string customerName { get; set; }
        public string customerEmail { get; set; }
    }
}
