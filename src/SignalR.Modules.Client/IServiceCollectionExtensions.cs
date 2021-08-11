using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace SignalR.Modules.Client
{
    public static class IServiceCollectionExtensions
    {
        private static IHubConnectionBuilder UseModuleHubDefault(this IHubConnectionBuilder builder, Uri hubUri)
        {
            return builder
                .WithUrl(hubUri)
                .WithAutomaticReconnect();
        }

        public static ModuleHubClientBuilder AddSignalRModules(this IServiceCollection services, string mainHubName, Uri hubUri)
        {
            return services.AddSignalRModules(mainHubName, options => options.Builder = builder => builder.UseModuleHubDefault(hubUri));
        }

        public static ModuleHubClientBuilder AddSignalRModules(this IServiceCollection services, string mainHubName, Func<IServiceProvider, Uri> hubUri)
        {
            return services.AddSignalRModules(mainHubName, (options, sp) => options.Builder = builder => builder.UseModuleHubDefault(hubUri(sp)));
        }

        public static ModuleHubClientBuilder AddSignalRModules(this IServiceCollection services, string mainHubName, Action<ModuleHubConnectionOptions> options)
        {
            return services.AddSignalRModules(mainHubName, (op, _) => options(op));
        }

        public static ModuleHubClientBuilder AddSignalRModules(this IServiceCollection services, string mainHubName, Action<ModuleHubConnectionOptions, IServiceProvider> options)
        {
            services.TryAddSingleton<ModuleHubConnectionManager>();

            services.AddOptions<ModuleHubConnectionOptions>(mainHubName)
                .Configure(options);

            var clientBuilder = new ModuleHubClientBuilder(services, mainHubName);

            return clientBuilder;
        }
    }
}
