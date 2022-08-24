using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class CurrentGameConfigurationModel
    {
        public string GameCode { get; }
        public ReadGameConfigurationModel ReadGameConfigurationModel { get; }

        public CurrentGameConfigurationModel(string gameCode, ReadGameConfigurationModel model)
        {
            GameCode = gameCode;
            ReadGameConfigurationModel = model;
        }
    }
}
