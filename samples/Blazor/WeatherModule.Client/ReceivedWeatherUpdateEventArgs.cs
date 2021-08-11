using WeatherModule.Shared;

namespace WeatherModule.Client
{
    public class ReceivedWeatherUpdateEventArgs
    {
        public WeatherForecast[] Data { get; private set; }

        public ReceivedWeatherUpdateEventArgs(WeatherForecast[] weatherForecast)
        {
            Data = weatherForecast;
        }
    }
}
