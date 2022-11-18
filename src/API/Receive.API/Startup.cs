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
using Receive.API.IntegrationEvents.EventHandling;
using Receive.API.IntegrationEvents.Events;

namespace Receive.API
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
                Title = "Receive.API",
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
            services.AddSingleton<IRabbitMqConnection, DefaultRabbitMqConnection>();

            // Add event bus subscriptions manager
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            // Add event bus
            services.AddSingleton<IEventBus, RabbitMqBus>();

            // Add event handlers
            services.AddTransient<TriggeredIntegrationEventHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Receive.API v1"));
            }

            if (env.IsProduction())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            // Configure event bus
            ConfigureEventBus(app);
        }

        private static void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            // Subscribing to TriggeredIntegrationEvent with TriggeredIntegrationEventHandler.
            eventBus.Subscribe<TriggeredIntegrationEvent, TriggeredIntegrationEventHandler>();
        }
    }
}
