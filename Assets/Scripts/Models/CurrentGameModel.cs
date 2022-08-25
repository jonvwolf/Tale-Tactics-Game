using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class CurrentGameModel
    {
        public IReadOnlyDictionary<long, LoadedImageAssetModel> LoadedImages { get; }
        public IReadOnlyDictionary<long, LoadedAudioAssetModel> LoadedAudios { get; }
        public CurrentGameModel(Dictionary<long, LoadedImageAssetModel> images, Dictionary<long, LoadedAudioAssetModel> audios)
        {
            LoadedImages = images;
            LoadedAudios = audios;
        }
    }
}
