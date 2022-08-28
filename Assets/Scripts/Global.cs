using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

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

        public static event EventHandler<UserSettingsEventArgs> OnUserSettingsChanged;
        
        public static void UserSettingsChanged(UserSettingsEventArgs args)
        {
            var ev = OnUserSettingsChanged;
            ev?.Invoke(null, args);
        }

        public static UserSettingsEventArgs GetCurrentUserSettings()
        {
            var volume = PlayerPrefs.GetFloat(Constants.VolumeSettingKey, float.MinValue);
            if (volume == float.MinValue)
                volume = Constants.DefaultVolume;

            var vsync = PlayerPrefs.GetInt(Constants.VSyncSettingKey, int.MinValue);
            if (vsync == int.MinValue)
                vsync = Constants.VSyncEnabledValue;

            return new UserSettingsEventArgs()
            {
                Volume = volume,
                VSync = vsync
            };
        }

        public static void ApplyUserSettings(UserSettingsEventArgs args, AudioMixer mixer = default)
        {
            if (args.VSync.HasValue)
            {
                if (QualitySettings.vSyncCount != args.VSync.Value)
                    QualitySettings.vSyncCount = args.VSync.Value;
                Debug.Log("vSyncCount = " + QualitySettings.vSyncCount);
            }

            // TODO: Not sure if everysingle Start of scene, I have to do this...
            if (args.Volume.HasValue && mixer != default)
                mixer.SetFloat(Constants.MixerMasterVolumeKey, Mathf.Log10(args.Volume.Value) * 20);
        }

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
