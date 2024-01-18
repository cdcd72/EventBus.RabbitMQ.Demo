using Autofac.Extensions.DependencyInjection;
using Infra.Core.EventBus;
using Infra.Core.EventBus.Abstractions;
using Infra.EventBus.RabbitMQ;
using Infra.EventBus.RabbitMQ.Abstractions;
using Infra.EventBus.RabbitMQ.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog.Web;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var appConfigBuilder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

    builder.Configuration.AddConfiguration(appConfigBuilder.Build());

    builder.Host
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .UseNLog();

    builder.Services
        .AddControllers()
        .AddControllersAsServices();

    builder.Services
        .AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Send.API",
            Version = "v1"
        }))
        .AddLogging(loggingBuilder => loggingBuilder.AddNLog("Nlog.config"))
        .Configure<ConnectionSettings>(settings => builder.Configuration.GetSection(ConnectionSettings.SectionName).Bind(settings))
        .Configure<Settings>(settings => builder.Configuration.GetSection(Settings.SectionName).Bind(settings));

    // Register event bus
    RegisterEventBus(builder.Services);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Receive.API v1"));
    }

    if (app.Environment.IsProduction())
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

    return 0;
}
catch
{
    return 1;
}

static void RegisterEventBus(IServiceCollection services)
{
    // Add RabbitMQ connection
    services.AddSingleton<IRabbitMqConnection, DefaultRabbitMqConnection>();

    // Add event bus subscriptions manager
    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

    // Add event bus
    services.AddSingleton<IEventBus, RabbitMqBus>();
}
