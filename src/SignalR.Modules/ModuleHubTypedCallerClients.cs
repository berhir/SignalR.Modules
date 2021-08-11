using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public class ModuleHubTypedCallerClients<TModuleHubClient> : ModuleHubTypedClients<TModuleHubClient>, IHubCallerClients<TModuleHubClient>
        where TModuleHubClient : class
    {
        private readonly IHubCallerClients hubCallerClients;

        public ModuleHubTypedCallerClients(IHubCallerClients hubCallerClients) :
            base(hubCallerClients)
        {
            this.hubCallerClients = hubCallerClients;
        }

        public TModuleHubClient Caller => CreateClient(hubCallerClients.Caller);

        public TModuleHubClient Others => CreateClient(hubCallerClients.Others);

        public TModuleHubClient OthersInGroup(string groupName)
        {
            return CreateClient(hubCallerClients.OthersInGroup(groupName));
        }
    }
}
