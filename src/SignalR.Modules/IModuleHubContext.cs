using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public interface IModuleHubContext<THub>
        where THub : ModuleHub
    {
        IHubClients Clients { get; }

        IGroupManager Groups { get; }
    }
}
