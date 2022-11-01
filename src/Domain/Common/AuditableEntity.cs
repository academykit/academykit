namespace Lingtren.Domain.Common
{
    using System;

    /// <summary>
    /// AuditableEntity
    /// </summary>
    public abstract class AuditableEntity : IdentifiableEntity
    {
        /// <summary>
        /// Get or set created by
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get or set created on
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Get or set updated by
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Get or set updated on
        /// </summary>
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;


        /// <summary>
        /// Initializes a new instance of the <see cref="AuditableEntity"/> class.
        /// </summary>
        protected AuditableEntity()
        {
        }
    }
}
