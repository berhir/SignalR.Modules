using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace SignalR.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSignalRModuleHub<THub>(this IServiceCollection services)
            where THub : ModulesEntryHub
        {
            var mainHubType = typeof(THub);

            var attributes = mainHubType.GetCustomAttributes(typeof(SignalRModuleHubAttribute), false)
                .Cast<SignalRModuleHubAttribute>();

            foreach (var attribute in attributes)
            {
                var moduleHubType = attribute.ModuleHubType;
                var baseType = moduleHubType.BaseType;

                if (baseType?.Equals(typeof(ModuleHub)) == true)
                {
                    services.AddTransient(moduleHubType);
                    var contextInterfaceType = typeof(IModuleHubContext<>).MakeGenericType(moduleHubType);
                    var contextImplementationType = typeof(ModuleHubContext<,>).MakeGenericType(mainHubType, moduleHubType);
                    services.AddSingleton(contextInterfaceType, contextImplementationType);
                }
                else if(baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition().Equals(typeof(ModuleHub<>)))
                {
                    services.AddTransient(moduleHubType);
                    var moduleHubTypedType = baseType.GetGenericArguments()[0];
                    var contextInterfaceType = typeof(IModuleHubContext<,>).MakeGenericType(moduleHubType, moduleHubTypedType);
                    var contextImplementationType = typeof(ModuleHubContext<,,>).MakeGenericType(mainHubType, moduleHubType, moduleHubTypedType);
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
