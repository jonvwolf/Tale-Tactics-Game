using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsPrefabConroller : MonoBehaviour
{
    public Button BtnGoBack;
    public Button btnClearCache;
    public TMP_Text txtCurrentCache;
    public Button btnExitGame;
    public Toggle tglVsync;
    public Slider sldVolume;

    public Image imgExiting;
    public TMP_Text txtExiting;

    // Start is called before the first frame update
    void Start()
    {
        var model = Global.CurrentGameModel;
        if (model != default)
        {
            txtCurrentCache.text = model.GameCode;
        }
        else
        {
            txtCurrentCache.text = "<none>";
        }

        if (Global.CurrentScene == Constants.GameSceneName)
        {
            btnClearCache.gameObject.SetActive(false);
        }
        else if (Global.CurrentScene == Constants.LoadingSceneName)
        {
            btnClearCache.gameObject.SetActive(false);
        }
        else
        {
            btnExitGame.gameObject.SetActive(false);
        }

        btnClearCache.onClick.AddListener(BtnClearCache_Click);
        btnExitGame.onClick.AddListener(BtnExitGame_Click);
        BtnGoBack.onClick.AddListener(BtnGoBack_Click);

        var settings = Global.GetCurrentUserSettings();
        if (settings.VSync == Constants.VSyncEnabledValue)
            tglVsync.isOn = true;
        else
            tglVsync.isOn = false;

        sldVolume.value = settings.Volume.Value;

        tglVsync.onValueChanged.AddListener(TglVsync_ValueChanged);
        sldVolume.onValueChanged.AddListener(SldVolume_ValueChanged);

        Global.OnOkExitGame += OnOkExitGame;
    }

    void OnDestroy()
    {
        Global.OnOkExitGame -= OnOkExitGame;
    }

    void OnOkExitGame(object sender, EventArgs args)
    {
        // scene says OK to leave
        SceneManager.LoadScene(Constants.MainSceneName);
    }

    void SldVolume_ValueChanged(float value)
    {
        PlayerPrefs.SetFloat(Constants.VolumeSettingKey, value);

        Global.UserSettingsChanged(new Assets.Scripts.Models.UserSettingsEventArgs()
        {
            Volume = value
        });
    }

    void TglVsync_ValueChanged(bool value)
    {
        int val = Convert.ToInt32(value);
        PlayerPrefs.SetInt(Constants.VSyncSettingKey, val);

        Global.UserSettingsChanged(new Assets.Scripts.Models.UserSettingsEventArgs()
        {
            VSync = val
        });
    }

    void BtnGoBack_Click()
    {
        gameObject.SetActive(false);
        PlayerPrefs.Save();
    }

    void BtnClearCache_Click()
    {
        var model = Global.CurrentGameModel;
        if (model != default)
        {
            model.Dispose();
            Global.CurrentGameModel = default;
            txtCurrentCache.text = "<none>";
        }
    }

    void BtnExitGame_Click()
    {
        PlayerPrefs.Save();
        // trigger event
        Global.ExitGame();

        imgExiting.gameObject.SetActive(true);
        txtExiting.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
