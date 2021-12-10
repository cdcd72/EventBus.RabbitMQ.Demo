using Infra.Core.EventBus;
using Infra.Core.EventBus.Abstractions;
using Infra.EventBus.RabbitMQ;
using Infra.EventBus.RabbitMQ.Abstractions;
using Infra.EventBus.RabbitMQ.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog.Web;

namespace Send.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .AddControllersAsServices();

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Send.API",
                Version = "v1"
            }));

            services.AddLogging(builder => builder.AddNLog("Nlog.config"));

            services.Configure<ConnectionSettings>(settings => Configuration.GetSection(ConnectionSettings.SectionName).Bind(settings))
                    .Configure<Settings>(settings => Configuration.GetSection(Settings.SectionName).Bind(settings));

            // Register event bus
            RegisterEventBus(services);
        }

        private static void RegisterEventBus(IServiceCollection services)
        {
            // Add RabbitMQ connection
            services.AddSingleton<IRabbitMQConnection, DefaultRabbitMQConnection>();

            // Add event bus subscriptions manager
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            // Add event bus
            services.AddSingleton<IEventBus, RabbitMQBus>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Send.API v1"));
            }

            if (env.IsProduction())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
