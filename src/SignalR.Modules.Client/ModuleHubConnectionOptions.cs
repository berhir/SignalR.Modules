using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace SignalR.Modules.Client
{
    public class ModuleHubConnectionOptions
    {
        public Action<IHubConnectionBuilder> Builder { get; set; }
    }
}
