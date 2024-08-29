namespace AcademyKit.Server.Application.Common.Models.RequestModels
{
    /// <summary>
    /// Represents the initial setup request containing user and company information.
    /// </summary>
    public class InitialSetupRequestModel
    {
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password for the user's account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmation of the password to ensure they match.
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the address of the company.
        /// </summary>
        public string CompanyAddress { get; set; }

        /// <summary>
        /// Gets or sets the logo url of the company.
        /// </summary>
        public string LogoUrl { get; set; }
    }
}
