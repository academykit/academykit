namespace AcademyKit.Domain.Common
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
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Get or set created on
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Get or set updated by
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// Get or set updated on
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditableEntity"/> class.
        /// </summary>
        protected AuditableEntity() { }
    }
}
