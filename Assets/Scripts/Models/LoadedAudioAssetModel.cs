using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class LoadedAudioAssetModel
    {
        public AudioClip AudioClip { get; }

        public ReadAudioModel Model { get; }

        public LoadedAudioAssetModel(AudioClip audioClip, ReadAudioModel model)
        {
            AudioClip = audioClip;
            Model = model;
        }
    }
}
