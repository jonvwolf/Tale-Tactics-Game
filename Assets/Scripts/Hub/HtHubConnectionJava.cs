#if UNITY_ANDROID

using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Hub
{
    public class HtHubConnectionJava : IHtHubConnection
    {
        public event EventHandler<HmCommandModel> OnHmCommand;
        public event EventHandler<HmCommandPredefinedModel> OnHmPredefinedCommand;
        public event EventHandler<HubConnectionStatusEventArgs> OnConnectionStatusChanged;

        AndroidJavaObject myplugin;

        readonly GameCodeModel gameCodeModel;

        bool isDisposed;

        public HtHubConnectionJava(GameCodeModel gameCodeModel)
        {
            this.gameCodeModel = gameCodeModel;

            //myplugin = new AndroidJavaObject("com.taletactics.myplugin.MyPlugin");
            myplugin = new AndroidJavaObject("com.example.mylib.MyPlugin");
        }

        
        public Task<bool> ConnectAsync()
        {
            //var result = myplugin.Call<int>("Add", 5, 1);
            //Debug.Log("FROM JAVA!: " + result);
            var ok = myplugin.Call<string>("GetStr");
            Debug.Log("FROM JAVA!2: " + ok);
            return Task.FromResult(true);
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

        public Task PlayerSendLog(TextLogModel model)
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            myplugin.Dispose();
            return Task.CompletedTask;
        }
    }
}
#endif