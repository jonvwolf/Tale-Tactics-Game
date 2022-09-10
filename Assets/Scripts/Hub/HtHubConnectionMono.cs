#if UNITY_EDITOR || PLATFORM_SUPPORTS_MONO

using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Hub
{
    public class HtHubConnectionMono : IHtHubConnection
    {
        readonly GameCodeModel gameCode;
        private bool disposedValue;

        HubConnection hub;

        public event EventHandler<HmCommandModel> OnHmCommand;
        public event EventHandler<HmCommandPredefinedModel> OnHmPredefinedCommand;
        public event EventHandler<HubConnectionStatusEventArgs> OnConnectionStatusChanged;

        IDisposable playerReceiveHmCommandHandler;
        IDisposable playerReceiveHmCommandPredefinedHandler;

        public HtHubConnectionMono(GameCodeModel gameCodeModel)
        {
            gameCode = gameCodeModel;
        }

        public async Task<bool> ConnectAsync()
        {
            if (hub != default)
            {
                await StopAsync();
            }

            try
            {
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.HubTimeoutSeconds));

                hub = new HubConnectionBuilder()
                    .WithUrl(Constants.HubUrl, Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets, (options) =>
                    {
                        options.SkipNegotiation = true;
                    })
                    .WithAutomaticReconnect()
                    .Build();

                await hub.StartAsync(cancellationTokenSource.Token);

                hub.Closed += Hub_Closed;
                hub.Reconnected += Hub_Reconnected;
                hub.Reconnecting += Hub_Reconnecting;

                playerReceiveHmCommandHandler = hub.On("PlayerReceiveHmCommand", (Func<HmCommandModel, Task>)((model) =>
                {
                    var ev = OnHmCommand;
                    ev?.Invoke(null, model);
                    return PlayerSendBackHmCommandAsync(model);
                }));

                playerReceiveHmCommandPredefinedHandler = hub.On("PlayerReceiveHmCommandPredefined", (Func<HmCommandPredefinedModel, Task>)((model) =>
                {
                    var ev = OnHmPredefinedCommand;
                    ev?.Invoke(null, model);
                    return PlayerSendBackHmCommandPredefined(model);
                }));
            }
            catch (Exception e)
            {
                if (hub != default)
                {
                    hub.Closed -= Hub_Closed;
                    hub.Reconnected -= Hub_Reconnected;
                    hub.Reconnecting -= Hub_Reconnecting;

                    playerReceiveHmCommandHandler?.Dispose();
                    playerReceiveHmCommandPredefinedHandler?.Dispose();

                    if (playerReceiveHmCommandHandler != default)
                        hub.Remove("PlayerReceiveHmCommand");

                    if (playerReceiveHmCommandPredefinedHandler!= default)
                        hub.Remove("PlayerReceiveHmCommandPredefined");

                    await hub.DisposeAsync();
                }

                hub = default;

                Debug.LogError("(Hub) Failed to connect: " + e.ToString());
                ConnectionStatusChanged(new HubConnectionStatusEventArgs()
                {
                    FailedToConnect = true,
                    Exception = e
                });
                return false;
            }

            try
            {
                await hub.InvokeAsync("JoinGameAsPlayer", gameCode);
                await PlayerSendLog(new TextLogModel()
                {
                    From = "A player",
                    Message = "Has joined"
                });
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("(Hub) Invoke error: " + e.ToString());
                ConnectionStatusChanged(new HubConnectionStatusEventArgs()
                {
                    InvokeFailed = true,
                    Exception = e
                });
                return false;
            }
        }

        private void ConnectionStatusChanged(HubConnectionStatusEventArgs args)
        {
            var ev = OnConnectionStatusChanged;
            ev?.Invoke(null, args);
        }

        private Task Hub_Reconnecting(Exception arg)
        {
            Debug.LogError("(Reconnecting) Hub exception: " + (arg?.ToString() ?? "<null>"));

            ConnectionStatusChanged(new HubConnectionStatusEventArgs()
            {
                IsReconnecting = true,
                Exception = arg
            });
            return Task.CompletedTask;
        }

        private Task Hub_Reconnected(string arg)
        {
            ConnectionStatusChanged(new HubConnectionStatusEventArgs()
            {
                Reconnected = true
            });
            return Task.CompletedTask;
        }

        private Task Hub_Closed(Exception arg)
        {
            Debug.LogError("(Closed) Hub exception: " + (arg?.ToString() ?? "<null>"));
            ConnectionStatusChanged(new HubConnectionStatusEventArgs()
            {
                Disconnected = true,
                Exception = arg
            });
            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (hub == default)
            {
                Debug.LogWarning("Hub is default");
                return;
            }

            try
            {
                hub.Closed -= Hub_Closed;
                hub.Reconnected -= Hub_Reconnected;
                hub.Reconnecting -= Hub_Reconnecting;

                playerReceiveHmCommandHandler?.Dispose();
                playerReceiveHmCommandPredefinedHandler?.Dispose();

                if (playerReceiveHmCommandHandler != default)
                    hub.Remove("PlayerReceiveHmCommand");

                if (playerReceiveHmCommandPredefinedHandler != default)
                    hub.Remove("PlayerReceiveHmCommandPredefined");

                try
                {
                    using var localToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.HubStopTimeoutSeconds));
                    await hub.StopAsync(localToken.Token);
                }
                finally
                {
                    await hub.DisposeAsync();
                    hub = default;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("(Hub) Stop error: " + e.ToString());
            }
        }

        async Task PlayerSendBackHmCommandAsync(HmCommandModel model)
        {
            try
            {
                await hub.SendAsync("PlayerSendBackHmCommand", gameCode, model);
                Debug.Log("Send back HmCommand: " + model);
            }
            catch (Exception e)
            {
                Debug.LogError("(Hub) Invoke error: " + e.ToString());
                ConnectionStatusChanged(new HubConnectionStatusEventArgs()
                {
                    InvokeFailed = true,
                    Exception = e
                });
            }
        }

        async Task PlayerSendBackHmCommandPredefined(HmCommandPredefinedModel model)
        {
            try
            {
                await hub.SendAsync("PlayerSendBackHmCommandPredefined", gameCode, model);
                Debug.Log("Send back HmCommandPredefined: " + model);
            }
            catch (Exception e)
            {
                Debug.LogError("(Hub) Invoke error: " + e.ToString());
                ConnectionStatusChanged(new HubConnectionStatusEventArgs()
                {
                    InvokeFailed = true,
                    Exception = e
                });
            }
        }

        public async Task PlayerSendLog(TextLogModel model)
        {
            try
            {
                await hub.SendAsync("PlayerSendLog", gameCode, model);
            }
            catch (Exception e)
            {
                Debug.LogError("(Hub) Invoke error: " + e.ToString());
                ConnectionStatusChanged(new HubConnectionStatusEventArgs()
                {
                    InvokeFailed = true,
                    Exception = e
                });
            }
        }

        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (hub != default)
                        await hub.DisposeAsync();

                    playerReceiveHmCommandHandler?.Dispose();
                    playerReceiveHmCommandPredefinedHandler?.Dispose();
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

        public async Task DisposeAsync()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            await DisposeAsync(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
#endif