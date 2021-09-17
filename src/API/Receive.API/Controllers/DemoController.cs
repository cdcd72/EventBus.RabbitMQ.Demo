using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Receive.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger<DemoController> _logger;

        public DemoController(ILogger<DemoController> logger) => _logger = logger;
    }
}
