using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Receive.API.Controllers;

[ApiController]
[Route("[controller]")]
public class DemoController(ILogger<DemoController> logger) : ControllerBase
{
    private readonly ILogger<DemoController> _logger = logger;
}
