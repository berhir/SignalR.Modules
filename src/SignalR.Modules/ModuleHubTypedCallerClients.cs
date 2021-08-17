using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public class ModuleHubTypedCallerClients<TModuleHubClient> : ModuleHubTypedClients<TModuleHubClient>, IHubCallerClients<TModuleHubClient>
        where TModuleHubClient : class
    {
        private readonly IHubCallerClients _hubCallerClients;

        public ModuleHubTypedCallerClients(IHubCallerClients hubCallerClients)
            : base(hubCallerClients)
        {
            _hubCallerClients = hubCallerClients;
        }

        public TModuleHubClient Caller => CreateClient(_hubCallerClients.Caller);

        public TModuleHubClient Others => CreateClient(_hubCallerClients.Others);

        public TModuleHubClient OthersInGroup(string groupName)
        {
            return CreateClient(_hubCallerClients.OthersInGroup(groupName));
        }
    }
}
