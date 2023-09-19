// <copyright file="BaseApiController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

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
        /// The current user.
        /// </summary>
        private CurrentUser currentUser;

        protected BaseApiController()
        {
        }

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
        /// Checks if the user is superadmin.
        /// </summary>
        protected static void IsSuperAdmin(UserRole role)
        {
            if (role != UserRole.SuperAdmin)
            {
                throw new ForbiddenException("Super Admin Access");
            }
        }

        /// <summary>
        /// Checks if the user is superadmin or admin.
        /// </summary>
        protected static void IsSuperAdminOrAdmin(UserRole role)
        {
            if (role != UserRole.SuperAdmin && role != UserRole.Admin)
            {
                throw new ForbiddenException("Super Admin or Admin Access");
            }
        }

        /// <summary>
        /// Checks if the user is superadmin or admin or trainer.
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
