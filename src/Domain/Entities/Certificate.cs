namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class Certificate : AuditableEntity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string Institute { get; set; }
        public int? Duration { get; set; }
        public User User { get; set; }
        public CertificateStatus Status { get; set; }
        public decimal OptionalCost { get; set; }
    }
}
