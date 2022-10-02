using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class CurrentGameModel : IDisposable
    {
        public string GameCode { get; }

        readonly Dictionary<long, LoadedImageAssetModel> _images = new();
        readonly Dictionary<long, LoadedAudioAssetModel> _audios = new();
        private bool disposedValue;

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

        public LoadedAudioAssetModel GetAudio(long id)
        {
            if (_audios.TryGetValue(id, out var model))
            {
                return model;
            }
            return default;
        }

        public LoadedImageAssetModel GetImage(long id)
        {
            if (_images.TryGetValue(id, out var model))
            {
                return model;
            }
            return default;
        }

        public Dictionary<long, LoadedAudioAssetModel> GetAllAudios()
        {
            return new Dictionary<long, LoadedAudioAssetModel>(_audios);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var image in _images)
                    {
                        UnityEngine.Object.Destroy(image.Value.Texture);
                    }
                    foreach (var image in _images)
                    {
                        UnityEngine.Object.Destroy(image.Value.Sprite);
                    }
                    foreach (var audio in _audios)
                    {
                        UnityEngine.Object.Destroy(audio.Value.AudioClip);
                    }

                    _images.Clear();
                    _audios.Clear();
                }

                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CurrentGameModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
