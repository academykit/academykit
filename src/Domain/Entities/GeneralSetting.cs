namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class GeneralSetting : AuditableEntity
    {
        public string LogoUrl { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContactNumber { get; set; }
        public string EmailSignature { get; set; }
        public User User { get; set; }
        public string CustomConfiguration { get; set; }
    }
}