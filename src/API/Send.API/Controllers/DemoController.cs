using System;
using System.Threading.Tasks;
using Infra.Core.EventBus.Abstractions;
using Infra.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Send.API.IntegrationEvents.Events;

namespace Send.API.Controllers;

[ApiController]
[Route("[controller]")]
public class DemoController(ILogger<DemoController> logger, IEventBus eventBus) : ControllerBase
{
    [Route(nameof(TriggerAsync))]
    [HttpPost]
    public async Task<ActionResult> TriggerAsync(string input)
    {
        var eventMessage = new TriggeredIntegrationEvent(input);

        try
        {
            eventBus.Publish(eventMessage);

            logger.Information($"Published integration event: {eventMessage.Id} from Send.API");
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"ERROR Publishing integration event: {eventMessage.Id} from Send.API");

            throw;
        }

        return Ok();
    }
}
