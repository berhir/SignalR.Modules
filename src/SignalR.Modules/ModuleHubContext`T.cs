using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public class ModuleHubContext<TMainHub, TModuleHub, TModuleHubTyped> : IModuleHubContext<TModuleHub, TModuleHubTyped>
        where TMainHub : ModulesEntryHub
        where TModuleHub : ModuleHub<TModuleHubTyped>
        where TModuleHubTyped : class
    {
        private readonly IHubContext<TMainHub> _hubContext;

        public ModuleHubContext(IHubContext<TMainHub> hubContext)
        {
            _hubContext = hubContext;

            Groups = _hubContext.Groups;
            Clients = new ModuleHubTypedClients<TModuleHubTyped>(_hubContext.Clients);
        }

        public IHubClients<TModuleHubTyped> Clients { get; }

        public IGroupManager Groups { get; }
    }
}
