using SignalR.Modules.Client;
using System.Threading.Tasks;
using WeatherModule.Shared;

namespace WeatherModule.Client
{
    public class WeatherHubClient : ModuleHubClient
    {
        public delegate void ReceivedWeatherUpdateEventHandler(object sender, ReceivedWeatherUpdateEventArgs e);

        public event ReceivedWeatherUpdateEventHandler? ReceivedWeatherUpdate;

        public async Task Subscribe()
        {
            await this.InvokeAsync("SendSubscribeWeatherUpdates");
        }

        public async Task Unubscribe()
        {
            await this.InvokeAsync("SendUnsubscribeWeatherUpdates");
        }

        protected override void OnInitialized()
        {
            this.On<WeatherForecast[]>("ReceiveWeatherUpdate", data =>
            {
                var ev = ReceivedWeatherUpdate;
                ev?.Invoke(this, new ReceivedWeatherUpdateEventArgs(data));
            });
            base.OnInitialized();
        }
    }
}
