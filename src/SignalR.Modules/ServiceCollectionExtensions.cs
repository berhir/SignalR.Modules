using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace SignalR.Modules
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all SignalR modules specified in the <see cref="SignalRModuleHubAttribute"/>s of the entry hub.
        /// </summary>
        /// <typeparam name="TEntryHub">The hub that is used as the entry hub for all specified module hubs.</typeparam>
        /// <param name="services">The service collection.</param>
        public static void AddSignalRModules<TEntryHub>(this IServiceCollection services)
            where TEntryHub : ModulesEntryHub
        {
            var entryHubType = typeof(TEntryHub);

            var attributes = entryHubType.GetCustomAttributes(typeof(SignalRModuleHubAttribute), false)
                .Cast<SignalRModuleHubAttribute>();

            foreach (var attribute in attributes)
            {
                var moduleHubType = attribute.ModuleHubType;
                var baseType = moduleHubType.BaseType;

                if (baseType?.Equals(typeof(ModuleHub)) == true)
                {
                    services.AddTransient(moduleHubType);
                    var contextInterfaceType = typeof(IModuleHubContext<>).MakeGenericType(moduleHubType);
                    var contextImplementationType = typeof(ModuleHubContext<,>).MakeGenericType(entryHubType, moduleHubType);
                    services.AddSingleton(contextInterfaceType, contextImplementationType);
                }
                else if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition().Equals(typeof(ModuleHub<>)))
                {
                    services.AddTransient(moduleHubType);
                    var moduleHubTypedType = baseType.GetGenericArguments()[0];
                    var contextInterfaceType = typeof(IModuleHubContext<,>).MakeGenericType(moduleHubType, moduleHubTypedType);
                    var contextImplementationType = typeof(ModuleHubContext<,,>).MakeGenericType(entryHubType, moduleHubType, moduleHubTypedType);
                    services.AddSingleton(contextInterfaceType, contextImplementationType);
                }
                else
                {
                    throw new Exception($"SignalR module hub '{moduleHubType.Name}' must have a base of '{nameof(ModuleHub)}' or '{typeof(ModuleHub<>).Name}'");
                }
            }
        }
    }
}
