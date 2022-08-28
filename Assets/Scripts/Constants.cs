using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class Constants
    {
        public readonly static bool IsDebug = true;
        public readonly static string LoadingSceneName = "LoadingScene";
        public readonly static string MainSceneName = "MainScene";
        public readonly static string GameSceneName = "GameScene";

        public readonly static float WaitForSecondsAfterEachLoadAsset = 0.450f;

        public const string VSyncSettingKey = "vsync";
        public const string VolumeSettingKey = "volume";
        public const string MixerMasterVolumeKey = "mastervol";
        public const int VSyncEnabledValue = 1;

        readonly static string BaseUrl = "https://ht-api.whostreaming.net";
        public static string GetGameConfigurationUrl(string gameCode)
        {
            return $"{BaseUrl}/games/{gameCode}/configuration";
        }
    }
}
