using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class Global
    {
        /// <summary>
        /// For LoadingScene
        /// </summary>
        static CurrentGameConfigurationModel readGameConfigurationModel;
        /// <summary>
        /// For Game scene
        /// </summary>
        public static CurrentGameModel CurrentGameModel { get; set; }

        public static string CurrentScene { get; set; } = string.Empty;

        public static CurrentGameConfigurationModel GetGameConfiguration()
        {
            // TODO: should copy model?
            return readGameConfigurationModel;
        }

        public static void SetGameConfiguration(CurrentGameConfigurationModel model)
        {
            // TODO: should copy the model?
            readGameConfigurationModel = model;
        }
    }
}
