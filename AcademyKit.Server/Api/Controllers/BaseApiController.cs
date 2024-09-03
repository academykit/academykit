namespace AcademyKit.Api.Controllers
{
    using System.Text.Encodings.Web;
    using AcademyKit.Api.Common;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Configurations;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The base API controller providing common functionalities for all controllers.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        private AppUrls _appUrls;

        /// <summary>
        /// The current user of the application.
        /// </summary>
        private CurrentUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiController"/> class with application URLs configuration.
        /// </summary>
        protected BaseApiController() { }

        protected void InitializeAppUrls(IOptions<AppUrls> appUrls)
        {
            _appUrls = appUrls.Value;
        }

        /// <summary>
        /// Gets the currently logged-in user.
        /// </summary>
        protected CurrentUser CurrentUser
        {
            get
            {
                if (_currentUser == null && User.Identity.IsAuthenticated)
                {
                    _currentUser = User.ToLoggedInUser();
                }

                return _currentUser;
            }
        }

        /// <summary>
        /// Gets the application URLs configuration.
        /// </summary>
        protected AppUrls AppUrls => _appUrls;

        /// <summary>
        /// Ensures that the user has Super Admin privileges.
        /// </summary>
        /// <param name="role">The role of the current user.</param>
        /// <exception cref="ForbiddenException">Thrown when the user is not a Super Admin.</exception>
        protected static void IsSuperAdmin(UserRole role)
        {
            if (role != UserRole.SuperAdmin)
            {
                throw new ForbiddenException("Super Admin Access Required");
            }
        }

        /// <summary>
        /// Ensures that the user has Super Admin or Admin privileges.
        /// </summary>
        /// <param name="role">The role of the current user.</param>
        /// <exception cref="ForbiddenException">Thrown when the user is neither a Super Admin nor an Admin.</exception>
        protected static void IsSuperAdminOrAdmin(UserRole role)
        {
            if (role != UserRole.SuperAdmin && role != UserRole.Admin)
            {
                throw new ForbiddenException("Super Admin or Admin Access Required");
            }
        }

        /// <summary>
        /// Ensures that the user has Super Admin, Admin, or Trainer privileges.
        /// </summary>
        /// <param name="role">The role of the current user.</param>
        /// <exception cref="ForbiddenException">Thrown when the user is not a Super Admin, Admin, or Trainer.</exception>
        protected static void IsSuperAdminOrAdminOrTrainer(UserRole role)
        {
            if (role != UserRole.SuperAdmin && role != UserRole.Admin && role != UserRole.Trainer)
            {
                throw new ForbiddenException("Super Admin, Admin, or Trainer Access Required");
            }
        }

        /// <summary>
        /// Ensures that the user has Trainer privileges.
        /// </summary>
        /// <param name="role">The role of the current user.</param>
        /// <exception cref="ForbiddenException">Thrown when the user is not a Trainer.</exception>
        protected static void IsTrainer(UserRole role)
        {
            if (role != UserRole.Trainer)
            {
                throw new ForbiddenException("Trainer Access Required");
            }
        }

        /// <summary>
        /// Redirects to the specified page in the frontend application.
        /// </summary>
        /// <param name="url">The relative URL of the frontend page.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the frontend page.</returns>
        protected IActionResult RedirectToFrontend(string url)
        {
            return Redirect(url);
        }

        /// <summary>
        /// Redirects to the error page in the frontend application with the specified error message and details.
        /// </summary>
        /// <param name="errorMessage">The error message to display.</param>
        /// <param name="errorDetails">Additional details about the error.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the error page.</returns>
        protected IActionResult RedirectToErrorPage(string errorMessage, string errorDetails)
        {
            return Redirect(
                $"{_appUrls.App}/error?error={UrlEncoder.Default.Encode(errorMessage)}&details={UrlEncoder.Default.Encode(errorDetails)}"
            );
        }
    }
}
