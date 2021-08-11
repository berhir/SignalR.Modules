using ChatModule.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalR.Modules;

namespace BlazorSignalR.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IModuleHubContext<ChatHub> _hubContext;

        public ChatController(ILogger<ChatController> logger, IModuleHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpGet]
        public ActionResult SendMessage(string user, string message)
        {
            _logger.LogInformation($"Received message '{message}' from user '{user}' via HTTP and send it to all users via SignalR");
            _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
            return Ok();
        }
    }
}
