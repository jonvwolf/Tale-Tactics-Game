using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class LoadedImageAssetModel
    {
        public Texture2D Texture { get; }

        public ReadImageModel Model { get; }

        public LoadedImageAssetModel(Texture2D texture, ReadImageModel model)
        {
            Texture = texture;
            Model = model;
        }
    }
}
