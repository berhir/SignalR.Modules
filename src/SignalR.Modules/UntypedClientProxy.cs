using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Modules
{
    public class UntypedClientProxy<TModuleHub> : ClientProxy<TModuleHub>, IClientProxy
        where TModuleHub : ModuleHub
    {
        public UntypedClientProxy(IClientProxy clientProxy)
            : base(clientProxy)
        {
        }

        public Task SendCoreAsync(string method, object?[]? args, CancellationToken cancellationToken = default)
        {
            return SendAsync(method, args, cancellationToken);
        }
    }
}
