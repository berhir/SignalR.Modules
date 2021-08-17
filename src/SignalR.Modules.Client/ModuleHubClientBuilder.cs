using Microsoft.Extensions.DependencyInjection;
using System;

namespace SignalR.Modules.Client
{
    public class ModuleHubClientBuilder
    {
        private const string ClientSuffix = "Client";

        private readonly IServiceCollection _services;

        public ModuleHubClientBuilder(IServiceCollection services, string mainHubName)
        {
            _services = services;
            MainHubName = mainHubName;
        }

        public string MainHubName { get; }

        public ModuleHubClientBuilder AddModuleHubClient<TClient>()
            where TClient : ModuleHubClient
        {
            return AddModuleHubClient<TClient>(GetModuleHubName(typeof(TClient)));
        }

        public ModuleHubClientBuilder AddModuleHubClient<TClient>(ServiceLifetime lifetime)
            where TClient : ModuleHubClient
        {
            return AddModuleHubClient<TClient>(GetModuleHubName(typeof(TClient)), lifetime);
        }

        public ModuleHubClientBuilder AddModuleHubClient<TClient>(string moduleHubName)
            where TClient : ModuleHubClient
        {
            return AddModuleHubClient<TClient>(moduleHubName, ServiceLifetime.Scoped);
        }

        public ModuleHubClientBuilder AddModuleHubClient<TClient>(string moduleHubName, ServiceLifetime lifetime)
            where TClient : ModuleHubClient
        {
            var item = new ServiceDescriptor(
                typeof(TClient),
                sp =>
                {
                    var client = ActivatorUtilities.CreateInstance<TClient>(sp);
                    client.Initialize(sp.GetRequiredService<ModuleHubConnectionManager>(), MainHubName, moduleHubName);
                    return client;
                },
                lifetime);

            _services.Add(item);

            return this;
        }

        private static string GetModuleHubName(Type clientType)
        {
            var name = clientType.Name;

            if (name.EndsWith(ClientSuffix))
            {
                return name.Substring(0, name.Length - ClientSuffix.Length);
            }

            return name;
        }
    }
}
