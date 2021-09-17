using System;
using System.Threading.Tasks;
using Infra.Core.EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Send.API.IntegrationEvents.Events;

namespace Send.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger<DemoController> _logger;
        private readonly IEventBus _eventBus;

        public DemoController(ILogger<DemoController> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        [Route(nameof(TriggerAsync))]
        [HttpPost]
        public async Task<ActionResult> TriggerAsync(string input)
        {
            var eventMessage = new TriggeredIntegrationEvent(input);

            try
            {
                _eventBus.Publish(eventMessage);

                _logger.LogInformation($"Published integration event: {eventMessage.Id} from Send.API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR Publishing integration event: {eventMessage.Id} from Send.API");

                throw;
            }

            return Ok();
        }
    }
}
