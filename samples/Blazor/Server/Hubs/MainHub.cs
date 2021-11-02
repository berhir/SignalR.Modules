using BlazorSignalR.Server.SignalRModules;
using ChatModule.Server;
using SignalR.Modules;
using System;
using WeatherModule.Server;

namespace BlazorSignalR.Server.Hubs
{
    [SignalRModuleHub(typeof(ChatHub))]
    [SignalRModuleHub(typeof(WeatherHub))]
    [SignalRModuleHub(typeof(CounterHub))]
    public partial class MainHub : ModulesEntryHub
    {
        public MainHub(IServiceProvider provider)
            : base(provider)
        {
        }
    }
}
