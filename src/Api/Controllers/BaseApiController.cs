namespace Lingtren.Api.Controllers
{
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Domain.Enums;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    // [ApiExplorerSettings(IgnoreApi = true)] 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// The current user
        /// </summary>
        private CurrentUser _currentUser;

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
                if (_currentUser == null && User.Identity.IsAuthenticated)
                {
                    _currentUser = User.ToLoggedInUser();
                }
                return _currentUser;
            }
        }

        protected BaseApiController() { }

        /// <summary>
        /// Checks if the user is admin or not.
        /// </summary>
        protected static void IsAdmin(UserRole role)
        {
            if (role != UserRole.Admin)
            {
                throw new ForbiddenException("Admin Access");
            }
        }
        /// <summary>
        /// Checks if the user is admin or teacher.
        /// </summary>
        protected static void IsTeacherAdmin(UserRole role)
        {
            if (role != UserRole.Admin && role != UserRole.Teacher)
            {
                throw new ForbiddenException("Admin or Teacher Access");
            }
        }

        /// <summary>
        /// Checks if the user is teacher or not.
        /// </summary>
        protected static void IsTeacher(UserRole role)
        {
            if (role != UserRole.Teacher)
            {
                throw new ForbiddenException("Teacher Access");
            }
        }
    }
}
