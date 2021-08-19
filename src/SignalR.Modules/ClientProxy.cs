using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Modules
{
    public abstract class ClientProxy
    {
        private readonly string _name;
        private readonly IClientProxy _clientProxy;

        public ClientProxy(string name, IClientProxy clientProxy)
        {
            _name = name;
            _clientProxy = clientProxy;
        }

        protected Task SendAsync(string name, object?[]? args, CancellationToken cancellationToken = default)
        {
            return _clientProxy.SendCoreAsync($"{_name}_{name}", args, cancellationToken);
        }
    }
}
