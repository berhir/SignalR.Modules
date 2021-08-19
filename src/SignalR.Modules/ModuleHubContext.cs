using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public class ModuleHubContext<TEntryHub, TModuleHub> : IModuleHubContext<TModuleHub>
        where TEntryHub : ModulesEntryHub
        where TModuleHub : ModuleHub
    {
        private readonly IHubContext<TEntryHub> _hubContext;

        public ModuleHubContext(IHubContext<TEntryHub> hubContext)
        {
            _hubContext = hubContext;

            Groups = _hubContext.Groups;
            Clients = new ModuleHubUntypedClients<TModuleHub>(_hubContext.Clients);
        }

        public IHubClients Clients { get; }

        public IGroupManager Groups { get; }
    }
}
