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
        public const float DefaultVolume = 5f;

        readonly static string BaseUrl = "https://ht-api.whostreaming.net";
        //readonly static string BaseUrl = "https://localhost:7216";

        public readonly static string HubUrl = $"{BaseUrl}/game-hub";

        public const int HubTimeoutSeconds = 300;
        public const int HubStopTimeoutSeconds = 150;

        public const float AudioFadeInTime = 5f;
        public const float AudioFadeOutTime = 5f;
        public const float ImageFadeInTime = 5f;
        public const float ImageFadeOutTime = 5f;

        public static string GetGameConfigurationUrl(string gameCode)
        {
            return $"{BaseUrl}/games/{gameCode}/configuration";
        }
    }
}
