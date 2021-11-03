using Microsoft.AspNetCore.SignalR;
using SignalR.Modules;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorSignalR.Server.SignalRModules
{
    public class CounterHub : ModuleHub
    {
        private static int _count = 0;

        [HubMethodName("CounterHub_IncrementCount")]
        public async Task<int> IncrementCountAsync()
        {
            var count = Interlocked.Increment(ref _count);
            await Clients.Others.SendAsync("ReceiveCount", _count);

            return count;
        }
    }
}
