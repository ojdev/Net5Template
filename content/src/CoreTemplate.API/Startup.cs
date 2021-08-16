#if (hangfire)
using Hangfire;
using Hangfire.PostgreSql;
#endif
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
#if (pg)
using Microsoft.EntityFrameworkCore;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
#if (rabbitmq)
using RabbitMQ.EventBus.AspNetCore.Configurations;
#endif
using System;
using FluentValidation.AspNetCore;
using System.Reflection;
using System.Threading.Tasks;
using CoreTemplate.API.Extensions;
using CoreTemplate.API.Infrastructure;
using CoreTemplate.API.Infrastructure.Filters;
#if (es)
using CoreTemplate.Infrastructure.ES;
using CoreTemplate.Infrastructure.ES.Extensions;
using Elasticsearch.Net;
#endif
using System.Collections.Generic;
using Nest;
using Microsoft.Extensions.DependencyInjection.Extensions;
#if (pg)
using CoreTemplate.Infrastructure.NpgSql;
#endif
namespace CoreTemplate.API
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
#if (es)
        /// <summary>
        /// 
        /// </summary>
        public static ESConnectionStringConfiguration _ESConfiguration;
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

#if (es)
            _ESConfiguration = new ESConnectionStringConfiguration();
            Configuration.Bind(_ESConfiguration);
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.Filters.Add(typeof(ApiResponseFilterAttribute));
            })
               .AddNewtonsoftJson()
               .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining(typeof(Startup)));

            services.AddHttpContextAccessor();

            services.AddHeaderPropagation();

            services.AddAutoMapper(typeof(AutomapperConfigs));

            services.AddMediatR(typeof(Startup).Assembly);

            AddHealthChecks(services);

#if (pg)
            AddEFCore(services);
#endif

#if (es)
            AddES(services);
#endif
#if (hangfire)
            if (Configuration.GetValue<bool>("Hangfire:Enabled"))
            {
                AddHangfire(services);
            }
#endif

#if (rabbitmq)
            AddRabbitMQEventBus(services);
#endif

#if (redis)
            RedisCache(services);
#endif

            AddNSwag(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
#if (es)
        /// <param name="elastic"></param>
#endif
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
#if (es)
            ,IElasticClient elastic
#endif
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

#if (pg)
            DataSeederConfigure(app);
#endif
#if (es)
            elastic.Run<T>();
#endif
            app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.yaml")
               .UseSwaggerUi3(p => p.DocumentPath = "/swagger/{documentName}/swagger.yaml")
               .UseReDoc(c =>
               {
                   c.DocumentPath = "/swagger/v1/swagger.yaml";
                   c.Path = "/redoc";
               })
               ;

            app.UseHeaderPropagation();
#if (rabbitmq)
            app.RabbitMQEventBusAutoSubscribe();
#endif
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });

#if (hangfire)
                if (Configuration.GetValue<bool>("Hangfire:Enabled"))
                {
                    endpoints.MapHangfireDashboard(new DashboardOptions
                    {
                        Authorization = new[] { new HangfireAuthorizationFilter() }
                    });
                }
#endif
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }

        /// <summary>
        /// 健康检查
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual IServiceCollection AddHealthChecks(IServiceCollection services)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

#if (pg)
            hcBuilder.AddNpgSql(Configuration["ConnectionStrings:Default"]);
#endif

#if (es)
            hcBuilder.AddElasticsearch(setup =>
            {
                setup.UseServer(_ESConfiguration.ElasticSearch.Host); if (!string.IsNullOrWhiteSpace(_ESConfiguration.ElasticSearch?.Username) && !string.IsNullOrWhiteSpace(_ESConfiguration.ElasticSearch?.Password))
                {
                    setup.UseBasicAuthentication(_ESConfiguration.ElasticSearch.Username, _ESConfiguration.ElasticSearch.Password);
                }
            });
#endif
#if (rabbitmq)
            var ampq = $"amqp://{Configuration["RabbitMQService:UserName"]}:{Configuration["RabbitMQService:Password"]}@{Configuration["RabbitMQService:ServiceName"]}/";
            hcBuilder.AddRabbitMQ(ampq, null, null, null, null, null);
