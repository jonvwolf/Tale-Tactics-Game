using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class HubConnectionStatusEventArgs
    {
        public bool Reconnected { get; set; }
        public bool IsReconnecting { get; set; }
        public bool Disconnected { get; set; }
        public bool FailedToConnect { get; set; }
        public bool InvokeFailed { get; set; }
        public bool ConnectedByJs { get; set; }
        [JsonIgnore]
        public Exception Exception { get; set; }

        public override string ToString()
        {
            return $"Reconnected: {Reconnected} IsReconnecting: {IsReconnecting} Disconnected: {Disconnected}" +
                $"FailedToConnect: {FailedToConnect} InvokedFailed: {InvokeFailed} ConnectedByJs: {ConnectedByJs}" + 
                $"Exception: {Exception?.ToString()}";
        }
    }
}
