using Hangfire.Dashboard;

namespace SubmissionEvaluation
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.IsInRole("admin");
        }
    }
}
