using Assets.Scripts.Models;
using Assets.Scripts.ServerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Hub
{
    public interface IHtHubConnection
    {
        public event EventHandler<HmCommandModel> OnHmCommand;
        public event EventHandler<HmCommandPredefinedModel> OnHmPredefinedCommand;
        public event EventHandler<HubConnectionStatusEventArgs> OnConnectionStatusChanged;

        public Task PlayerSendLog(TextLogModel model);
        public Task DisposeAsync();
        public Task StopAsync();
        public Task<bool> ConnectAsync();
    }
}
