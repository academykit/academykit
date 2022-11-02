namespace Lingtren.Api.Controllers
{
    using Lingtren.Api.Common;
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
                if (_currentUser == null)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        _currentUser = User.ToLoggedInUser();
                    }
                }
                return _currentUser;
            }
        }

        protected BaseApiController() { }
    }
}
