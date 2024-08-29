namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;

    /// <summary>
    /// Represents the general settings for an organization within the system.
    /// </summary>
    public class GeneralSetting : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the URL of the organization's logo.
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the physical address of the organization.
        /// </summary>
        public string CompanyAddress { get; set; }

        /// <summary>
        /// Gets or sets the contact number of the organization.
        /// </summary>
        public string CompanyContactNumber { get; set; }

        /// <summary>
        /// Gets or sets the email signature used by the organization.
        /// </summary>
        public string EmailSignature { get; set; }

        /// <summary>
        /// Gets or sets any custom configuration in a serialized format.
        /// </summary>
        public string CustomConfiguration { get; set; }

        /// <summary>
        /// Indicates whether the organization setup has been completed.
        /// </summary>
        public bool IsSetupCompleted { get; set; }

        /// <summary>
        /// Gets or sets the user associated with these settings.
        /// </summary>
        public User User { get; set; }
    }
}
