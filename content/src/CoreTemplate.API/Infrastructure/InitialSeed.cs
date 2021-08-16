using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CoreTemplate.Infrastructure.NpgSql;

namespace CoreTemplate.API.Infrastructure
{
    /// <summary>
    /// 数据预处理
    /// </summary>
    public class InitialSeed
    { /// <summary>
      /// 
      /// </summary>
      /// <param name="context"></param>
      /// <param name="logger"></param>
      /// <returns></returns>
        public async Task SeedAsync(InfrastructureContext context, ILogger<InitialSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(InitialSeed));
            await policy.ExecuteAsync(async () =>
            {
                using (context)
                {
                    //数据初始化
                    await context.SaveChangesAsync();
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="prefix"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        private AsyncRetryPolicy CreatePolicy(ILogger<InitialSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
