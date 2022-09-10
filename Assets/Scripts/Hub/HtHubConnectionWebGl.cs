#if UNITY_WEBGL

using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using System;
using System.Threading.Tasks;

namespace Assets.Scripts.Hub
{
    public class HtHubConnectionWebGl : IHtHubConnection
    {
        
        public event EventHandler<HmCommandModel> OnHmCommand;
        public event EventHandler<HmCommandPredefinedModel> OnHmPredefinedCommand;
        public event EventHandler<HubConnectionStatusEventArgs> OnConnectionStatusChanged;


        public HtHubConnectionWebGl(GameCodeModel gameCodeModel)
        {
            
        }

        public async Task<bool> ConnectAsync()
        {
            return false;    
        }

        public async Task StopAsync()
        {
            
        }

        public async Task PlayerSendLog(TextLogModel model)
        {
            
        }

        public async Task DisposeAsync()
        {
            
        }
    }
}
#endif