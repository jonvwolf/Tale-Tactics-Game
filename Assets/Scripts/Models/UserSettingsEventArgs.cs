using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class UserSettingsEventArgs : EventArgs
    {
        public int? VSync { get; set; }
        public float? Volume { get; set; }
        public int? BiggerGameText { get; set; }
    }
}
