using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalR.Modules.Client
{
    public abstract class ModuleHubClient : IDisposable
    {
        private readonly SubscriptionList _subscriptions = new();
        private bool _disposed;
        private bool _initialized;
        private ModuleHubConnectionManager _connectionManager;
        private string _mainHubName;
        private string _moduleHubName;

        public void Initialize(ModuleHubConnectionManager connectionManager, string mainHubName, string moduleHubName)
        {
            if (_initialized)
            {
                throw new InvalidOperationException($"This {nameof(ModuleHubClient)} was already initialized");
            }

            _connectionManager = connectionManager;
            _mainHubName = mainHubName;
            _moduleHubName = moduleHubName;

            _connectionManager.Attach(this);
            _initialized = true;
            OnInitialized();
        }

        public string MainHubName => _mainHubName;

        public string ModuleHubName => _moduleHubName;

        public HubConnectionState State => _connectionManager.GetState(this);

        public string ConnectionId => _connectionManager.GetConnectionId(this);

        public IDisposable On(string methodName, Type[] parameterTypes, Func<object[], object, Task> handler, object state)
        {
            CheckState();
            var hubConnectionSubscription = _connectionManager.On(this, methodName, parameterTypes, handler, state);

            // the hub connection is shared between module hub clients.
            // when this client gets disposed, we must unsubscribe all handlers registered by this client.
            var subscription = new Subscription(hubConnectionSubscription, _subscriptions);
            _subscriptions.Add(subscription);
            return subscription;
        }

        public async Task EnsureConnectionStartedAsync()
        {
            await _connectionManager.EnsureConnectionStartedAsync(this);
        }

        public async Task<object> InvokeCoreAsync(string methodName, Type returnType, object[] args, CancellationToken cancellationToken = default)
        {
            CheckState();
            return await _connectionManager.InvokeAsync(this, methodName, returnType, args, cancellationToken);
        }

        public Task SendCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            CheckState();
            return _connectionManager.SendAsync(this, methodName, args, cancellationToken);
        }

        public async Task<ChannelReader<object>> StreamAsChannelCoreAsync(string methodName, Type returnType, object[] args, CancellationToken cancellationToken = default)
        {
            CheckState();
            return await _connectionManager.StreamAsChannelAsync(this, methodName, returnType, args, cancellationToken);
        }

        public IAsyncEnumerable<TResult> StreamAsyncCore<TResult>(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            CheckState();
            return _connectionManager.StreamAsyncCore<TResult>(this, methodName, args, cancellationToken);
        }

        public virtual Task OnConnectionReconnected(string arg)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnConnectionReconnecting(Exception arg)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnConnectionClosed(Exception arg)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnInitialized()
        {
        }

        /// <summary>
        /// Releases all resources currently used by this <see cref="ModuleHubClient"/> instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this method is being invoked by the <see cref="Dispose()"/> method,
        /// otherwise <c>false</c>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            var subscriptions = _subscriptions.GetSubscriptions();
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            _connectionManager.Detach(this);
            _disposed = true;
        }

        private void CheckState()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (!_initialized)
            {
                throw new InvalidOperationException($"This {GetType().Name} was not initialized.");
            }
        }

        private class Subscription : IDisposable
        {
            private readonly IDisposable _innerSubscription;
            private readonly SubscriptionList _subscriptions;

            public Subscription(IDisposable innerSubscription, SubscriptionList subscriptions)
            {
                _innerSubscription = innerSubscription;
                _subscriptions = subscriptions;
            }

            public void Dispose()
            {
                _innerSubscription.Dispose();
                _subscriptions.Remove(this);
            }
        }

        private class SubscriptionList
        {
            private readonly List<Subscription> _subscriptions = new();

            internal IDisposable[] GetSubscriptions()
            {
                lock (_subscriptions)
                {
                    return _subscriptions.ToArray();
                }
            }

            internal void Add(Subscription subscription)
            {
                lock (_subscriptions)
                {
                    _subscriptions.Add(subscription);
                }
            }

            internal void Remove(Subscription subscription)
            {
                lock (_subscriptions)
                {
                    _subscriptions.Remove(subscription);
                }
            }
        }
    }
}
