using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SignalR.Modules
{
    public class ModuleHubTypedClients<TModuleHubClient> : IHubClients<TModuleHubClient>
        where TModuleHubClient : class
    {
        private readonly IHubClients<IClientProxy> _hubCallerClients;

        public ModuleHubTypedClients(IHubClients<IClientProxy> hubContext)
        {
            _hubCallerClients = hubContext;
        }

        public TModuleHubClient All => CreateClient(_hubCallerClients.All);

        public TModuleHubClient AllExcept(IReadOnlyList<string> excludedConnectionIds)
        {
            return CreateClient(_hubCallerClients.AllExcept(excludedConnectionIds));
        }

        public TModuleHubClient Client(string connectionId)
        {
            return CreateClient(_hubCallerClients.Client(connectionId));
        }

        public TModuleHubClient Group(string groupName)
        {
            return CreateClient(_hubCallerClients.Group(groupName));
        }

        public TModuleHubClient GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds)
        {
            return CreateClient(_hubCallerClients.GroupExcept(groupName, excludedConnectionIds));
        }

        public TModuleHubClient Groups(IReadOnlyList<string> groupNames)
        {
            return CreateClient(_hubCallerClients.Groups(groupNames));
        }

        public TModuleHubClient User(string userId)
        {
            return CreateClient(_hubCallerClients.Users(userId));
        }

        public TModuleHubClient Users(IReadOnlyList<string> userIds)
        {
            return CreateClient(_hubCallerClients.Users(userIds));
        }

        TModuleHubClient IHubClients<TModuleHubClient>.Clients(IReadOnlyList<string> connectionIds)
        {
            return CreateClient(_hubCallerClients.Clients(connectionIds));
        }

        protected virtual TModuleHubClient CreateClient(IClientProxy clientProxy)
        {
            // todo: improve this. maybe a solution without reflection?
            var interfaceType = typeof(TModuleHubClient);

            var implType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => GetLoadableTypes(a))
                .Single(t => interfaceType.IsAssignableFrom(t) && !t.IsAbstract);
            return (TModuleHubClient)Activator.CreateInstance(implType, clientProxy)!;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).Select(t => t!);
            }
        }
    }
}
