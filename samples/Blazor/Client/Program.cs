using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Modules.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherModule.Client;

namespace BlazorSignalR.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSignalRModules("MainHub", sp => sp.GetRequiredService<NavigationManager>().ToAbsoluteUri("/hub"))
                .AddModuleHubClient<ChatHubClient>()
                .AddModuleHubClient<WeatherHubClient>()
                .AddModuleHubClient<SignalRCounter>("CounterHub");

            await builder.Build().RunAsync();
        }
    }
}
