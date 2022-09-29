#if UNITY_WEBGL

using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Hub
{
    public class HtHubConnectionWebGl : IHtHubConnection
    {
        public event EventHandler<HmCommandModel> OnHmCommand;
        public event EventHandler<HmCommandPredefinedModel> OnHmPredefinedCommand;
        public event EventHandler<HubConnectionStatusEventArgs> OnConnectionStatusChanged;

        [DllImport("__Internal")]
        //private static extern void ConnectJs(string gameCode, string url, Action<string> connectionEventCallback, Action<string, string> onCallbacks);
        private static extern void ConnectJs(string gameCode, string url);
        [DllImport("__Internal")]
        private static extern void StopJs();
        [DllImport("__Internal")]
        private static extern void DisposeJs();

        readonly GameCodeModel gameCodeModel;

        public HtHubConnectionWebGl(GameCodeModel gameCodeModel)
        {
            this.gameCodeModel = gameCodeModel;
        }

        void OnConnectionEventCallback(string data)
        {
            Debug.Log($"Data: {data}");
        }

        void OnOnCallBacks(string type, string data)
        {
            Debug.Log($"Type: {type} Data: {data}");
        }

        public Task<bool> ConnectAsync()
        {
            Debug.Log($"Called {nameof(ConnectAsync)}");
            try
            {
                // we will assume it connects
                //ConnectJs(gameCodeModel.GameCode, Constants.HubUrl, OnConnectionEventCallback, OnOnCallBacks);
                ConnectJs(gameCodeModel.GameCode, Constants.HubUrl);
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Debug.LogError("(Hub) Failed to connect: " + e.ToString());
                ConnectionStatusChanged(new HubConnectionStatusEventArgs()
                {
                    FailedToConnect = true,
                    Exception = e
                });
                return Task.FromResult(false);
            }
        }

        private void ConnectionStatusChanged(HubConnectionStatusEventArgs args)
        {
            var ev = OnConnectionStatusChanged;
            ev?.Invoke(null, args);
        }

        public Task StopAsync()
        {
            Debug.Log($"Called {nameof(StopAsync)}");
            try
            {
                StopJs();
            }
            catch (Exception e)
            {
                Debug.LogError("(Hub) Failed to stop: " + e.ToString());
            }

            return Task.CompletedTask;
        }

        public Task PlayerSendLog(TextLogModel model)
        {
            // Quit message is sent through StopJs
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Debug.Log($"Called {nameof(DisposeAsync)}");
            try
            {
                DisposeJs();
            }
            catch (Exception e)
            {
                Debug.LogError("(Hub) Failed to dispose: " + e.ToString());
            }
            
            return Task.CompletedTask;
        }
    }
}
#endif