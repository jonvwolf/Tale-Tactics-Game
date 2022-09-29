using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class JsLinkEventArgs : EventArgs
    {
        public string Type { get; set; }
        public string Data { get; set; }
    }
}
