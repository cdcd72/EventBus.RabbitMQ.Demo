using Infra.Core.EventBus.Events;

namespace Receive.API.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public record TriggeredIntegrationEvent : IntegrationEvent
    {
        public string Input { get; init; }

        public TriggeredIntegrationEvent(string input)
            => Input = input;
    }
}
