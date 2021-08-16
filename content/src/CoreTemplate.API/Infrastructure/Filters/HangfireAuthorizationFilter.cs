using Hangfire.Dashboard;

namespace CoreTemplate.API.Infrastructure.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// 这里需要配置权限规则
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
