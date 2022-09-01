using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ServerModels
{
    public class GameCodeModel
    {
        public string GameCode { get; set; }

        public GameCodeModel(string gameCode)
        {
            GameCode = gameCode;
        }
    }
}
