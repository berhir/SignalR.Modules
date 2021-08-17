using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public interface IModuleHubContext<TModuleHub, TModuleHubTyped>
        where TModuleHub : ModuleHub<TModuleHubTyped>
        where TModuleHubTyped : class
    {
        IHubClients<TModuleHubTyped> Clients { get; }

        IGroupManager Groups { get; }
    }
}
