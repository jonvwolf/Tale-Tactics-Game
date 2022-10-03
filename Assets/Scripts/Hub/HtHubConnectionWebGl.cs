#if UNITY_WEBGL

using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using Newtonsoft.Json;
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
        private static extern void ConnectJs(string gameCode, string url);
        [DllImport("__Internal")]
        private static extern void StopJs();
        [DllImport("__Internal")]
        private static extern void DisposeJs();

        readonly GameCodeModel gameCodeModel;

        bool isDisposed;

        public HtHubConnectionWebGl(GameCodeModel gameCodeModel)
        {
            this.gameCodeModel = gameCodeModel;

            Global.OnJsLinkEvent += Global_OnJsLinkEvent;
        }

        private void Global_OnJsLinkEvent(object sender, JsLinkEventArgs e)
        {
            const string OnHmCommandType = "OnHmCommand";
            const string OnHmPredefinedCommandType = "OnHmPredefinedCommand";
            const string OnConnectionStatusChangedType = "OnConnectionStatusChangedType";

            if (string.IsNullOrWhiteSpace(e.Data) || string.IsNullOrWhiteSpace(e.Type))
            {
                Debug.LogError("Type or Data is empty");
                return;
            }

            if (e.Type == OnHmCommandType)
            {
                Debug.Log($"Called {OnHmCommandType}");
                var args = CheckJsLinkData<HmCommandModel>(e.Data);
                if (args != default)
                {
                    var ev = OnHmCommand;
                    ev?.Invoke(null, args);
                }
            }
            else if (e.Type == OnHmPredefinedCommandType)
            {
                Debug.Log($"Called {OnHmPredefinedCommandType}");

                var args = CheckJsLinkData<HmCommandPredefinedModel>(e.Data);
                if (args != default)
                {
                    var ev = OnHmPredefinedCommand;
                    ev?.Invoke(null, args);
                }
            }
            else if (e.Type == OnConnectionStatusChangedType)
            {
                Debug.Log($"Called {OnConnectionStatusChangedType}");

                var args = CheckJsLinkData<HubConnectionStatusEventArgs>(e.Data);
                if (args != default)
                {
                    var ev = OnConnectionStatusChanged;
                    ev?.Invoke(null, args);
                }
            }
        }

        public static T CheckJsLinkData<T>(string data) where T : class
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<T>(data);
                if (obj == default)
                    throw new InvalidOperationException("After deserializing obj is null");
                return obj;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error trying to deserialize object: {e}");
            }
            return default;
        }

        public Task<bool> ConnectAsync()
        {
            Debug.Log($"Called {nameof(ConnectAsync)}");
            try
            {
                // we will assume it connects
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
            Debug.LogWarning($"{nameof(PlayerSendLog)} is disabled in WebGl");
            // Quit message is sent through StopJs
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            if (isDisposed)
                return Task.CompletedTask;

            isDisposed = true;
            Debug.Log($"Called {nameof(DisposeAsync)}");
            try
            {
                Global.OnJsLinkEvent -= Global_OnJsLinkEvent;
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