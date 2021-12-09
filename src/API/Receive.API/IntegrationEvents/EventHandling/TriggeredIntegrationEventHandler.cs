using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Receive.API.IntegrationEvents.Events;
using Infra.Core.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Infra.Core.Extensions;

namespace Receive.API.IntegrationEvents.EventHandling
{
    [SuppressMessage("Naming", "CA1711", Justification = "<Suspended>")]
    public class TriggeredIntegrationEventHandler : IIntegrationEventHandler<TriggeredIntegrationEvent>
    {
        private readonly ILogger<TriggeredIntegrationEventHandler> _logger;

        public TriggeredIntegrationEventHandler(ILogger<TriggeredIntegrationEventHandler> logger)
            => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task HandleAsync(TriggeredIntegrationEvent integrationEvent)
        {
            _logger.Information($"Event is triggered! event: {integrationEvent.Id} input: {integrationEvent.Input}");

            await Task.CompletedTask;
        }
    }
}
