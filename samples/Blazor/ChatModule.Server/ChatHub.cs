using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using SignalR.Modules;

namespace ChatModule.Server
{
    public class ChatHub : ModuleHub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
