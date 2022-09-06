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
        public Sprite Sprite { get; }

        public ReadImageModel Model { get; }

        public LoadedImageAssetModel(Texture2D texture, ReadImageModel model)
        {
            Texture = texture;
            Model = model;
            Sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
