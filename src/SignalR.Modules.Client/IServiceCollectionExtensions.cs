using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace SignalR.Modules.Client
{
    public static class IServiceCollectionExtensions
    {
        public static ModuleHubClientBuilder AddSignalRModules(this IServiceCollection services, string entryHubName, Uri hubUri)
        {
            return services.AddSignalRModules(entryHubName, options => options.Builder = builder => builder.UseModuleHubDefault(hubUri));
        }

        public static ModuleHubClientBuilder AddSignalRModules(this IServiceCollection services, string entryHubName, Func<IServiceProvider, Uri> hubUri)
        {
            return services.AddSignalRModules(entryHubName, (options, sp) => options.Builder = builder => builder.UseModuleHubDefault(hubUri(sp)));
        }

        public static ModuleHubClientBuilder AddSignalRModules(this IServiceCollection services, string entryHubName, Action<ModuleHubConnectionOptions> options)
        {
            return services.AddSignalRModules(entryHubName, (op, _) => options(op));
        }

        public static ModuleHubClientBuilder AddSignalRModules(this IServiceCollection services, string entryHubName, Action<ModuleHubConnectionOptions, IServiceProvider> options)
        {
            services.TryAddSingleton<ModuleHubConnectionManager>();

            services.AddOptions<ModuleHubConnectionOptions>(entryHubName)
                .Configure(options);

            var clientBuilder = new ModuleHubClientBuilder(services, entryHubName);

            return clientBuilder;
        }

        private static IHubConnectionBuilder UseModuleHubDefault(this IHubConnectionBuilder builder, Uri hubUri)
        {
            return builder
                .WithUrl(hubUri)
                .WithAutomaticReconnect();
        }
    }
}
