using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public class ModuleHubUntypedCallerClients<TModuleHub> : ModuleHubTypedCallerClients<IClientProxy>, IHubClients, IHubCallerClients
        where TModuleHub : ModuleHub
    {
        public ModuleHubUntypedCallerClients(IHubCallerClients hubCallerClients)
            : base(hubCallerClients)
        {
        }

        protected override IClientProxy CreateClient(IClientProxy clientProxy)
        {
            return new UntypedClientProxy<TModuleHub>(clientProxy);
        }
    }
}
