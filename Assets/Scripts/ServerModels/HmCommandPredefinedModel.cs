using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ServerModels
{
    public class HmCommandPredefinedModel
    {
        public bool? ClearScreen { get; set; }
        public bool? StopSoundEffects { get; set; }
        public bool? StopBgm { get; set; }

        public override string ToString()
        {
            return $"Clearscreen: {ClearScreen} Stopsoundeffects: {StopSoundEffects} Stopbgm: {StopBgm}";
        }
    }
}
