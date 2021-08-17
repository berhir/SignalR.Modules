using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;

namespace SignalR.Modules
{
    public abstract class ModulesEntryHub : Hub
    {
        protected ILogger Logger { get; }

        protected IServiceProvider ServiceProvider { get; }

        public ModulesEntryHub(ILogger logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        protected void InitModuleHub<TModuleHub>(TModuleHub hub)
            where TModuleHub : ModuleHub
        {
            hub.Context = Context;
            hub.Groups = Groups;
            hub.Clients = new ModuleHubUntypedCallerClients<TModuleHub>(Clients);
        }

        protected void InitModuleHub<TModuleHubClient>(ModuleHub<TModuleHubClient> moduleHub)
            where TModuleHubClient : class
        {
            moduleHub.Context = Context;
            moduleHub.Groups = Groups;
            moduleHub.Clients = new ModuleHubTypedCallerClients<TModuleHubClient>(Clients);
        }
    }
}
