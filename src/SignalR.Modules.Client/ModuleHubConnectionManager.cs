using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalR.Modules.Client
{
    public class ModuleHubConnectionManager
    {
        private readonly ConcurrentDictionary<string, HubConnection> _connections = new();
        private readonly ConcurrentDictionary<ModuleHubClient, string> _clients = new();
        private readonly IOptionsMonitor<ModuleHubConnectionOptions> _connectionOptions;
        private readonly object _lock = new();
        private Task? _startConnectionTask = default;

        public ModuleHubConnectionManager(IOptionsMonitor<ModuleHubConnectionOptions> connectionOptions)
        {
            _connectionOptions = connectionOptions;
        }

        public void Attach(ModuleHubClient client)
        {
            if (!_clients.TryAdd(client, client.ModuleHubName))
            {
                throw new Exception("Client is already attached");
            }

            var connection = EnsureConnection(client.EntryHubName);

            connection.Closed += client.OnConnectionClosed;
            connection.Reconnecting += client.OnConnectionReconnecting;
            connection.Reconnected += client.OnConnectionReconnected;
        }

        public void Detach(ModuleHubClient client)
        {
            var connection = ValidateClientAndGetConnection(client);

            connection.Closed -= client.OnConnectionClosed;
            connection.Reconnecting -= client.OnConnectionReconnecting;
            connection.Reconnected -= client.OnConnectionReconnected;

            _clients.TryRemove(client, out _);
        }

        public IDisposable On(ModuleHubClient client, string methodName, Type[] parameterTypes, Func<object[], object, Task> handler, object state)
        {
            var connection = ValidateClientAndGetConnection(client);
            return connection.On($"{client.ModuleHubName}_{methodName}", parameterTypes, handler, state);
        }

        public async Task<object> InvokeAsync(ModuleHubClient client, string methodName, Type returnType, object[] args, CancellationToken cancellationToken = default)
        {
            var connection = ValidateClientAndGetConnection(client);
            return await connection.InvokeCoreAsync($"{client.ModuleHubName}_{methodName}", returnType, args, cancellationToken);
        }

        public async Task SendAsync(ModuleHubClient client, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            var connection = ValidateClientAndGetConnection(client);
            await connection.SendCoreAsync($"{client.ModuleHubName}_{methodName}", args, cancellationToken);
        }

        public async Task<ChannelReader<object>> StreamAsChannelAsync(ModuleHubClient client, string methodName, Type returnType, object[] args, CancellationToken cancellationToken = default)
        {
            var connection = ValidateClientAndGetConnection(client);
            return await connection.StreamAsChannelCoreAsync($"{client.ModuleHubName}_{methodName}", returnType, args, cancellationToken);
        }

        public IAsyncEnumerable<TResult> StreamAsyncCore<TResult>(ModuleHubClient client, string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            var connection = ValidateClientAndGetConnection(client);
            return connection.StreamAsyncCore<TResult>($"{client.ModuleHubName}_{methodName}", args, cancellationToken);
        }

        public Task EnsureConnectionStartedAsync(ModuleHubClient client)
        {
            if (!_clients.ContainsKey(client))
            {
                throw new ArgumentException("The client is not attached", nameof(client));
            }

            var task = _startConnectionTask;
            if (task == null)
            {
                lock (_lock)
                {
                    task = _startConnectionTask;
                    if (task == null)
                    {
                        var connection = EnsureConnection(client.EntryHubName);

                        if (connection.State == HubConnectionState.Disconnected)
                        {
                            task = _startConnectionTask = connection.StartAsync();
                            _startConnectionTask.ContinueWith((t) => _startConnectionTask = null);
                        }
                        else
                        {
                            return Task.CompletedTask;
                        }
                    }
                }
            }

            return task;
        }

        public HubConnectionState GetState(ModuleHubClient client)
        {
            var connection = ValidateClientAndGetConnection(client);
            return connection.State;
        }

        public string GetConnectionId(ModuleHubClient client)
        {
            var connection = ValidateClientAndGetConnection(client);
            return connection.ConnectionId;
        }

        private HubConnection ValidateClientAndGetConnection(ModuleHubClient client)
        {
            if (!_clients.ContainsKey(client))
            {
                throw new ArgumentException("The client is not attached", nameof(client));
            }

            if (!_connections.TryGetValue(client.EntryHubName, out var connection))
            {
                throw new Exception("Connection for client not found");
            }

            return connection;
        }

        private HubConnection EnsureConnection(string entryHubName)
        {
            return _connections.GetOrAdd(entryHubName, (hubName) =>
            {
                var options = _connectionOptions.Get(entryHubName);
                var builder = new HubConnectionBuilder();

                if (options.Builder == null)
                {
                    throw new InvalidOperationException("No HubConnectionBuilder configured.");
                }

                options.Builder.Invoke(builder);
                var connection = builder.Build();
                return connection;
            });
        }
    }
}
