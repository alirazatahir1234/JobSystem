using Hangfire.Dashboard;

namespace JobSystem.Api
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // In development, allow all access
            // In production, you would check for proper authorization
            var httpContext = context.GetHttpContext();
            return httpContext.Request.Host.Host == "localhost" || 
                   Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        }
    }
}