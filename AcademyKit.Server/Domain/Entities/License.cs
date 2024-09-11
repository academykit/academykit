namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class License : AuditableEntity
    {
        public string LicenseKey { get; set; }
        public LicenseStatusType Status { get; set; }
        public int LicenseKeyId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime ActivatedOn { get; set; }
        public DateTime ExpiredOn { get; set; }
        public string VariantName { get; set; }
        public int VariantId { get; set; }
    }
}
