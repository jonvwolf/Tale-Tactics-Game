using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class CurrentGameModel
    {
        public string GameCode { get; }

        readonly Dictionary<long, LoadedImageAssetModel> _images = new();
        readonly Dictionary<long, LoadedAudioAssetModel> _audios = new();

        public IReadOnlyDictionary<long, LoadedImageAssetModel> LoadedImages => _images;
        public IReadOnlyDictionary<long, LoadedAudioAssetModel> LoadedAudios => _audios;
        public CurrentGameModel(string gameCode)
        {
            GameCode = gameCode;
        }

        public void AddImage(long id, LoadedImageAssetModel model)
        {
            _images[id] = model;
        }

        public void AddAudio(long id, LoadedAudioAssetModel model)
        {
            _audios[id] = model;
        }
    }
}
