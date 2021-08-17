using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public class ModuleHubUntypedClients<TModuleHub> : ModuleHubTypedClients<IClientProxy>, IHubClients
        where TModuleHub : ModuleHub
    {
        public ModuleHubUntypedClients(IHubClients<IClientProxy> hubContext)
            : base(hubContext)
        {
        }

        protected override IClientProxy CreateClient(IClientProxy clientProxy)
        {
            return new UntypedClientProxy<TModuleHub>(clientProxy);
        }
    }
}
