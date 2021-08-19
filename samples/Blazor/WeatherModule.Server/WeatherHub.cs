using Microsoft.Extensions.Logging;
using SignalR.Modules;
using System;
using System.Threading.Tasks;

namespace WeatherModule.Server
{
    public class WeatherHub : ModuleHub<IWeatherClient>
    {
        public const string WeatherUpdatesGroupName = "WeatherUpdates";

        private readonly ILogger<WeatherHub> _logger;

        public WeatherHub(ILogger<WeatherHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            // this is only for demo purpose. automatically subscribing is not recommended.
            // because weather data will be sent to the client even if the user is on a page that does not handle it.
            await Groups.AddToGroupAsync(Context.ConnectionId, WeatherUpdatesGroupName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, WeatherUpdatesGroupName);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendSubscribeWeatherUpdates()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, WeatherUpdatesGroupName);
            _logger.LogInformation($"Client with connection id '{Context.ConnectionId}' subscribed to weather updates");
        }

        public async Task SendUnsubscribeWeatherUpdates()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, WeatherUpdatesGroupName);
            _logger.LogInformation($"Client with connection id '{Context.ConnectionId}' unsubscribed from weather updates");
        }
    }
}
