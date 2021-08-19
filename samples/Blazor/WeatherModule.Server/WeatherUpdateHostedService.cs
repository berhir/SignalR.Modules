using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalR.Modules;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherModule.Shared;

namespace WeatherModule.Server
{
    public sealed class WeatherUpdateHostedService : IHostedService, IDisposable
    {
        private static readonly string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private readonly ILogger<WeatherUpdateHostedService> _logger;
        private readonly IModuleHubContext<WeatherHub, IWeatherClient> _hubContext;
        private Timer? _timer;

        public WeatherUpdateHostedService(ILogger<WeatherUpdateHostedService> logger, IModuleHubContext<WeatherHub, IWeatherClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(WeatherUpdateHostedService)} is running.");

            _timer = new Timer(SendWeatherUpdate, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(WeatherUpdateHostedService)} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void SendWeatherUpdate(object? state)
        {
            var rng = new Random();
            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = _summaries[rng.Next(_summaries.Length)],
            })
            .ToArray();

            _hubContext.Clients.Group(WeatherHub.WeatherUpdatesGroupName).ReceiveWeatherUpdate(forecast);

            _logger.LogInformation(
                $"{nameof(WeatherUpdateHostedService)} sent an update");
        }
    }
}
