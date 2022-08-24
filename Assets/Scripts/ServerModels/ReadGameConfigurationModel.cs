using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class ReadGameConfigurationModel
    {
        public IReadOnlyList<ReadImageModel> Images { get; set; }
        public IReadOnlyList<ReadAudioModel> Audios { get; set; }
        public IReadOnlyList<ReadMinigameModel> Minigames { get; set; }
    }
}
