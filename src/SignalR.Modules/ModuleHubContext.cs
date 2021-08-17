using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public class ModuleHubContext<TMainHub, TModuleHub> : IModuleHubContext<TModuleHub>
        where TMainHub : ModulesEntryHub
        where TModuleHub : ModuleHub
    {
        private readonly IHubContext<TMainHub> _hubContext;

        public ModuleHubContext(IHubContext<TMainHub> hubContext)
        {
            _hubContext = hubContext;

            Groups = _hubContext.Groups;
            Clients = new ModuleHubUntypedClients<TModuleHub>(_hubContext.Clients);
        }

        public IHubClients Clients { get; }

        public IGroupManager Groups { get; }
    }
}
