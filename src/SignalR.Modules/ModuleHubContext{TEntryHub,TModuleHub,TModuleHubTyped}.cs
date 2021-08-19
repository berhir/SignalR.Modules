using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public class ModuleHubContext<TEntryHub, TModuleHub, TModuleHubTyped> : IModuleHubContext<TModuleHub, TModuleHubTyped>
        where TEntryHub : ModulesEntryHub
        where TModuleHub : ModuleHub<TModuleHubTyped>
        where TModuleHubTyped : class
    {
        private readonly IHubContext<TEntryHub> _hubContext;

        public ModuleHubContext(IHubContext<TEntryHub> hubContext)
        {
            _hubContext = hubContext;

            Groups = _hubContext.Groups;
            Clients = new ModuleHubTypedClients<TModuleHubTyped>(_hubContext.Clients);
        }

        public IHubClients<TModuleHubTyped> Clients { get; }

        public IGroupManager Groups { get; }
    }
}
