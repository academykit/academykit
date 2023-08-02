using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Lingtren.Infrastructure.Configurations
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var isAuthorized = httpContext.User.Identity.IsAuthenticated;
            if (isAuthorized)
            {

            }
            return true;
        }
    }
}