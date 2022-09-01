using Assets.Scripts.ServerModels;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Hub
{
    public class HtHubConnection : IDisposable
    {
        readonly GameCodeModel gameCode;
        readonly CancellationTokenSource cancellationTokenSource = new (TimeSpan.FromSeconds(Constants.HubTimeoutSeconds));
        private bool disposedValue;

        HubConnection hub;

        public HtHubConnection(GameCodeModel gameCodeModel)
        {
            gameCode = gameCodeModel;
        }

        public async Task ConnectAsync()
        {
            hub = new HubConnectionBuilder()
                .WithUrl(Constants.HubUrl)
                .Build();

            await hub.StartAsync(cancellationTokenSource.Token);
            await hub.InvokeAsync("JoinGameAsPlayer", gameCode, cancellationTokenSource.Token);
        }

        public async Task StopAsync()
        {
            await hub.StopAsync(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();

            // TODO: dispose and create new ones (do not reuse hubconnection)
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: check if GetResult doesn't result in deadlock
                    hub?.DisposeAsync().GetAwaiter().GetResult();
                    cancellationTokenSource.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~HubConnection()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
