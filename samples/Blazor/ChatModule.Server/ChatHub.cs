using Microsoft.AspNetCore.SignalR;
using SignalR.Modules;
using System.Threading.Tasks;

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
