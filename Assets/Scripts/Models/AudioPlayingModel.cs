using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    internal class AudioPlayingModel
    {
        public AudioSource AudioSource { get; set; }
        public Coroutine Coroutine { get; set; }
    }
}
