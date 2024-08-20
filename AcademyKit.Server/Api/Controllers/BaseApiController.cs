namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Api.Common;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Domain.Enums;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    // [ApiExplorerSettings(IgnoreApi = true)]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// The current user.
        /// </summary>
        private CurrentUser currentUser;

        protected BaseApiController() { }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <value>
        /// The current user.
        /// </value>
        protected CurrentUser CurrentUser
        {
            get
            {
                if (currentUser == null && User.Identity.IsAuthenticated)
                {
                    currentUser = User.ToLoggedInUser();
                }

                return currentUser;
            }
        }

        /// <summary>
        /// Checks if the user is super admin.
        /// </summary>
        protected static void IsSuperAdmin(UserRole role)
        {
            if (role != UserRole.SuperAdmin)
            {
                throw new ForbiddenException("Super Admin Access");
            }
        }

        /// <summary>
        /// Checks if the user is super admin or admin.
        /// </summary>
        protected static void IsSuperAdminOrAdmin(UserRole role)
        {
            if (role != UserRole.SuperAdmin && role != UserRole.Admin)
            {
                throw new ForbiddenException("Super Admin or Admin Access");
            }
        }

        /// <summary>
        /// Checks if the user is super admin or admin or trainer.
        /// </summary>
        protected static void IsSuperAdminOrAdminOrTrainer(UserRole role)
        {
            if (role != UserRole.SuperAdmin && role != UserRole.Admin && role != UserRole.Trainer)
            {
                throw new ForbiddenException("Super Admin or Admin or Trainer Access");
            }
        }

        /// <summary>
        /// Checks if the user is trainer.
        /// </summary>
        protected static void IsTrainer(UserRole role)
        {
            if (role != UserRole.Trainer)
            {
                throw new ForbiddenException("Trainer Access");
            }
        }
    }
}
