using CoreTemplate.API;
#if (pg)
using CoreTemplate.Infrastructure.NpgSql;
#endif
using IntegrationTests.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
#if (pg)
using Microsoft.EntityFrameworkCore;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
#if (rabbitmq)
using RabbitMQ.EventBus.AspNetCore;
#endif

namespace IntegrationTests
{
    public class IntegrationTestStartup : Startup
    {
        public IntegrationTestStartup(IConfiguration configuration) : base(configuration)
        {
        }
#if (pg)
        public override IServiceCollection AddEFCore(IServiceCollection services)
        {
            var connection = new SqliteConnection(new SqliteConnectionStringBuilder() { DataSource = ":memory:" }.ConnectionString);
            connection.Open();
            services.AddDbContext<InfrastructureContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlite(connection);
            });
            services.AddTransient<DataSeeder>();
            return services;
        }
#endif
#if (rabbitmq)
        public override IServiceCollection AddRabbitMQEventBus(IServiceCollection services)
        {
            services.AddScoped<IRabbitMQEventBus>(x => new Mock<IRabbitMQEventBus>().Object);
            return services;
        }
#endif
#if (pg)
        public override void DataSeederConfigure(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var seeder = serviceScope.ServiceProvider.GetService<DataSeeder>();
            seeder.Seed();
        }
    }
    #endif
}