#endif
            return services;
        }
#if (redis)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual IServiceCollection RedisCache(IServiceCollection services)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = "";
                options.Configuration = $"{Configuration["BuildingCache:Host"]}:{Configuration["BuildingCache:Port"]},password={Configuration["BuildingCache:Password"]}";
            });
            return services;
        }
#endif

#if (es)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual IServiceCollection AddES(IServiceCollection services)
        {
            var pool = new StaticConnectionPool(new List<Uri>() { new Uri(_ESConfiguration.ElasticSearch.Host) });
            var settings = new ConnectionSettings(pool);
            if (!string.IsNullOrWhiteSpace(_ESConfiguration.ElasticSearch?.Username) && !string.IsNullOrWhiteSpace(_ESConfiguration.ElasticSearch?.Password))
            {
                settings.BasicAuthentication(_ESConfiguration.ElasticSearch.Username, _ESConfiguration.ElasticSearch.Password);
            }
            services.TryAddSingleton<IElasticClient>(options =>
            {
                var client = new ElasticClient(settings);
                return client;
            });
            return services;
        }
#endif
        /// <summary>
        /// Nswag
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual IServiceCollection AddNSwag(IServiceCollection services)
        {
            #region swagger - nswag
            services.AddOpenApiDocument(c =>
            {
                c.PostProcess = document =>
                {
                    document.Info.Title = "CoreTemplate.API";
                    document.Info.Version = "v1";
                    document.Info.Description = "Swagger描述";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "鹿径",
                        Email = "admin@lujing.tech",
                        Url = "https://www.lujing.tech/"
                    };
                    document.Info.TermsOfService = "redoc";
                    document.Info.License = new OpenApiLicense
                    {
                        Name = "许可证",
                        Url = "https://www.lujing.tech/"
                    };
                };
                //c.AddCustomHeaders();
            });
            #endregion
            return services;
        }

#if (pg)
        /// <summary>
        /// 配置EntityFramework
        /// 配合集成测试，该方法可重写
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual IServiceCollection AddEFCore(IServiceCollection services)
        {
            services
                .AddDbContext<InfrastructureContext>(options =>
                {
                    options.UseLazyLoadingProxies()
                        .UseNpgsql(Configuration["ConnectionStrings:Default"],
                            npgsqlOptionsAction: sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                            });
                },
                ServiceLifetime.Scoped);
            return services;
        }
#endif
#if (rabbitmq)
        /// <summary>
        /// 配置EventBus，RabbitMQ
        /// 配合集成测试，该方法可重写
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual IServiceCollection AddRabbitMQEventBus(IServiceCollection services)
        {
            var ampqURI = $"amqp://{Configuration["RabbitMQService:UserName"]}:{Configuration["RabbitMQService:Password"]}@{Configuration["RabbitMQService:ServiceName"]}/";
            services.AddRabbitMQEventBus(() => ampqURI, r =>
            {
                r.ClientProvidedAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                r.EnableRetryOnFailure(true, 5000, TimeSpan.FromSeconds(30));
                r.RetryOnFailure(TimeSpan.FromSeconds(30));
                r.QueuePrefix(QueuePrefixType.ClientProvidedName);
                r.SetBasicQos(1000);
                r.DeadLetterExchangeConfig(c =>
                {
                    c.Enabled = false;
                });
            });
            return services;
        }
#endif
#if (hangfire)
        /// <summary>
        /// Hangfire配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public virtual IServiceCollection AddHangfire(IServiceCollection services)
        {
            services.AddHangfire(options =>
            {
                options.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(Configuration["Hangfire:ConnectionString"]);
            });
            services.AddHangfireServer();
            return services;
        }
#endif
#if (pg)
        /// <summary>
        /// 数据初始化
        /// 配合集成测试，该方法可重写
        /// </summary>
        /// <param name="app"></param>
        public virtual void DataSeederConfigure(IApplicationBuilder app)
        {
            app.MigrateDbContext<InfrastructureContext>((context, services) =>
            {
                var logger = services.GetService<ILogger<InitialSeed>>();
                new InitialSeed().SeedAsync(context, logger).Wait();
            });
        }
#endif
    }
}
