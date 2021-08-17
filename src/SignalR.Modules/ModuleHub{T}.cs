using Microsoft.AspNetCore.SignalR;
using System;

namespace SignalR.Modules
{
    /// <summary>
    /// A base class for a SignalR Module hub.
    /// </summary>
    public abstract class ModuleHub<T> : ModuleHub
        where T : class
    {
        private bool _disposed;
        private IHubCallerClients<T> _clients = default!;

        /// <summary>
        /// Gets or sets an object that can be used to invoke methods on the clients connected to this hub.
        /// </summary>
        public new IHubCallerClients<T> Clients
        {
            get
            {
                CheckDisposed();
                return _clients;
            }

            set
            {
                CheckDisposed();
                _clients = value;
            }
        }

        /// <summary>
        /// Releases all resources currently used by this <see cref="Hub"/> instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this method is being invoked by the <see cref="Dispose()"/> method,
        /// otherwise <c>false</c>.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
