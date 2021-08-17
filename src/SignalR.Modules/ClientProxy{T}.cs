using Microsoft.AspNetCore.SignalR;

namespace SignalR.Modules
{
    public abstract class ClientProxy<T> : ClientProxy
        where T : ModuleHub
    {
        public ClientProxy(IClientProxy clientProxy)
            : base(typeof(T).Name, clientProxy)
        {
        }
    }
}
