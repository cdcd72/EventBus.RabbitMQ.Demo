using System;
using System.Threading.Tasks;
using Receive.API.IntegrationEvents.Events;
using Infra.Core.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Infra.Core.Extensions;

#pragma warning disable CA1711

namespace Receive.API.IntegrationEvents.EventHandling;

public class TriggeredIntegrationEventHandler(ILogger<TriggeredIntegrationEventHandler> logger) : IIntegrationEventHandler<TriggeredIntegrationEvent>
{
    private readonly ILogger<TriggeredIntegrationEventHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task HandleAsync(TriggeredIntegrationEvent integrationEvent)
    {
        _logger.Information($"Event is triggered! event: {integrationEvent.Id} input: {integrationEvent.Input}");

        await Task.CompletedTask;
    }
}
